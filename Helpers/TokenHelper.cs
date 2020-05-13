using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TodoList.Data.Entities;
using System.IO;
using Newtonsoft.Json;

namespace TodoList.Helpers
{
    public class TokenHelper
    {
        public struct RevokedToken
        {
            public string JTI { get; set; }
            public string UserId { get; set; }
        }
        private readonly IConfiguration configuration;
        private readonly SymmetricSecurityKey secret;
        private readonly string revokedTokensPath;
        
        public TokenHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
            secret = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWT:Secret"]));
            revokedTokensPath = configuration["RevokedTokens"];
        }
        public string Generate(User user)
        {
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.AuthTime,DateTime.UtcNow.ToString()),
                    new Claim("userId",user.Id.ToString())
                }),
                Issuer = configuration["JWT:Issuer"],
                Audience = configuration["JWT:Issuer"],
                SigningCredentials = new SigningCredentials(secret,SecurityAlgorithms.HmacSha512Signature),
                
                Expires = DateTime.UtcNow.AddDays(30),
                
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(handler.CreateToken(descriptor));
        }

        public async Task<bool> IsRevoked(string jti , string userId)
        {
            var tokens = await GetRevokedTokens();
            return tokens.Contains(new RevokedToken
            {
                JTI = jti,
                UserId = userId
            });
        }

        public async Task Revoke(string jti,string userId)
        {
            var tokens = await GetRevokedTokens();
            RevokedToken revokedToken = new RevokedToken
            {
                JTI = jti,
                UserId = userId
            };
            if (!tokens.Contains(revokedToken))
            {
                tokens.Add(revokedToken);

                string json = JsonConvert.SerializeObject(tokens.ToArray());
                File.WriteAllText(revokedTokensPath, json);
            }
            
            
        }
        
        private async Task<IList<RevokedToken>> GetRevokedTokens()
        {
            return JsonConvert.DeserializeObject<IList<RevokedToken>>(File.ReadAllText(revokedTokensPath));
        }
    }

   
}
