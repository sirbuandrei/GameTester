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

            map = new Map(@"..\..\..\MapData\Map2.tmx", @"..\..\..\MapData\GrassTileset.tsx", Content);

/*            Console.WriteLine(map.collisions.Count);

            foreach (Map.Collision collision in map.collisions)
                Console.WriteLine(String.Format("{0}: ", collision.iD) + collision.collidablePolygon.ToString() + "\n\n");*/
            
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

            Vector Translation = new Vector(0, 0);

            foreach(Map.Collision collision in map.collisions)
            {
                CollisionDetection.PolygonCollisionResult result = CollisionDetection.PolygonCollision(player.hitbox, collision.collidablePolygon, player.velocityVector);

                if (result.WillIntersect)
                    Translation = result.MinimumTranslationVector;
            }

            player.Translate(Translation);

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
/*
            int W = (int) (player.hitbox.Points[1].X - player.hitbox.Points[0].X);
            int H = (int) (player.hitbox.Points[2].Y - player.hitbox.Points[0].Y);

            Texture2D rect = new Texture2D(_graphics.GraphicsDevice, W, H);

            Color[] data = new Color[W * H];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Chocolate;
            rect.SetData(data);

            _spriteBatch.Draw(rect, new Vector2(player.hitbox.Points[0].X, player.hitbox.Points[0].Y), Color.White);
*/
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
