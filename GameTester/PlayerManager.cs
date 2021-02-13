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
        [ProtoMember(1)]
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

    }
}
