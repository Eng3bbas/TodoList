using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoList.Http.Requests;
using TodoList.Services;

namespace TodoList.Controllers
{
    [Route("api/auth/{action}")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService service;
        public AuthenticationController(AuthenticationService service)
        {
            this.service = service;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await service.Register(request);
            if (response.Status == Http.Responses.AuthenticationResponse.StatusEnum.Fail)
                return BadRequest(new { Email = new string[] { "This email is used" } });
            return StatusCode(201, response);
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await service.Login(request);
            if (response.Status == Http.Responses.AuthenticationResponse.StatusEnum.Fail)
                return BadRequest(new { Error = "Email Not found or invalid password" });
            return Ok(response);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            string jti = HttpContext.User.FindFirst("jti").Value;
            string userId = HttpContext.User.FindFirst("userId").Value;
            await service.Logout(
                    jti,
                    userId
            );
            return NoContent();
        }
    }
}