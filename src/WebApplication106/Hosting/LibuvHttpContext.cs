using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Http.Features;

namespace Microsoft.AspNet.Http
{
    public class LibuvHttpContext : HttpContext
    {
        public LibuvHttpContext()
        {
            Response = new LibuvHttpResponse(this);
        }

        public override IServiceProvider ApplicationServices
        {
            get; set;
        }

        public override AuthenticationManager Authentication
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override ConnectionInfo Connection
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IFeatureCollection Features
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IDictionary<object, object> Items
        {
            get; set;
        }

        public override HttpRequest Request
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override CancellationToken RequestAborted
        {
            get; set;
        }

        public override IServiceProvider RequestServices
        {
            get; set;
        }

        public override HttpResponse Response { get; }

        public override ISession Session
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override string TraceIdentifier
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override ClaimsPrincipal User
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override WebSocketManager WebSockets
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Abort()
        {
            throw new NotImplementedException();
        }
    }
}
