using System;
using System.Collections.Generic;
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
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
            Random random = new Random();
            Vector2 pos = new();

            for (int i = 0; i < 10; i++)
            {
                pos.X = random.Next(400);
                pos.Y = random.Next(400);
                spriteBatch.Draw(Art.Skull, pos, null, color, 0f, Vector2.Zero, 0.5f, 0, 0);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
