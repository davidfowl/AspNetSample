using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;

namespace Microsoft.AspNet.Builder
{
    public static class StaticFileApplicationBuilderExtensions
    {
        public static ApplicationBuilder UseStaticFiles(this ApplicationBuilder app)
        {
            return app.Use(StaticFiles);
        }

        private static RequestDelegate StaticFiles(RequestDelegate next)
        {
            return context =>
            {
                // TODO: Very basic and insecure static files demo!
                return next(context);
            };
        }
    }
}
