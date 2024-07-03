using HealthTracking.Authentication.Configration;
using HealthTracking.Authentication.Models.Dtos.Incoming;
using HealthTracking.Authentication.Models.Dtos.Outgoing;
using HealthTracking.DataService.IConfigration;
using HealthTracking.Entity.DbSet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HealthTracking.Api.Controllers.v1
{
    public class AccountsController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        public AccountsController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> optionsMonitor) : base(unitOfWork)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
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
            var token = GenereteJwtToken(newUser);

            return Ok(new UserRegistrationResponseDto
            {
                Success = true,
                Token = token,
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
            var token = GenereteJwtToken(user);

            return Ok(new UserRegistrationResponseDto
            {
                Success = true,
                Token = token,
            });

        }

        private string GenereteJwtToken(IdentityUser user)
        {
            // responsible forcreating the token
            var jwtHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email), // unique
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // used by refresh token

                }),
                Expires = DateTime.UtcNow.AddHours(5), // TODO update Expire to minutes
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            // generate the security obj token
            var token = jwtHandler.CreateToken(tokenDescriptor);

            // convert the security token into a string
            var jwtToken = jwtHandler.WriteToken(token);

            return jwtToken;

        }

    }
}
