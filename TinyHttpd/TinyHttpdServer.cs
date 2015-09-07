using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace TinyHttpd
{

    //Reference: https://dev.windows.com/en-us/samples
    //           https://www.hackster.io/windowsiot/blinky-webserver-sample

    public sealed class TinyHttpdServer : IDisposable
    {
       
        private const uint BufferSize = 131072;
        private int port = 5978;
        private readonly StreamSocketListener listener;
        public async void Start()
        {
            await listener.BindServiceNameAsync(port.ToString());
        }


        // private AppServiceConnection appServiceConnection;
        public TinyHttpdServer(int serverPort)
        {
            listener = new StreamSocketListener();
            port = serverPort;
            listener.ConnectionReceived += (s, e) =>
                            ProcessRequestAsync(e.Socket);
        }

        private async void ProcessRequestAsync(StreamSocket socket)
        {
            // this works for text only
            StringBuilder request = new StringBuilder();
            using (IInputStream input = socket.InputStream)
            {
                byte[] data = new byte[BufferSize];
                IBuffer buffer = data.AsBuffer();
                uint dataRead = BufferSize;
                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }

            using (IOutputStream output = socket.OutputStream)
            {
                string requestMethod = request.ToString().Split('\n')[0];
                string[] requestParts = requestMethod.Split(' ');

                if (requestParts[0] == "GET")
                    await WriteResponseAsync(requestParts[1], output);
                else
                    throw new InvalidDataException("HTTP method not supported: "
                                                   + requestParts[0]);
            }
        }

        private async Task WriteResponseAsync(string request, IOutputStream os)
        {
            string file = @"Assets\html" + request.Replace("\\", "/");


            if (request == "/")
            {
                file = @"Assets\html\index.html";
            }
            else if (!System.IO.File.Exists(file))
            {
                file = @"Assets\html\404.html";
            }
            using (Stream resp = os.AsStreamForWrite())
            {
                var contentType = "text/html";
                if (System.IO.Path.GetExtension(file).ToLower() == ".jpg" ||
                    System.IO.Path.GetExtension(file).ToLower() == ".png" ||
                    System.IO.Path.GetExtension(file).ToLower() == ".jpeg")
                {
                    contentType = "image/*";
                }
                byte[] bodyArray = File.ReadAllBytes(file);
                MemoryStream stream = new MemoryStream(bodyArray);
                string header = String.Format("HTTP/1.1 200 OK\r\n" +
                                              "Content-Length: {0}\r\n" +
                                               "content-type: {1}\r\n" +
                                              "Connection: close\r\n\r\n",
                    stream.Length, contentType);
                byte[] headerArray = Encoding.UTF8.GetBytes(header);
                await resp.WriteAsync(headerArray, 0, headerArray.Length);
                await stream.CopyToAsync(resp);
                await resp.FlushAsync();
            }
            
        }


        public void Dispose()
        {
            listener.Dispose();
        }
    }
}
