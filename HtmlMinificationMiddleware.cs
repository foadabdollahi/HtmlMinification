using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.RegularExpressions;

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
            var stream = httpContext.Response.Body;

            try
            {
                using var bodyBuffer = new MemoryStream();
                httpContext.Response.Body = bodyBuffer;

                await _next(httpContext);

                bodyBuffer.Seek(0, SeekOrigin.Begin);

                var isHtml = httpContext.Response.ContentType?.ToLower().Contains("text/html") == true;
                if (isHtml && httpContext.Response.StatusCode == 200)
                {
                    using var reader = new StreamReader(bodyBuffer);
                    var responseBody = await reader.ReadToEndAsync();

                    responseBody = Linarize(responseBody);

                    var bytes = Encoding.UTF8.GetBytes(responseBody);

                    using var memoryStream = new MemoryStream(bytes);
                    memoryStream.Write(bytes, 0, bytes.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    httpContext.Response.ContentLength = bytes.Length; //set new Response length
                    await memoryStream.CopyToAsync(stream);
                }
                else
                {
                    await bodyBuffer.CopyToAsync(stream);
                }
            }
            finally
            {
                httpContext.Response.Body = stream;
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