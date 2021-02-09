using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTester
{
    public class Camera
    {
        public Matrix transform;
        Vector2 centre;
        Viewport viewport;

        public Camera(Viewport viewport)
        {
            this.viewport = viewport;
        }

        public void Update(Player player, int mapWidth, int mapHeight)
        {
            centre = player.position + new Vector2(player.animationManager.animation.frameWidth / 2, player.animationManager.animation.frameHeight / 2);

            if (centre.X + viewport.Width / 2 > mapWidth)
                centre.X = mapWidth - viewport.Width / 2;
            else if (centre.X - viewport.Width / 2 < 0)
                centre.X = viewport.Width / 2;

            if (centre.Y + viewport.Height / 2 > mapHeight)
                centre.Y = mapHeight - viewport.Height / 2;
            else if (centre.Y - viewport.Height / 2 < 0)
                centre.Y = viewport.Height / 2;

            transform = Matrix.CreateScale(1.5f, 1.5f, 0) * Matrix.CreateTranslation(new Vector3(-centre + new Vector2(viewport.Width / 2, viewport.Height / 2), 0.0f));
        }

        public void Follow(Player player, int mapWidth, int mapHeight)
        {
            float ZoomAmount = (float) 2.5;

            centre = player.position + new Vector2(player.animationManager.animation.frameWidth / 2, player.animationManager.animation.frameHeight / 2);

            if (centre.X + (viewport.Width / 2) > mapWidth * ZoomAmount)
                Console.WriteLine("dada");
            else Console.WriteLine("nunu");
                //centre.X = mapWidth - viewport.Width / 2;
            /*else if (centre.X - viewport.Width / 2 < 0)
                centre.X = viewport.Width / 2;*/

            /*if (centre.Y + viewport.Height / 2 > mapHeight)
                centre.Y = mapHeight - viewport.Height / 2;
            else if (centre.Y - viewport.Height / 2 < 0)
                centre.Y = viewport.Height / 2;*/

            var position = Matrix.CreateTranslation(
                -centre.X,
                -centre.Y,
                0);

            var offset = Matrix.CreateTranslation(
                viewport.Width / 2,
                viewport.Height / 2,
                0);

            var zoom = Matrix.CreateScale(ZoomAmount, ZoomAmount, 1);

            transform = position * zoom * offset;
        }
    }
}
