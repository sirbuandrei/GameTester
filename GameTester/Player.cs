using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameTester
{
    public class Player
    {
        public float velocity = 1f;
        public Vector2 position;
        Dictionary<string, Animation> animationDictionary;
        public AnimationManager animationManager;

        public Player(Vector2 position, string characterType, ContentManager Content)
        {
            this.position = position;
            animationDictionary = new Dictionary<string, Animation>()
            {
                {"WalkUp", new Animation(Content.Load<Texture2D>(@"Player\" + characterType + @"\WalkUp"), 3)},
                {"WalkDown", new Animation(Content.Load<Texture2D>(@"Player\" + characterType + @"\WalkDown"), 3)},
                {"WalkRight", new Animation(Content.Load<Texture2D>(@"Player\" + characterType + @"\WalkRight"), 3)},
                {"WalkLeft", new Animation(Content.Load<Texture2D>(@"Player\" + characterType + @"\WalkLeft"), 3)},
            };
            animationManager = new AnimationManager(animationDictionary.First().Value);
        }

        public void Move(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.W))
            {
                animationManager.Play(animationDictionary["WalkUp"]);
                position.Y -= velocity;
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                animationManager.Play(animationDictionary["WalkDown"]);
                position.Y += velocity;
            }
            else if (keyboardState.IsKeyDown(Keys.A))
            {
                animationManager.Play(animationDictionary["WalkLeft"]);
                position.X -= velocity;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                animationManager.Play(animationDictionary["WalkRight"]);
                position.X += velocity;
            }
            else
                animationManager.Stop();
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            Move(keyboardState);
            animationManager.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animationManager.Draw(spriteBatch, position);
        }
    }
}
