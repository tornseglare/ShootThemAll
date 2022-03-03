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

        // The tint of the image. This will also allow us to change the transparency.
        Color color = Color.White;

        int count = 50;

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
            count += 100;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
            Random random = new Random();
            Vector2 pos = new();

            for (int i = 0; i < count; i++)
            {
                pos.X = random.Next(600);
                pos.Y = random.Next(350);
                spriteBatch.Draw(Art.Skull, pos, null, color, 0f, Vector2.Zero, 0.5f, 0, 0);
            }

            // Each frame takes a certain amount of time, normally around 16ms. How many times a second is that? 1000 / 16 =~ 60 times a second. That is the FPS, frames per second.
            double fps = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;
            string text = "FPS: " + (int)fps + " ObjCount: " + count;
            Vector2 textPos = new(0, 400);
            spriteBatch.DrawString(Art.Font, text, textPos, Color.White);

            //Debug.WriteLine(gameTime.ElapsedGameTime.TotalMilliseconds);
            //Debug.WriteLine(fps);
            //double fps60 = 1000 / 60;
            //Debug.WriteLine(gameTime.ElapsedGameTime.TotalMilliseconds / fps60); // Should be close to one if 60 fps.

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
