using HealthTracking.DataService.Data;
using HealthTracking.DataService.IConfigration;
using HealthTracking.Entity.DbSet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracking.Api.Controllers.v1
{
    [Authorize]
    public class UsersController : BaseController
    {
        public UsersController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [HttpGet("TestRun")]
        public IActionResult TestRun()
        {
            return Ok("Success");
        }

        [HttpGet]

        public async Task<IActionResult> GetUsers()
        {
            var users = await _unitOfWork.Users.GetAll();
            if (users == null) return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(users);
        }

        [HttpGet]
        [Route("GetUser", Name = "GetUser")]
        public async Task<IActionResult> GetUser([FromQuery] Guid id)
        {
            var user = await _unitOfWork.Users.GetById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> AddUsers(User user)
        {
            await _unitOfWork.Users.Add(user);
            await _unitOfWork.CompleteAsync();
            return CreatedAtRoute("GetUser", new { id = user.Id }, user); // return where you can get that user
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            throw new NotImplementedException();
        }



    }
}
