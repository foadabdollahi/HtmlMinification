using Microsoft.AspNetCore.Builder;
using System;

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
