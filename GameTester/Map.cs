using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameTester
{
    public class Map
    {
        public class Collision
        {
            public int iD;
            public Polygon collidablePolygon;

            public Collision(int iD, List<Vector> points)
            {
                this.iD = iD;
                collidablePolygon = new Polygon();

                foreach(Vector point in points)
                    collidablePolygon.Points.Add(point);
               collidablePolygon.BuildEdges();
            }
        }

        public List<Layer> layers;
        Layer layer;
        public Vector2 startingPoint;
        XmlDocument xmlDocument;
        public int Width, Height;
        char[] delimiterChars = { ',', '\n', '\r' };
        public ContentManager Content;
        public List<Collision> collisions;

        public Map(string xmlFile, string collisionFile, ContentManager Content)
        {
            this.Content = Content;
            layers = new List<Layer>();
            collisions = new List<Collision>();
            startingPoint = new Vector2(16*8, 16*19);
            xmlDocument = new XmlDocument();
            XmlRead(xmlFile);
            XmlReadCollision(collisionFile);
        }

        private void XmlRead(string fileName)
        {
            xmlDocument.Load(fileName);

            foreach(XmlNode node in xmlDocument.DocumentElement.ChildNodes)
            {
                if(node.Name == "layer" && node.InnerText != "")
                {
                    node.InnerText = node.InnerText.Replace("\n", "");
                    node.InnerText = node.InnerText.Replace("\r", "");
                    string[] tileIDs = node.InnerText.Split(delimiterChars);
                    layer = new Layer(tileIDs, Content);
                    layers.Add(layer);
                }
            }

        }

        private void XmlReadCollision(string collisionFile)
        {
            xmlDocument.Load(collisionFile);

            foreach(XmlNode node in xmlDocument.DocumentElement.ChildNodes)
            {
                if(node.Name == "tile")
                {
                    List<Vector> collisionPoints = new List<Vector>();
                    int iD = Int32.Parse(node.Attributes["id"].Value);

                    foreach (XmlNode objGroup in node.ChildNodes)
                    {
                        XmlNode childNode = objGroup.ChildNodes[0];
                        if (childNode.Name == "object")
                        {
                            float xStart = float.Parse(childNode.Attributes["x"].Value);
                            float yStart = float.Parse(childNode.Attributes["y"].Value);

                            if (!childNode.HasChildNodes)
                            {
                                float width = float.Parse(childNode.Attributes["width"].Value);
                                float height = float.Parse(childNode.Attributes["height"].Value);

                                foreach(Tile tile in layers[layers.Count - 1].tiles)
                                {
                                    if(tile.iD == iD)
                                    {
                                        collisionPoints.Add(new Vector(xStart + tile.position.X, yStart + tile.position.Y));
                                        collisionPoints.Add(new Vector(xStart + tile.position.X + width, yStart + tile.position.Y));
                                        collisionPoints.Add(new Vector(xStart + tile.position.X + width, yStart + tile.position.Y + height));
                                        collisionPoints.Add(new Vector(xStart + tile.position.X, yStart + tile.position.Y + height));
                                        Collision collision = new Collision(iD, collisionPoints);
                                        collisions.Add(collision);
                                    }
                                }
                            }
                            else
                            {
                                List<Vector> pointsToAdd = new List<Vector>();

                                string stringToParse = childNode.ChildNodes[0].Attributes["points"].Value;
                                string[] points = stringToParse.Split(' ');
                                foreach(var point in points)
                                {
                                    string[] finalPoints = point.Split(',');
                                    pointsToAdd.Add(new Vector(float.Parse(finalPoints[0]), float.Parse(finalPoints[1])));
                                }

                                foreach(Tile tile in layers[layers.Count - 1].tiles)
                                {
                                    if(tile.iD == iD)
                                    {
                                        foreach(Vector pointToAdd in pointsToAdd)
                                        {
                                            collisionPoints.Add(new Vector(tile.position.X + xStart + pointToAdd.X, tile.position.Y + yStart + pointToAdd.Y));
                                        }
                                        Collision collision = new Collision(iD, collisionPoints);
                                        collisions.Add(collision);
                                    }
                                }

                            }

                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Layer layer in layers)
                layer.Draw(spriteBatch);
        }

    }
}
