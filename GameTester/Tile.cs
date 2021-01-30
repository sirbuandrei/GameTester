using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTester
{
    public class Tile
    {
        private Texture2D texture;
        public Vector2 position;
        private int tileWitdh, tileHeight;
        public Polygon polygon;
        public int iD;

        public Tile(string iD, int x, int y, ContentManager Content)
        {
            this.iD = Int32.Parse(iD);
            this.tileWitdh = 16;
            this.tileHeight = 16;
            texture = Content.Load<Texture2D>(@"FullSetOfTiles\Tiles" + iD);
            position = new Vector2(x * tileWitdh, y * tileHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
