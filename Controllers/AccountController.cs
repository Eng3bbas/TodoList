using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoList.Data;
using TodoList.Data.Entities;
using TodoList.Http.Requests;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using AutoMapper;
using TodoList.Data.DTOs;

namespace TodoList.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly IMapper mapper;
        public AccountController(IWebHostEnvironment env,IMapper mapper)
        {
            this.env = env;
            this.mapper = mapper;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateAccountRequest request ,[FromServices] IRepository<User> repository)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await repository.Find(Guid.Parse(User.FindFirst("userId").Value));
            if ((await repository.Filter(u => u.Email == request.Email)).Count() > 1)
                return BadRequest(new { Email = new[] { "use Anthor Email" } });

            if(request.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Image.Name);
                if(user.Image != null)
                    System.IO.File.Delete(env.WebRootPath + $"/ProfileImages/{user.Image}");
                using(FileStream fs = new FileStream(Path.Combine(env.WebRootPath + "/ProfileImages" , fileName), FileMode.Create,FileAccess.ReadWrite))
                {
                   await request.Image.CopyToAsync(fs);
                }
                user.Image = $"ProfileImages/{fileName}";
            }
            user.Email = request.Email;
            user.Name = request.Name;
            if (request.Password != null)
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            return Ok(mapper.Map<UserDTO>(await repository.Update(user)));
        }
    }
}