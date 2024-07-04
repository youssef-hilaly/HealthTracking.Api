using AutoMapper;
using HealthTracking.Configration.Messages;
using HealthTracking.DataService.Data;
using HealthTracking.DataService.IConfigration;
using HealthTracking.Entity.DbSet;
using HealthTracking.Entity.Dtos.Errors;
using HealthTracking.Entity.Dtos.Generic;
using HealthTracking.Entity.Dtos.Incoming;
using HealthTracking.Entity.Dtos.Incoming.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracking.Api.Controllers.v1
{
    [Authorize]
    public class ProfileController : BaseController
    {
        public ProfileController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IMapper mapper) : base(unitOfWork, userManager, mapper) { }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            // any Request with a Jwt token The EF will process the Token and Do some verification then attach a User object based on the token to HttpContext
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            var result = new Result<User>();

            if (loggedInUser == null)
            {
                result.Error = PopulateError(400, ErrorsMessages.profile.UserNotFound, ErrorsMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            }


            var identityId = new Guid(loggedInUser.Id);

            var profile = await _unitOfWork.Users.GetByIdentityId(identityId);

            if (profile == null)
            {
                result.Error = PopulateError(400, ErrorsMessages.profile.UserNotFound, ErrorsMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            result.Content = profile;

            var userDto = _mapper.Map<UserDto>(profile);
            return Ok(userDto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            var result = new Result<User>();

            if (!ModelState.IsValid)
            {
                result.Error = PopulateError(400, ErrorsMessages.Generic.InvalidPayload, ErrorsMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if (loggedInUser == null)
            {
                result.Error = PopulateError(400, ErrorsMessages.profile.UserNotFound, ErrorsMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            var identityId = new Guid(loggedInUser.Id);

            var profile = await _unitOfWork.Users.GetByIdentityId(identityId);

            if (profile == null)
            {
                result.Error = PopulateError(400, ErrorsMessages.profile.UserNotFound, ErrorsMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            profile.Address = updateProfileDto.Address;
            profile.Country = updateProfileDto.Country;
            profile.Sex = updateProfileDto.Sex;
            profile.PhoneNumber = updateProfileDto.PhoneNumber;

            bool isUpdated = await _unitOfWork.Users.UpdateUserProfile(profile);


            if (!isUpdated)
            {
                result.Error = PopulateError(500, ErrorsMessages.Generic.SomethingWentWrong, ErrorsMessages.Generic.TypeInternalServerError);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            await _unitOfWork.CompleteAsync();

            result.Content = profile;

            var userDto = _mapper.Map<UserDto>(profile);

            return Ok(userDto);
        }
    }
}
