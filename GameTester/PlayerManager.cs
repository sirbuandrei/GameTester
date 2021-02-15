using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;

namespace GameTester
{
    [ProtoContract]
    class PlayerManager
    {
        [ProtoMember(1, OverwriteList = true)]
        public List<Player> players;

        public PlayerManager()
        {
            this.players = new List<Player>();
        }

        public static T ProtoDeserialize<T>(byte[] data) where T : class
        {
            if (null == data) return null;

            try
            {
                using (var stream = new MemoryStream(data))
                {
                    return Serializer.Deserialize<T>(stream);
                }
            }
            catch
            {
                // Log error
                throw;
            }
        }

        public void Print()
        {
            foreach (Player p in players)
            {
                Console.WriteLine("Player {0}: {1}, {2}", p.ID, p.position.X, p.position.Y);
            }
        }
    }
}
