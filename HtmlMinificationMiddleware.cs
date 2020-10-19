using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HtmlMinification
{
    public class HtmlMinificationMiddleware
    {
        private readonly RequestDelegate _next;

        public HtmlMinificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Stream originalBody = httpContext.Response.Body;

            try
            {
                await using (var memStream = new MemoryStream())
                {
                    httpContext.Response.Body = memStream;
                    await _next.Invoke(httpContext);

                    memStream.Position = 0;
                    string responseBody = await new StreamReader(memStream).ReadToEndAsync();

                    responseBody = Linarize(responseBody);
                    byte[] byteArray = Encoding.UTF8.GetBytes(responseBody);

                    var newBody = new MemoryStream(byteArray);

                    await newBody.CopyToAsync(originalBody);
                }
            }
            finally
            {
                httpContext.Response.Body = originalBody;
            }
        }

        private string Linarize(string html)
        {
            html = Regex.Replace(html, "^\\s*", string.Empty, RegexOptions.Compiled | RegexOptions.Multiline);
            html = Regex.Replace(html, "\\r\\n", string.Empty, RegexOptions.Compiled | RegexOptions.Multiline);
            html = Regex.Replace(html, "<!--*.*?-->", string.Empty,
                RegexOptions.Compiled | RegexOptions.Multiline); //Remove comments

            return html.Replace("<!DOCTYPE html>", "<!DOCTYPE html>\r\n");
        }
    }
}
