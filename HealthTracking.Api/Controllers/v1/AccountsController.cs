using AutoMapper;
using HealthTracking.Authentication.Configration;
using HealthTracking.Authentication.Models.Dtos.Generic;
using HealthTracking.Authentication.Models.Dtos.Incoming;
using HealthTracking.Authentication.Models.Dtos.Outgoing;
using HealthTracking.DataService.IConfigration;
using HealthTracking.Entity.DbSet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HealthTracking.Api.Controllers.v1
{
    public class AccountsController : BaseController
    {
        private readonly JwtConfig _jwtConfig;
        private readonly TokenValidationParameters _tokenValidationParameters;
        public AccountsController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> optionsMonitor,
            TokenValidationParameters tokenValidationParameters,
            IMapper mapper) : base(unitOfWork, userManager, mapper)
        {
            _jwtConfig = optionsMonitor.CurrentValue;
            _tokenValidationParameters = tokenValidationParameters;

            // it will check for the ValidateLifetime inside validate token while working on refresh token so iw will throw an exeption couse of the expiration
            // so we need to diabled it
            _tokenValidationParameters.ValidateLifetime = false;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(UserRegistrationRequestDto userRegistrationDto)
        {
            // check if the user exist before
            var userExist = await _userManager.FindByEmailAsync(userRegistrationDto.Email);

            if(userExist != null)
            {
                return BadRequest(new UserRegistrationResponseDto
                {
                    Success = false,
                    Errors = new List<string>() { "Email already in use" }
                });
            }

            // Create a new user
            var newUser = new IdentityUser()
            {
                Email = userRegistrationDto.Email,
                UserName = userRegistrationDto.Email,
                EmailConfirmed = true // TODO build email confirmation functionality
            };

            var isCreated = await _userManager.CreateAsync(newUser, userRegistrationDto.Password);

            if (!isCreated.Succeeded)
            {
                return BadRequest(new UserRegistrationResponseDto
                {
                    Success = false,
                    Errors = isCreated.Errors.Select(x => x.Description).ToList()
                });
            }

            // Adding user to the database
            var user = new User
            {
                IdentityId = new Guid(newUser.Id),
                FirstName = userRegistrationDto.FirstName,
                LastName = userRegistrationDto.LastName,
                Email = userRegistrationDto.Email,
                DateOfBirth = DateTime.Now,
                Country = "",
                PhoneNumber = "",
                status = 1
            };

            await _unitOfWork.Users.Add(user);
            await _unitOfWork.CompleteAsync();

            // Generete Jwt Token
            var token = await GenereteJwtToken(newUser);

            return Ok(new UserRegistrationResponseDto
            {
                Success = true,
                Token = token.JwtToken,
                RefreshToken = token.RefreshToken
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserLoginRequestDto userLoginDto)
        {
            var user = await _userManager.FindByEmailAsync(userLoginDto.Email);

            // check if the user exist before
            if (user == null)
            {
                return BadRequest(new UserRegistrationResponseDto
                {
                    Success = false,
                    Errors = new List<string>() { "Invalid Email or Password" }
                });
            }

            // check password
            bool validPassword = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);
            if (!validPassword)
            {
                return BadRequest(new UserRegistrationResponseDto
                {
                    Success = false,
                    Errors = new List<string>() { "Invalid Email or Password" }
                });
            }

            // Generete Jwt Token
            var tokens = await GenereteJwtToken(user);

            return Ok(new UserRegistrationResponseDto
            {
                Success = true,
                Token = tokens.JwtToken,
                RefreshToken = tokens.RefreshToken
            });

        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
        {
            var result = await VerifyToken(tokenRequestDto);

            if(result ==  null)
            {
                return BadRequest(new UserRegistrationResponseDto
                {
                    Success = false,
                    Errors = new List<string>() { "Token Validation faild" }
                });
            }
            return Ok(result);
        }

        private async Task<AuthResult> VerifyToken(TokenRequestDto tokenRequestDto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // check the validity of the token
                var principle = tokenHandler.ValidateToken(tokenRequestDto.Token, _tokenValidationParameters, out var validatedToken);

                // check if the string is an actual JWT token not a random string
                if(validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    // check for the hash algorithm same as our
                    var isOurHashAlgo = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if(!isOurHashAlgo) return null;
                }

                // check for the expiry date
                var utcExpiryDate = long.Parse(principle.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                // Convert to date then check
                var expDate = UnixTimeStampToDateTime(utcExpiryDate);

                // Checking if the jwt token has expired
                if(expDate > DateTime.UtcNow)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string> { "Jwt token has not expired" },
                    };
                }

                // check if the refresh token exist
                var refreshTokenExist = await _unitOfWork.RefreshTokens.GetByRefreshToken(tokenRequestDto.RefreshToken);

                if(refreshTokenExist == null)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string> { "Invalid Refresh Token" },
                    };
                }

                // Check the Expiry Date of the refresh token
                if(refreshTokenExist.ExpiryDate < DateTime.UtcNow)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string> { "Refresh Token has Expired please login again" },
                    };
                }

                // check if the refresh token has been used before
                if (refreshTokenExist.IsUsed)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string> { "Refresh Token has been used before" },
                    };
                }

                // check if the refresh token has been revoked
                if (refreshTokenExist.IsRevoked)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string> { "Refresh Token has been used revoked" },
                    };
                }


                var jti = principle.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if(jti != refreshTokenExist.JwtId)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string> { "Refresh Token Does not match the jwt token" },
                    };
                }

                // Start processing
                refreshTokenExist.IsUsed = true;

                var updateResult = await _unitOfWork.RefreshTokens.MarkRefreshTokenAsUsed(refreshTokenExist);

                if (!updateResult)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string> { "Error processing request" },
                    };
                }

                await _unitOfWork.CompleteAsync();

                var dbUser = await _userManager.FindByIdAsync(refreshTokenExist.UserId);

                if(dbUser == null)
                {
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new List<string> { "Error processing request" },
                    };
                }

                // Generate the token
                var tokens = await  GenereteJwtToken(dbUser);

                return new UserRegistrationResponseDto
                {
                    Success = true,
                    Token = tokens.JwtToken,
                    RefreshToken = tokens.RefreshToken
                };

            }
            catch (Exception ex)
            {
                // TODO Add better error Handling
                // TODO Add a logger
                return null;
            }
        }

        private DateTime UnixTimeStampToDateTime(long unixDate)
        {
            //Sets the time to 1, Jan, 1970
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            // Add seconds then return
            return dateTime.AddSeconds(unixDate).ToUniversalTime();
        }

        private async Task<TokenData> GenereteJwtToken(IdentityUser user)
        {
            // responsible forcreating the token
            var jwtHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id),
                    new Claim(ClaimTypes.NameIdentifier, user.Id), // To keep track for the logged in user To know who is that user from the token
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email), // unique
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // used by refresh token

                }),
                Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame), // TODO update Expire to minutes
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            // generate the security obj token
            var token = jwtHandler.CreateToken(tokenDescriptor);

            // convert the security token into a string
            var jwtToken = jwtHandler.WriteToken(token);

            //Generate a refresh token
            var refreshToken = new RefreshToken
            {
                AddedDate = DateTime.UtcNow,
                Token = $"{RamdomStringGenerator(25)}_{Guid.NewGuid()}",
                UserId = user.Id,
                IsUsed = false,
                IsRevoked = false,
                status = 1,
                JwtId = token.Id,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
            };

            await _unitOfWork.RefreshTokens.Add(refreshToken);
            await _unitOfWork.CompleteAsync();

            var tokenData = new TokenData
            {
                JwtToken = jwtToken,
                RefreshToken = refreshToken.Token
            };

            return tokenData;
        }

        private string RamdomStringGenerator(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

        }

    }
}
