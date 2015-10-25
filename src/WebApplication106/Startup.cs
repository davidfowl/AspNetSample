using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace WebApplication106
{
    public class Startup
    {
        public static void Configure(ApplicationBuilder app)
        {
            app.UseStaticFiles();

            app.Run(context =>
            {
                return context.Response.WriteAsync("Hello World!");
            });
        }

        public static void Main(string[] args) => WebApplication.Run(args, Configure);
    }
}
