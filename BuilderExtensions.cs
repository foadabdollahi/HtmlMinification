using System;
using Microsoft.AspNetCore.Builder;

namespace HtmlMinification
{
    public static class BuilderExtensions
    {
         
        public static IApplicationBuilder UseHtmlMinification(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HtmlMinificationMiddleware>();
        }
    }
}
