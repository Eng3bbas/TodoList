using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Helpers;
using TodoList.Http.Requests;
using TodoList.Http.Responses;
using System.IO;
using TodoList.Data;
using TodoList.Data.Entities;
using AutoMapper;
using TodoList.Data.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace TodoList.Services
{
    public class AuthenticationService
    {
        private readonly TokenHelper token;
        private readonly IWebHostEnvironment env;
        private readonly IRepository<User> repository;
        private readonly IMapper mapper;

        public AuthenticationService(TokenHelper token,IWebHostEnvironment env,IRepository<User> repository,IMapper mapper)
        {
            this.token = token;
            this.env = env;
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<AuthenticationResponse> Register(RegisterationRequest request)
        {
            if ((await repository.Filter(u => u.Email == request.Email)).Count() > 0)
                return new AuthenticationResponse { Status = AuthenticationResponse.StatusEnum.Fail, Token = null, UserData = null };
            User user = new User {
                Email = request.Email,
                Name = request.Name,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };
            if(request.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName);
                using(FileStream fs = new FileStream(Path.Combine(env.WebRootPath + "/ProfileImages", fileName),FileMode.Create))
                {
                    await request.Image.CopyToAsync(fs);
                }
                user.Image = "ProfileImages/" + fileName;
            }
            return new AuthenticationResponse
            {
                UserData = mapper.Map<UserDTO>(await repository.Create(user)),
                Token = token.Generate(user),
                Status = AuthenticationResponse.StatusEnum.OK
            };
        }

        public async Task<AuthenticationResponse> Login(LoginRequest request)
        {
            User user = (await repository.Filter(u => u.Email == request.Email)).Single();
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return new AuthenticationResponse { Status = AuthenticationResponse.StatusEnum.Fail };
            return new AuthenticationResponse
            {
                Status = AuthenticationResponse.StatusEnum.OK,
                UserData = mapper.Map<UserDTO>(user),
                Token = token.Generate(user)
            }; 
        }

        public async Task Logout(string jti, string userId)
        => await token.Revoke(jti, userId);

    }
}
