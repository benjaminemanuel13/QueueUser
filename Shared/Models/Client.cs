using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Client
    {
        private readonly TaskCompletionSource<object> _task;
        private WebSocket _socket { get; set; }

        public Client(TaskCompletionSource<object> task, WebSocket socket)
        { 
            _task = task;
            _socket = socket;
        }

        public async void SendMessage(string message)
        {
            byte[] bytes = ASCIIEncoding.UTF8.GetBytes(message);
            await _socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async void StartListening()
        {
            while (true)
            {
                try
                {
                    byte[] bytes = new byte[5];

                    await _socket.ReceiveAsync(bytes, CancellationToken.None);

                    string message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                    if (message == "Bus1")
                    {
                        //EndConnection();
                    }
                    else if (message == "GotIt")
                    {
                        bytes = new byte[6];

                        await _socket.ReceiveAsync(bytes, CancellationToken.None);

                        message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                        int length = int.Parse(message);

                        if (length > 0)
                        {
                            bytes = new byte[length];

                            await _socket.ReceiveAsync(bytes, CancellationToken.None);

                            string recieve = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                            byte[] data = Convert.FromBase64String(recieve);

                            //RecieveData(data);
                        }
                    }
                    else //Assuming Heartbeat
                    {
                        //counter = 0;
                    }
                }
                catch (Exception ex)
                {
                    //Disconnected
                    //EndConnection();

                    break;
                }
            }
        }
    }
}
