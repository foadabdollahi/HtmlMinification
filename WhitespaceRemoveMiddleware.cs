using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FoadAbd.MyMiddleware
{
    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseWhitespaceRemoveMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<WhitespaceRemoveMiddleware>();
        }
    }

    public class WhitespaceRemoveMiddleware
    {
        private readonly RequestDelegate _next;

        public WhitespaceRemoveMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        { 

            using (var memoryStream = new MemoryStream())
            {
                var bodyStream = context.Response.Body;
                context.Response.Body = memoryStream;

                await _next(context);

                var isHtml = context.Response.ContentType?.ToLower().Contains("text/html");
                if (context.Response.StatusCode == 200 && isHtml.GetValueOrDefault())
                {
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        using (var streamReader = new StreamReader(memoryStream))
                        {
                            var responseBody = await streamReader.ReadToEndAsync();
                            responseBody = SpaceRemover(responseBody);

                            context.Response.Body = bodyStream;

                            await context.Response.WriteAsync(responseBody);
                             
                        }
                    }
                }
            }
        }

        private string SpaceRemover(string s)
        {
            s = Regex.Replace(s, "\\r|\\n", " ", RegexOptions.Compiled | RegexOptions.Multiline);
            s = Regex.Replace(s, "<!--*.*?-->", string.Empty, RegexOptions.Compiled | RegexOptions.Multiline);//Remove comments
            s = Regex.Replace(s, @"(?<=>)\s+?(?=<)", string.Empty).Trim();
            s = Regex.Replace(s, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Multiline);
            return s.Replace("<!DOCTYPE html>", "<!DOCTYPE html>\r\n");
        }
    }



}
