using AutoMapper;
using HealthTracking.Authentication.Models.Dtos.Outgoing;
using HealthTracking.Configration.Messages;
using HealthTracking.DataService.Data;
using HealthTracking.DataService.IConfigration;
using HealthTracking.Entity.DbSet;
using HealthTracking.Entity.Dtos.Generic;
using HealthTracking.Entity.Dtos.Incoming;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace HealthTracking.Api.Controllers.v1
{
    [Authorize]
    public class UsersController : BaseController
    {
        public UsersController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IMapper mapper) : base(unitOfWork, userManager, mapper) { }

        [HttpGet("TestRun")]
        public IActionResult TestRun()
        {
            return Ok("Success");
        }

        [HttpGet]

        public async Task<IActionResult> GetUsers()
        {
            var result = new PagedResult<UserDto>();

            var users = await _unitOfWork.Users.GetAll();
            
            // {try catch} => catch returns a null
            if (users == null)
            {
                result.Error = PopulateError(500, ErrorsMessages.Generic.SomethingWentWrong, ErrorsMessages.Generic.TypeInternalServerError);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            var usersDto = _mapper.Map<List<UserDto>>(users);

            result.Content = usersDto;
            result.ResultCount = users.Count();
            return Ok(result);
        }

        [HttpGet]
        [Route("GetUser", Name = "GetUser")]
        public async Task<IActionResult> GetUser([FromQuery] Guid id)
        {
            var result = new Result<UserDto>();

            var user = await _unitOfWork.Users.GetById(id);
            if (user == null)
            {
                result.Error = PopulateError(400, ErrorsMessages.profile.UserNotFound, ErrorsMessages.Generic.TypeBadRequest);
                return BadRequest( result);
            }

            var userDto = _mapper.Map<UserDto>(user);
            result.Content = userDto;
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserDto userDto)
        {
            var result = new Result<User>();
            
            bool isExist = await _unitOfWork.Users.FindByEmail(userDto.Email) != null;

            if (isExist)
            {
                result.Error = PopulateError(400, ErrorsMessages.User.EmailInUse, ErrorsMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            var user = _mapper.Map<User>(userDto);

            var newUser = new IdentityUser()
            {
                Email = user.Email,
                UserName = user.Email,
                EmailConfirmed = true // TODO build email confirmation functionality
            };

            var isCreated = await _userManager.CreateAsync(newUser, "Default1.");

            if (!isCreated.Succeeded)
            {
                List<string> errors = isCreated.Errors.Select(e => e.Description).ToList();
                var stringErrors = String.Join(", ", errors);
                result.Error = PopulateError(500, stringErrors, ErrorsMessages.Generic.TypeInternalServerError);
                return BadRequest(result);
            }

            user.IdentityId = new Guid(newUser.Id);

            await _unitOfWork.Users.Add(user);
            await _unitOfWork.CompleteAsync();

            result.Content = user;

            return CreatedAtRoute("GetUser", new { id = user.Id }, result); // return where you can get that user
        }
    }
}
