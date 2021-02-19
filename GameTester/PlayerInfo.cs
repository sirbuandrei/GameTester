using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;

namespace GameTester
{
    [Serializable]
    public class PlayerInfo
    {
        public Vector positionToSend { get; set; }
        public int ID { get; set; }
        public Vector velocityVector { get; set; }

        // Dictionary<string, Animation> animationDictionary;
        // public AnimationManager animationManager;

        public PlayerInfo()
        {
            ID = 0;
        }
    }
}
