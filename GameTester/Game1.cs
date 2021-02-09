using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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

        NewMap nMap;
        Camera camera;
        Map map;
        NewCamera nCamera;

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
            // map = new Map(@"..\..\..\MapData\Map2.tmx", @"..\..\..\MapData\GrassTileset.tsx", Content);

            //player = new Player(new Vector2(16 * 8, 16 * 19), "Conjurer", Content);
            player = new Player(nMap.playerStart, "Conjurer", Content);
            camera = new Camera(GraphicsDevice.Viewport);
            nCamera = new NewCamera(GraphicsDevice.Viewport);
            nCamera.Limits = new Rectangle(0, 0, 35 * 32, 35 * 32);

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

            foreach (Polygon p in nMap.getNearbyPolygons(new Vector(player.position.X, player.position.Y)))
            {
                CollisionDetection.PolygonCollisionResult result = CollisionDetection.PolygonCollision(player.hitbox, p, player.velocityVector);

                if (result.WillIntersect)
                {
                    if (!p.drawOrderGuide)
                        Translation += result.MinimumTranslationVector;
                    /*else
                        nMap.drawOnTop.Add();*/
                }
            }

            player.Translate(Translation);
            nCamera.Position = new Vector2(player.position.X - (GraphicsDevice.Viewport.Width / 2 / nCamera.Zoom),
                                           player.position.Y - (GraphicsDevice.Viewport.Height / 2 / nCamera.Zoom));
            // camera.Update(player, 35*16, 35*16);
            //camera.Update(player, nMap._width, nMap._height);
            //camera.Follow(player, nMap._width, nMap._height);

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

            _spriteBatch.Draw(background, Vector2.Zero, Color.White);
            _spriteBatch.Draw(background, new Vector2(0, 512), Color.White);
            _spriteBatch.Draw(background, new Vector2(0, 512 * 2), Color.White);
            _spriteBatch.Draw(background, new Vector2(512, 0), Color.White);
            _spriteBatch.Draw(background, new Vector2(512, 512), Color.White);
            _spriteBatch.Draw(background, new Vector2(512, 512 * 2), Color.White);
            _spriteBatch.Draw(background, new Vector2(512 * 2, 0), Color.White);
            _spriteBatch.Draw(background, new Vector2(512 * 2, 512), Color.White);
            _spriteBatch.Draw(background, new Vector2(512 * 2, 512 * 2), Color.White);
            nMap.Draw(_spriteBatch);
            player.Draw(_spriteBatch);


            foreach (Polygon p in nMap.getNearbyPolygons(new Vector(player.position.X, player.position.Y)))
            {
                for (int i = 1; i < p.Points.Count; i++)
                    DrawLine(new Vector2(p.Points[i - 1].X, p.Points[i - 1].Y), new Vector2(p.Points[i].X, p.Points[i].Y));
            }


            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
