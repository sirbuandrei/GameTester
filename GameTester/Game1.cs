using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// 86.124.142.106
/// </summary>

namespace GameTester
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Texture2D background;

        public static int windowHeight = 512, windowWidth = 512;

        public KeyboardState keyboardState, previousKeyBoardState;
        public Player player;

        private double time = 0;

        private PlayerManager playerManager;

        NewMap nMap;
        Camera camera;
        Map map;
        NewCamera nCamera;
        Client client;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = windowWidth;
            _graphics.PreferredBackBufferHeight = windowHeight;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            nMap = NewMap.Load(@"..\..\..\MapData\Map2.1.tmx");
            //nMap = NewMap.Load(@"..\..\..\MapData\Map.tmx");
            // map = new Map(@"..\..\..\MapData\Map2.tmx", @"..\..\..\MapData\GrassTileset.tsx", Content);

            //player = new Player(new Vector2(16 * 8, 16 * 19), "Conjurer", Content);
            playerManager = new PlayerManager();
            player = new Player(nMap.playerStart, "Conjurer", Content);
            camera = new Camera(GraphicsDevice.Viewport);
            nCamera = new NewCamera(GraphicsDevice.Viewport);
            Console.WriteLine(nMap.tileset._tileHeight);
            nCamera.Limits = new Rectangle(0, 0, nMap._width * nMap.tileset._tileWidth, nMap._height * nMap.tileset._tileHeight);

            client = new Client("79.114.16.172", 5555);
            client.SendPlayer(player.toPlayerManager());

            player.ID = (Int32) BitConverter.ToInt32(client.GetData(), 0);
            Console.WriteLine("Player ID: {0}", player.ID);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("background_stars");

            nMap.Init(GraphicsDevice);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            previousKeyBoardState = keyboardState;
            keyboardState = Keyboard.GetState();

            player.Update(gameTime, keyboardState);

            /*Vector Translation = new Vector(0, 0);

            foreach(Map.Collision collision in map.collisions)
            {
                CollisionDetection.PolygonCollisionResult result = CollisionDetection.PolygonCollision(player.hitbox, collision.collidablePolygon, player.velocityVector);

                if (result.WillIntersect)
                    Translation = result.MinimumTranslationVector;
            }

            player.Translate(Translation);*/

            Vector Translation = new Vector(0, 0);
            //nMap.drawOnTop.Clear();

            foreach (Tuple<Polygon, Vector3> tup in nMap.getNearbyPolygons(new Vector(player.position.X, player.position.Y)))
            {
                Polygon p = tup.Item1;
                CollisionDetection.PolygonCollisionResult result = CollisionDetection.PolygonCollision(player.hitbox, p, player.velocityVector);

                if (result.WillIntersect)
                    if (!p.drawOrderGuide)
                        Translation += result.MinimumTranslationVector;
            }

            foreach (Tuple<Polygon, Vector3> tup in nMap.getNearbyPolygons(new Vector(player.position.X, player.position.Y)))
            {
                Polygon p = tup.Item1;

                //Console.WriteLine(p.drawOrderGuide);
                if (p.drawOrderGuide)
                {
                    CollisionDetection.PolygonCollisionResult result = CollisionDetection.PolygonCollision(player.orderHitbox, p, player.velocityVector);

                    if (result.Intersect)
                        nMap.drawOnTop.data[(int)tup.Item2.X, (int)tup.Item2.Y] = -1;
                    else
                        nMap.drawOnTop.data[(int)tup.Item2.X, (int)tup.Item2.Y] = (int)tup.Item2.Z;
                }
            }

            player.Translate(Translation);
            nCamera.Position = new Vector2(player.position.X - (GraphicsDevice.Viewport.Width / 2 / nCamera.Zoom),
                                           player.position.Y - (GraphicsDevice.Viewport.Height / 2 / nCamera.Zoom));

            if (gameTime.TotalGameTime.TotalSeconds - time > 5)
            {
                /// UPDATE PLAYERS
                player.positionToSend = new Vector(player.position.X, player.position.Y);
                Console.WriteLine("3 sec elapsed, new player position {0}, {1}", player.positionToSend.X, player.positionToSend.Y);

                client.UpdatePlayer(player.toPlayerManager());

                /// GET PLAYERS
                client.SendData(Encoding.ASCII.GetBytes("get_players!"));

                List<PlayerManager> pmList = new List<PlayerManager>();
                string data = Encoding.ASCII.GetString(client.GetData());

                if(data.Length != 0)
                    pmList = JsonSerializer.Deserialize<List<PlayerManager>>(data);

                foreach (PlayerManager pManager in pmList)
                    Console.WriteLine(pManager.ToString());
                
                time = gameTime.TotalGameTime.TotalSeconds;
            }

            // camera.Update(player, 35*16, 35*16);
            // camera.Update(player, nMap._width, nMap._height);
            // camera.Follow(player, nMap._width, nMap._height);

            base.Update(gameTime);
        }

        public void DrawLine(Vector2 p1, Vector2 p2)
        {
            Texture2D primal_sqaure = new Texture2D(GraphicsDevice, 5, 5);

            Color[] color_data = new Color[5 * 5];
            for (int i = 0; i < color_data.Length; i++) color_data[i] = Color.Red;
            primal_sqaure.SetData(color_data);

            float angle = (float)Math.Atan2(p1.Y - p2.Y, p1.X - p2.X);
            float distance = Vector2.Distance(p1, p2);

            _spriteBatch.Draw(primal_sqaure, new Rectangle((int)p2.X, (int)p2.Y, (int)distance, 1), null, Color.White, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: nCamera.ViewMatrix);

/*            _spriteBatch.Draw(background, Vector2.Zero, Color.White);
            _spriteBatch.Draw(background, new Vector2(0, 512), Color.White);
            _spriteBatch.Draw(background, new Vector2(0, 512 * 2), Color.White);
            _spriteBatch.Draw(background, new Vector2(512, 0), Color.White);
            _spriteBatch.Draw(background, new Vector2(512, 512), Color.White);
            _spriteBatch.Draw(background, new Vector2(512, 512 * 2), Color.White);
            _spriteBatch.Draw(background, new Vector2(512 * 2, 0), Color.White);
            _spriteBatch.Draw(background, new Vector2(512 * 2, 512), Color.White);
            _spriteBatch.Draw(background, new Vector2(512 * 2, 512 * 2), Color.White);*/

            nMap.Draw(_spriteBatch);
            player.Draw(_spriteBatch);
            nMap.DrawTop(_spriteBatch);

            /*foreach (Tuple<Polygon, Vector3> tup in nMap.getNearbyPolygons(new Vector(player.position.X, player.position.Y)))
            {
                Polygon p = tup.Item1;

                for (int i = 1; i < p.Points.Count; i++)
                    DrawLine(new Vector2(p.Points[i - 1].X, p.Points[i - 1].Y), new Vector2(p.Points[i].X, p.Points[i].Y));
            }*/

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public byte[] addByteArrays(byte[] array1, byte[] array2)
        {
            byte[] newArray = new byte[array1.Length + array2.Length];
            array1.CopyTo(newArray, 0);
            array2.CopyTo(newArray, array1.Length);
            return newArray;
        }
    }
}
