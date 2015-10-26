using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Http
{
    public class LibuvHttpResponse : HttpResponse
    {
        private long? _contentLength;

        public LibuvHttpResponse(HttpContext context)
        {
            Body = new LibuvStream();
            HttpContext = context;
        }

        public override Stream Body
        {
            get; set;
        }

        public override long? ContentLength
        {
            get
            {
                return _contentLength ?? Body.Length;
            }
            set
            {
                _contentLength = value;
            }
        }


        public override string ContentType { get; set; }

        public override IResponseCookies Cookies
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool HasStarted
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IHeaderDictionary Headers
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override HttpContext HttpContext { get; }

        public override int StatusCode { get; set; }

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void OnStarting(Func<object, Task> callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void Redirect(string location, bool permanent)
        {
            throw new NotImplementedException();
        }
    }
}