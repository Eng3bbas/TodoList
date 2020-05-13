using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TodoList.Helpers;
namespace TodoList.Http.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class VerfiyTokenNotRevokedMiddleware
    {
        private readonly RequestDelegate _next;
        public VerfiyTokenNotRevokedMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            if(!httpContext.Request.Headers.ContainsKey("Authorization"))
                await _next.Invoke(httpContext);
            
            var token = httpContext.RequestServices.GetService(typeof(TokenHelper)) as TokenHelper;
            string jti = httpContext.User.FindFirst("jti")?.Value;
            string userId = httpContext.User.FindFirst("userId")?.Value;
            bool isRevoked = await token.IsRevoked(jti, userId);
            if (isRevoked)
            {
             httpContext.Response.StatusCode = 401;
             httpContext.Response.ContentType = "application/json";
             await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { Error = "Your token is revoked" }));
             await _next.Invoke(httpContext);
             return;
            }
                await _next.Invoke(httpContext);
        }

       
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class VerfiyTokenNotRevokedMiddlewareExtensions
    {
        public static IApplicationBuilder UseVerfiyTokenNotRevokedMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VerfiyTokenNotRevokedMiddleware>();
        }
    }
}
