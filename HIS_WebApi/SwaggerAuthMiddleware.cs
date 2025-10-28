using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HIS_WebApi
{
    /// <summary>
    /// 強制 Swagger 使用 Basic Auth 驗證，每次進入都要重新輸入帳密。
    /// </summary>
    public class SwaggerBasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _username;
        private readonly string _password;

        public SwaggerBasicAuthMiddleware(RequestDelegate next, string username, string password)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));

            _username = !string.IsNullOrWhiteSpace(username)
                ? username
                : throw new SecurityException("Swagger username must not be empty.");

            _password = !string.IsNullOrWhiteSpace(password)
                ? password
                : throw new SecurityException("Swagger password must not be empty.");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 只攔截 /swagger 開頭的路徑
            if (context.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase))
            {
                string authHeader = context.Request.Headers["Authorization"];

                bool isAuthorized = false;

                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var token = authHeader.Substring("Basic ".Length).Trim();
                        var credentialString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                        var credentials = credentialString.Split(':', 2);

                        if (credentials.Length == 2 &&
                            credentials[0] == _username &&
                            credentials[1] == _password)
                        {
                            isAuthorized = true;
                        }
                    }
                    catch
                    {
                        isAuthorized = false;
                    }
                }

                if (!isAuthorized)
                {
                    // 每次都強制瀏覽器重新詢問帳密
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Swagger UI\"";
                    context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
                    context.Response.Headers["Pragma"] = "no-cache";
                    context.Response.Headers["Expires"] = "0";
                    await context.Response.WriteAsync("Authentication required for Swagger UI");
                    return;
                }

                // 通過驗證 → 清除瀏覽器快取，避免自動登入
                context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "0";
            }

            await _next(context);
        }
    }

    public static class SwaggerBasicAuthExtension
    {
        public static IApplicationBuilder UseSwaggerBasicAuth(this IApplicationBuilder builder, string username, string password)
        {
            return builder.UseMiddleware<SwaggerBasicAuthMiddleware>(username, password);
        }
    }
}
