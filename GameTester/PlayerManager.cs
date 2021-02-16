using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;

namespace GameTester
{
    [Serializable]
    public class PlayerManager
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int ID { get; set; }

        public PlayerManager()
        {
            ID = 0;
        }

        override public string ToString()
        {
            return String.Format("X: {0}, Y: {1}, ID: {2}", X, Y, ID);
        }
    }
}
