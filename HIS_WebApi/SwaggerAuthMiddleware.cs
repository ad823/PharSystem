using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HIS_WebApi   
{
    /// <summary>
    /// Swagger Basic Auth 驗證中介層
    /// </summary>
    public class SwaggerBasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _username;
        private readonly string _password;

        public SwaggerBasicAuthMiddleware(RequestDelegate next, string username, string password)
        {
            _next = next;
            _username = username;
            _password = password;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                string authHeader = context.Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Substring("Basic ".Length).Trim();
                    var credentialString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                    var credentials = credentialString.Split(':', 2);

                    if (credentials.Length == 2 &&
                        credentials[0] == _username &&
                        credentials[1] == _password)
                    {
                        await _next(context);
                        return;
                    }
                }

                // 👇 這行是關鍵，會強迫瀏覽器彈出 Basic Auth 視窗
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Swagger UI\"";
                await context.Response.WriteAsync("Authorization required to access Swagger UI");
                return;
            }

            await _next(context);
        }

    }

    /// <summary>
    /// 提供 UseSwaggerBasicAuth 擴充方法
    /// </summary>
    public static class SwaggerBasicAuthExtension
    {
        public static IApplicationBuilder UseSwaggerBasicAuth(this IApplicationBuilder builder, string username, string password)
        {
            return builder.UseMiddleware<SwaggerBasicAuthMiddleware>(username, password);
        }
    }
}
