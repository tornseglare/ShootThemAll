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
    public class PositionScaleRotation
    {
        public Vector2 position;
        public float scale;
        public float rotation;
    }

    public class ShootThemAllGame : Game
    {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        readonly int windowedWidth = 800;
        readonly int windowedHeight = 600;

        float angle = 0.0f;
        static readonly double fullCircle = Math.PI * 2.0f; // equals 360 degrees, but expressed in radians.

        public ShootThemAllGame()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                HardwareModeSwitch = true // Try to set this to false.
            };

            GoWindowed();
        }

        /// <summary>
        /// If currently in fullscreen, leave it. Set windows width and height to windowedWidth and windowedHeight.
        /// </summary>
        public void GoWindowed()
        {
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = windowedWidth;
            graphics.PreferredBackBufferHeight = windowedHeight;
            graphics.ApplyChanges();

            // This is a bug, many years old and never fixed in Monogame: You must set resolution twice for it to have any effect, at least in windows.
            graphics.PreferredBackBufferWidth = windowedWidth;
            graphics.PreferredBackBufferHeight = windowedHeight;
            graphics.ApplyChanges();

            Window.Position = new Point(42, 42);
            Window.AllowUserResizing = true;
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

            angle += (float)(fullCircle / 3600.0d * gameTime.ElapsedGameTime.TotalMilliseconds);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            // First we create a position as a matrix. This is pixel position 50, 50. 
            // Using Matrix requires three dimensions, XYZ. We set Z to zero since we are a 2Dgame.
            Matrix positionMatrix = Matrix.CreateTranslation(150.0f, 150.0f, 0.0f);

            // Second we create the rotation matrix. The best way to see this is as one of the hands on the clock. I call it stick. 
            // We just give the stick a length in pixels, and this will be rotated, so effectively it becomes the radius. 
            // Idea: The stick is rotated angle radians, with the skull at the end of the stick. It will look like the skull is rotating 
            // around the clock, i.e it is rotating around a center. 
            Vector3 stick = new();
            stick.X = 80;
            stick.Y = 0;
            stick.Z = 0;

            Matrix rotationMatrix = Matrix.CreateTranslation(stick);

            // Rotate the matrix.
            rotationMatrix *= Matrix.CreateRotationZ(angle);

            // Now just add our rotation matrix to the position matrix.
            positionMatrix += rotationMatrix;

            // Multiply a 1,1,1 vector with the matrix, to get the final position.
            Vector3 v3position = Vector3.Transform(Vector3.One, positionMatrix);

            // spriteBatch.Draw() must have a Vector2 so transfer the coordinates.
            Vector2 position = new();
            position.X = v3position.X;
            position.Y = v3position.Y;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            spriteBatch.Draw(Art.Skull, position, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
