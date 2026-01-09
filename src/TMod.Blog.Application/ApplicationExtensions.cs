using Microsoft.AspNetCore.Builder;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TMod.Blog.Application.Middlewares;

namespace TMod.Blog.Application
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UseApplicationMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandleMiddleware>();
            app.UseMiddleware<PackResultMiddleware>();
            return app;
        }
    }
}
