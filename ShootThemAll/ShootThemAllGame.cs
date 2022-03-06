using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShootThemAll
{
    public class ShootThemAllGame : Game
    {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int count = 10;

        bool growing = false;
        int alpha = 255;

        public ShootThemAllGame()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 2560,
                PreferredBackBufferHeight = 1440
            };
        }

        protected override void Initialize()
        {
            this.Content.RootDirectory = "Content";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Art.Load(Content);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            // Increase the number of skulls until the framerate drops. 
            count += 1;

            // Cycle the alpha between 0 and 255.
            if (growing)
                alpha += 10;
            else
                alpha -= 10;
            if(alpha < 0)
            {
                alpha = 0;  
                growing = true;
            }
            if(alpha > 255)
            {
                alpha = 255;
                growing = false;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            // Working with alpha requires the NonPremultiplied blendstate.
            // 
            // NonPremultiplied basically means: 
            //  sourceBlend gets set to SourceAlpha - Each component of the color is multiplied by the alpha value of the source.
            //  destinationBlend gets set to InverseSourceAlpha - Each component of the color is multiplied by the inverse of the alpha value of the source.
            // 
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied);

            // Draw a few resized pixels and multiply with the cycling alpha.
            for (int i = 10; i < 700; i += 8)
            {
                Rectangle targetRect = new(i, 2, 7, 7);

                Color color = new((byte)255, (byte)255, (byte)255, (alpha));
                spriteBatch.Draw(Art.Pixel, targetRect, null, color);
            }

            Random random = new Random();
            Vector2 pos = new();

            for (int i = 0; i < count; i++)
            {
                pos.X = random.Next(600);
                pos.Y = 10 + random.Next(350);
                spriteBatch.Draw(Art.Skull, pos, null, Color.White, 0f, Vector2.Zero, 0.5f, 0, 0);
            }

            // Each frame takes a certain amount of time, normally around 16ms. How many times a second is that? 1000 / 16 =~ 60 times a second. That is the FPS, frames per second.
            double fps = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;
            string text = "FPS: " + (int)fps + " ObjCount: " + count;
            Vector2 textPos = new(0, 400);
            spriteBatch.DrawString(Art.Font, text, textPos, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
