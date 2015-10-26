using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Builder
{
    public class ApplicationBuilder
    {
        private readonly List<Func<RequestDelegate, RequestDelegate>> _components = new List<Func<RequestDelegate, RequestDelegate>>();

        public ApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        public ApplicationBuilder Run(RequestDelegate handler)
        {
            Use(next => context => handler(context));
            return this;
        }

        public RequestDelegate Build()
        {
            RequestDelegate app = context =>
            {
                context.Response.StatusCode = 404;

                return Task.CompletedTask;
            };

            for (int i = _components.Count - 1; i >= 0; i--)
            {
                app = _components[i](app);
            }

            return app;
        }
    }
}
