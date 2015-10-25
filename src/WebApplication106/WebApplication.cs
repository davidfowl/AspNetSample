using System;
using System.Diagnostics;
using System.IO;
using System.Net.Libuv;
using System.Text;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace WebApplication106
{
    public static class WebApplication
    {
        public static void Run(string[] args, Action<ApplicationBuilder> configure)
        {
            // Build middleware pipeline
            var appBuilder = new ApplicationBuilder();
            configure(appBuilder);
            var appDelegate = appBuilder.Build();

            var loop = new UVLoop();
            var listener = new TcpListener("0.0.0.0", 5000, loop);
            listener.ConnectionAccepted += (Tcp connection) =>
            {
                connection.ReadCompleted += (ByteSpan data) =>
                {
                    unsafe
                    {
                        var requestString = Encoding.UTF8.GetString(data.UnsafeBuffer, data.Length);
                        Console.WriteLine("*REQUEST:\n {0}", requestString.ToString());
                    }

                    var context = new LibuvHttpContext();

                    // BAD: Single threaded for now, we're ignoring the result of the task
                    // because we know it's synchronous
                    var task = appDelegate(context);

                    context.Response.ContentType = context.Response.ContentType ?? "text/plain";

                    Debug.Assert(task.IsCompleted, "Async not supported yet!");

                    // Flush and dispose (keep alive not supported)
                    var preamble = Encoding.UTF8.GetBytes($"HTTP/1.1 {context.Response.StatusCode} {GetStatusText(context.Response.StatusCode)}\r\nContent-Length:{context.Response.ContentLength}\r\nContent-Type:{context.Response.ContentType}\r\nConnection:Close\r\n\r\n");

                    ArraySegment<byte> bodyBuffer;
                    ((MemoryStream)context.Response.Body).TryGetBuffer(out bodyBuffer);

                    var entireBody = new byte[preamble.Length + bodyBuffer.Count];
                    Buffer.BlockCopy(preamble, 0, entireBody, 0, preamble.Length);
                    Buffer.BlockCopy(bodyBuffer.Array, bodyBuffer.Offset, entireBody, preamble.Length, bodyBuffer.Count);

                    connection.TryWrite(entireBody);

                    connection.Dispose();
                };

                connection.ReadStart();
            };

            listener.Listen();
            loop.Run();
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
