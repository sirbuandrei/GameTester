using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GameTester
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Texture2D background;

        private int windowHeight = 512, windowWidth = 512;

        public KeyboardState keyboardState, previousKeyBoardState;

        public Player player;

        Camera camera;

        Map map;

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

            map = new Map(@"C:\Users\andre\source\repos\GameTester\GameTester\MapData\Map2.tmx", @"C:\Users\andre\source\repos\GameTester\GameTester\MapData\GrassTileset.tsx", Content);

            Console.WriteLine(map.collisions.Count);

            //foreach (Map.Collision collision in map.collisions)
                //Console.WriteLine(collision.collidablePolygon.ToString());
            
            player = new Player(new Vector2(map.startingPoint.X, map.startingPoint.Y), "Conjurer", Content);
            camera = new Camera(GraphicsDevice.Viewport);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("background_blocks");
        }

        protected override void Update(GameTime gameTime)
        {
            previousKeyBoardState = keyboardState;
            keyboardState = Keyboard.GetState();

            player.Update(gameTime, keyboardState);
            camera.Update(player, 35*16, 35*16);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: camera.transform);

            map.Draw(_spriteBatch);
            //_spriteBatch.Draw(background, Vector2.Zero, Color.White);
            player.Draw(_spriteBatch);


            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
