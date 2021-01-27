using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace GameTester
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Texture2D background;

        private int windowHeight = 512, windowWidth = 512;

        public TiledMap tiledMap;
        private TiledMapRenderer tiledMapRenderer;

        public KeyboardState keyboardState, previousKeyBoardState;

        public Player player;

        Camera camera;

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

            camera = new Camera(GraphicsDevice.Viewport);
            player = new Player(new Vector2(256, 400), "Default", Content);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            tiledMap = Content.Load<TiledMap>(@"Map\FinalMap");
            tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, tiledMap);
            background = Content.Load<Texture2D>("background_blocks");
        }

        protected override void Update(GameTime gameTime)
        {
            previousKeyBoardState = keyboardState;
            keyboardState = Keyboard.GetState();

            player.Update(gameTime, keyboardState);
            tiledMapRenderer.Update(gameTime);
            camera.Update(player, background.Width, background.Height);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: camera.transform);

            _spriteBatch.Draw(background, Vector2.Zero, Color.White);
            player.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
