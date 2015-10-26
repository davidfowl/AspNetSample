using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Libuv;
using System.Text;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Hosting
{
    public static class WebApplication
    {
        public static void Run(string[] args, Action<ApplicationBuilder> config)
        {
            // Build middleware pipeline
            var appBuilder = new ApplicationBuilder();
            config(appBuilder);
            var appDelegate = appBuilder.Build();

            var loop = new UVLoop();
            var listener = new TcpListener("0.0.0.0", 5000, loop);
            listener.ConnectionAccepted += (Tcp connection) =>
            {
                connection.ReadCompleted += (ByteSpan data) =>
                {
                    //unsafe
                    //{
                    //    var requestString = Encoding.UTF8.GetString(data.UnsafeBuffer, data.Length);
                    //    Console.WriteLine("*REQUEST:\n {0}", requestString.ToString());
                    //}

                    var context = new LibuvHttpContext();

                    // BAD: Single threaded for now, we're ignoring the result of the task
                    // because we know it's synchronous
                    appDelegate(context);

                    context.Response.ContentType = context.Response.ContentType ?? "text/plain";

                    if (context.Response.StatusCode == 0)
                    {
                        context.Response.StatusCode = 200;
                    }

                    // Flush and dispose (keep alive not supported)
                    var contentLength = (int)context.Response.ContentLength;
                    var responseBuffer = new byte[1024];
                    var written = 0;

                    Append(responseBuffer, ref written, "HTTP/1.1 ");
                    Append(responseBuffer, ref written, context.Response.StatusCode);
                    Append(responseBuffer, ref written, " ");
                    Append(responseBuffer, ref written, GetStatusText(context.Response.StatusCode));
                    Append(responseBuffer, ref written, "\r\n");
                    Append(responseBuffer, ref written, "Content-Length:");
                    Append(responseBuffer, ref written, contentLength);
                    Append(responseBuffer, ref written, "\r\n");
                    Append(responseBuffer, ref written, "Content-Type:");
                    Append(responseBuffer, ref written, context.Response.ContentType);
                    Append(responseBuffer, ref written, "\r\n");
                    Append(responseBuffer, ref written, "Connection:Close\r\n\r\n");
                    Append(responseBuffer, ref written, ((LibuvStream)context.Response.Body).GetBuffer());

                    connection.TryWrite(responseBuffer, written);
                    connection.Dispose();
                };

                connection.ReadStart();
            };

            listener.Listen();
            // Console.WriteLine("Listening on ::5000");
            loop.Run();
        }

        private static void Append(byte[] responseBuffer, ref int written, ArraySegment<byte> buffer)
        {
            if (written + buffer.Count >= responseBuffer.Length)
            {
                Array.Resize(ref responseBuffer, responseBuffer.Length * 2);
            }

            Array.Copy(buffer.Array, buffer.Offset, responseBuffer, written, buffer.Count);

            written += buffer.Count;
        }

        private static void Append(byte[] responseBuffer, ref int written, byte[] buffer)
        {
            Append(responseBuffer, ref written, new ArraySegment<byte>(buffer));
        }

        private static void Append(byte[] responseBuffer, ref int written, string data)
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            Append(responseBuffer, ref written, buffer);
        }

        private static void Append(byte[] responseBuffer, ref int written, int value)
        {
            Append(responseBuffer, ref written, itoa(value));
        }

        private static string itoa(int value)
        {
            char[] buffer = new char[10];
            int at = buffer.Length - 1;
            while (value > 0)
            {
                var d = value % 10;
                value /= 10;

                buffer[at--] = (char)(d + '0');
            }

            at++;

            return new string(buffer, at, buffer.Length - at);
        }

        private static string GetStatusText(int statusCode)
        {
            if (statusCode == 200)
            {
                return "OK";
            }

            if (statusCode == 404)
            {
                return "Not Found";
            }

            if (statusCode == 500)
            {
                return "Internal Server Error";
            }

            return null;
        }
    }
}
