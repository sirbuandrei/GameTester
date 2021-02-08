using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace GameTester
{
    class NewMap
    {
        public List<NewLayer> layers;
        public Tileset tileset;
        public int _width, _height;
        public int _tileWidth, _tileHeight;

        private NewMap()
        {
            layers = new List<NewLayer>();
            _width = _height = 0;
            _tileWidth = _tileHeight = 0;
        }

        public static NewMap Load(string filename)
        {
            NewMap map = new NewMap();

            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            // <map>
            XmlNode root = doc.SelectSingleNode("map");
            map._width = int.Parse(root.Attributes["width"].InnerText);
            map._height = int.Parse(root.Attributes["height"].InnerText);
            map._tileWidth = int.Parse(root.Attributes["tilewidth"].InnerText);
            map._tileHeight = int.Parse(root.Attributes["tileheight"].InnerText);
            // </map>

            // <tileset>
            XmlNode tilesetInfo = root.SelectSingleNode("tileset");
            string tilesetPath = Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar + tilesetInfo.Attributes["source"].InnerText;
            map.tileset = new Tileset(tilesetPath);
            // </tileset>

            // <layer>
            foreach (XmlNode layerNode in root.SelectNodes("layer"))
            {
                NewLayer layer = new NewLayer(
                    layerNode.Attributes["name"].InnerText,
                    int.Parse(layerNode.Attributes["width"].InnerText),
                    int.Parse(layerNode.Attributes["height"].InnerText),
                    layerNode.SelectSingleNode("data").InnerText
                );

                map.layers.Add(layer);
            }
            // </layer>

            return map;
        }

        public void Init(GraphicsDevice graphics)
        {
            tileset.Load(graphics);
        }

        public List<Polygon> getNearbyPolygons(Vector pos)
        {
            Vector posToMap = PixelToMapCoord(pos);
            List<Polygon> collisions = new List<Polygon>();
            int search_area = 3;

            int from_X = (int) Math.Max(0, posToMap.X - search_area);
            int from_Y = (int) Math.Max(0, posToMap.Y - search_area);

            int to_X = (int) Math.Min(_width - 1, posToMap.X + search_area);
            int to_Y = (int) Math.Min(_height - 1, posToMap.Y + search_area);

            for(int i = from_Y; i < to_Y; i++)
            {
                for(int j = from_X; j < to_X; j++)
                {
                    foreach(NewLayer layer in layers)
                    {
                        if (layer.data[i, j] != -1)
                        {
                            foreach (Polygon collision in tileset.tiles[layer.data[i, j]].collisions)
                            {
                                Polygon p = collision.copy();

                                p.Offset(MapCoordToPixel(new Vector(j, i)));
                                p.BuildEdges();

                                collisions.Add(p);
                            }
                        }
                    }
                }
            }

            return collisions;
        }

        public Vector PixelToMapCoord(Vector coords)
        {
            return new Vector((int) (coords.X / _tileWidth), (int) (coords.Y / _tileHeight));
        }

        public Vector MapCoordToPixel(Vector coords)
        {
            return new Vector(coords.X * _tileWidth, coords.Y * _tileHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (NewLayer layer in layers)
                layer.Draw(spriteBatch, tileset);
        }
    }
}
