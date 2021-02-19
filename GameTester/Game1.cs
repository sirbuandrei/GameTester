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
using GPNetworkClient;
using GPNetworkMessage;
using System.Threading;

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
        private Dictionary<int, Player> allPlayers;

        NewMap nMap;
        NewCamera nCamera;

        UDPClient client;

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

            allPlayers = new Dictionary<int, Player>();
            player = new Player(nMap.playerStart, "Conjurer", Content);

            //camera = new Camera(GraphicsDevice.Viewport);
            
            nCamera = new NewCamera(GraphicsDevice.Viewport);
            nCamera.Limits = new Rectangle(0, 0, nMap._width * nMap.tileset._tileWidth, nMap._height * nMap.tileset._tileHeight);

            client = new UDPClient();
            if (!client.Connect("79.114.16.172", 5555))
            {
                Console.WriteLine("Cannot connect to server!");

                // Exit();
            }

            client.SendMessage(MessageType.ANY, player.toPlayerInfo());
            player.ID = client.ClientID;

            allPlayers.Add(player.ID, player);

            Thread.Sleep(100);
            client.Messages.Dequeue();

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

            Vector Translation = new Vector(0, 0);
            //nMap.drawOnTop.Clear();

            player.Translate(Translation);
            nCamera.Position = new Vector2(player.position.X - (GraphicsDevice.Viewport.Width / 2 / nCamera.Zoom),
                                           player.position.Y - (GraphicsDevice.Viewport.Height / 2 / nCamera.Zoom));

            foreach (Tuple<Polygon, Vector3> tup in nMap.getNearbyPolygons(new Vector(player.position.X, player.position.Y)))
            {
                Polygon p = tup.Item1;
                CollisionDetection.PolygonCollisionResult result = CollisionDetection.PolygonCollision(player.hitbox, p, player.velocityVector);

                if (result.WillIntersect)
                    if (!p.drawOrderGuide)
                        Translation += result.MinimumTranslationVector;
            }

            /// UPDATE PLAYERS
            player.positionToSend = new Vector(player.position.X, player.position.Y);
            client.SendMessage(MessageType.ANY, player.toPlayerInfo());

            string message;

            while (client.Messages.Count != 0)
            {
                message = client.Messages.Dequeue().Message;

                try
                {
                    PlayerInfo info = JsonSerializer.Deserialize<PlayerInfo>(message);

                    allPlayers[info.ID] = Player.fromInfo(info, Content);
                }
                catch (Exception e) { ; }
            }

            foreach (Player o_player in allPlayers.Values)
            {
                foreach (Tuple<Polygon, Vector3> tup in nMap.getNearbyPolygons(new Vector(o_player.position.X, o_player.position.Y)))
                {
                    Polygon p = tup.Item1;

                    if (p.drawOrderGuide)
                    {
                        CollisionDetection.PolygonCollisionResult result = CollisionDetection.PolygonCollision(o_player.orderHitbox, p, o_player.velocityVector);

                        if (result.Intersect)
                            nMap.drawOnTop.data[(int)tup.Item2.X, (int)tup.Item2.Y] = -1;
                        else
                            nMap.drawOnTop.data[(int)tup.Item2.X, (int)tup.Item2.Y] = (int)tup.Item2.Z;
                    }
                }
            }

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

            nMap.Draw(_spriteBatch);
            foreach (Player p in allPlayers.Values)
                p.Draw(_spriteBatch);
            nMap.DrawTop(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
