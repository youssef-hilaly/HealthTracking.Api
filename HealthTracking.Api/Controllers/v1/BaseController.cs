using AutoMapper;
using HealthTracking.DataService.IConfigration;
using HealthTracking.Entity.Dtos.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracking.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]

    public class BaseController : ControllerBase
    {
        protected IUnitOfWork _unitOfWork;
        protected UserManager<IdentityUser> _userManager;
        protected readonly IMapper _mapper;

        public BaseController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        internal Error PopulateError(int code, string message, string type)
        {
            return new Error
            {
                Code = code,
                Message = message,
                Type = type
            };
        }
    }
}
