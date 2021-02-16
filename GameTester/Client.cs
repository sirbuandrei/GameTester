using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using ProtoBuf;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            byte[] end_byte = new byte[1];
            end_byte[0] = (byte) '!';

            stream.Write(player, 0, player.Length);
            stream.Write(end_byte, 0, 1);
        }

        public byte[] GetData()
        {
            Stream stream = client.GetStream();

            List<byte> byte_list = new List<byte>();
            byte[] buffer = new byte[1024];
            int index_of_33 = -1;

            while (index_of_33 == -1)
            {
                int bytesRead = stream.Read(buffer, 0, 1024);

                foreach (byte b in buffer)
                    byte_list.Add(b);
                
                index_of_33 = byte_list.IndexOf(33);
            }
            byte_list.RemoveRange(index_of_33, byte_list.Count - index_of_33);

            Console.Write("byte_list = \n[ ");
            foreach (byte b in byte_list)
            {
                Console.Write("{0}, ", b);
            }
            Console.WriteLine(" ]");

            return byte_list.ToArray();
        }

        public void SendPlayer(PlayerManager pm)
        {
            Stream stream = client.GetStream();
            string jsonString = "new_player:" + JsonSerializer.Serialize(pm) + "!";
            
            Console.WriteLine(jsonString);

            stream.Write(Encoding.ASCII.GetBytes(jsonString), 0, jsonString.Length);
        }

        public void UpdatePlayer(PlayerManager pm)
        {
            Stream stream = client.GetStream();
            string jsonString = "update_player:" + JsonSerializer.Serialize(pm) + "!";

            stream.Write(Encoding.ASCII.GetBytes(jsonString), 0, jsonString.Length);
        }
    }
}
