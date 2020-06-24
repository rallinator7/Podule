using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApi.Services;
using WebApi.Models;
using System;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IJWTService _UserService;
        private readonly IDBService _DBService;

        public AuthController(IJWTService userService, IDBService DBService)
        {
            _UserService = userService;
            _DBService = DBService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateUserModel model)
        {
            var User = _DBService.GetUser(model.Email, model.Password);

            if (User == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var ReturnUser = _UserService.Authenticate(User);

            return Ok(ReturnUser);
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public IActionResult Create([FromBody]CreateUserModel model)
        {
            try
            {
                var User = _DBService.CreateUser(model.FirstName, model.LastName, model.Email, model.Company, model.Role, model.Password);

                if (User == null)
                    return BadRequest(new { message = "There is already an account associated with the email provided" });

                var ReturnUser = _UserService.Authenticate(User);

                return Ok(ReturnUser);
            }
            catch (Exception e)
            { 
                return Ok(e);
            }
        }

        [HttpGet("test")]
        public IActionResult GetAll()
        {      
            return Ok(":)");
        }
    }
}
