using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using ProtoBuf;

namespace GameTester
{
    public class Client
    {
        TcpClient client = new TcpClient();
        
        public Client(string ip, int port)
        {
            client.Connect(ip, port);
            client.NoDelay = true;
        }

        public void SendData(byte[] player)
        {
            Stream stream = client.GetStream();
            stream.Write(player, 0, player.Length);
        }

        public void GetData()
        {
            Stream stream = client.GetStream();

            Console.WriteLine(client.Available.ToString());

            s

/*            //byte[] buffer = new byte[2];

            int bytesRead = 0;

            while(bytesRead == 0)
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                Console.WriteLine(bytesRead);
            }

            Console.WriteLine(bytesRead);*/
        }
    }
}
