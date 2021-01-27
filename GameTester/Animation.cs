﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTester
{
    public class Animation
    {
        public Texture2D texture;
        public int frameHeight, frameWidth, currentFrame, frameCount;
        public float speed;

        public Animation(Texture2D texture, int frameCount)
        {
            this.texture = texture;
            this.frameCount = frameCount;
            this.frameHeight = texture.Height;
            this.frameWidth = texture.Width / frameCount;
            this.speed = 0.12f;
        }

    }
}
