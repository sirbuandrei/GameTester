using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTester
{
    public class Layer
    {
        public List<Tile> tiles;
        Tile tile;

        public Layer(string[] tileIDs, ContentManager Content)
        {
            tiles = new List<Tile>();
            for(int i = 0; i < tileIDs.Length; i ++)
            {
                if (tileIDs[i].ToString() != "" && tileIDs[i].ToString() != "0")
                {
                    tile = new Tile(tileIDs[i], i % 35, i / 35, Content);
                    tiles.Add(tile);
                }
            }

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tile tile in tiles)
                tile.Draw(spriteBatch);
        }
    }
}
