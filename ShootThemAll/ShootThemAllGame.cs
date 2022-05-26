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

        readonly int windowedWidth = 700;
        readonly int windowedHeight = 700;

        float angle = 0.0f;
        float theScale = 2.5f;
        float growShrink = 1.0f;

        readonly double fullCircle = Math.PI * 2.0f; // equals 360 degrees, but expressed in radians.
        readonly Random randomizer = new();

        readonly List<PositionScaleRotation> things = new();

        public ShootThemAllGame()
        {
            graphics = new GraphicsDeviceManager(this);

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

            int top = -windowedHeight / 2;// + Art.Skull.Height;
            int bottom = windowedHeight / 2 - Art.Skull.Height;
            int left = -windowedWidth / 2;// + Art.Skull.Width;
            int right = windowedWidth / 2 - Art.Skull.Width;

            for (int i = 0; i < 10; i++)
            {
                things.Add(new PositionScaleRotation()
                {
                    position = new Vector2(randomizer.Next(left, right), randomizer.Next(top, bottom)),
                    rotation = 0.0f,
                    scale = 1.0f
                });
            }

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            angle += (float)(fullCircle / (3600.0d * 2) * gameTime.ElapsedGameTime.TotalMilliseconds);

            if (angle > fullCircle)
                angle -= (float)fullCircle;
            if(angle < 0)
                angle += (float)fullCircle;

            // Let the scale grow and shrink over time.
            //theScale += growShrink * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 99;

            if (theScale > 2.0f)
            {
                growShrink = -1.0f;
                theScale = 2.0f;
            }
            else if(theScale < 0.1f)
            {
                growShrink = 1.0f;
                theScale = 0.1f;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            RotateSomeSkulls();

            /*// Some experiments. If you want to test them, comment out the line above and uncomment this.
            
            // Turn off the clearing to see the things drawn over time.
            //GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            DrawSomeSkulls();
            DrawFlower();
            DrawCircleAndAxis();

            spriteBatch.End();*/

            base.Draw(gameTime);
        }

        // Best with the intro-minute of Pink Floyds "Coming back to life". ;-)
        private void DrawSomeSkulls()
        {
            Matrix positionMatrix = Matrix.CreateTranslation(300.0f, 300.0f, 0.0f);

            positionMatrix *= Matrix.CreateRotationZ(angle);

            Vector3 v3position = Vector3.Transform(Vector3.One, positionMatrix);

            Vector2 position = new(v3position.X, v3position.Y);

            foreach (PositionScaleRotation thing in things)
            {
                spriteBatch.Draw(Art.Skull, thing.position + position, Color.White);
            }
        }

        // Rotate the matrix around its 'center', move the rotation center to the center of the screen, 
        // then draw the skulls. Since their positions are randomized around this center they will 
        // rotate around it nicely. 
        // 
        private void RotateSomeSkulls()
        {
            GraphicsDevice.Clear(Color.Black);

            //Matrix rotationMatrix = Matrix.Identity;
            Matrix rotationMatrix = Matrix.CreateRotationZ(angle);
            Matrix positionMatrix = Matrix.CreateTranslation(windowedWidth / 2, windowedHeight / 2, 0.0f);

            rotationMatrix *= positionMatrix;

            spriteBatch.Begin(
                SpriteSortMode.Deferred, 
                BlendState.NonPremultiplied, 
                null, null, null, null,
                rotationMatrix);

            foreach (PositionScaleRotation thing in things)
            {
                spriteBatch.Draw(Art.Skull, thing.position, Color.White);
            }

            spriteBatch.End();
        }

        // Draw a circle, but change the radius while drawing it. Turns into a flower. :-) 
        private void DrawFlower()
        {
            // First we create a position as a matrix. This is pixel position.
            // Using Matrix requires three dimensions, XYZ. We set Z to zero since we are a 2Dgame.
            Matrix positionMatrix = Matrix.CreateTranslation(300.0f, 300.0f, 0.0f);

            // Second we create the rotation matrix. The best way to see this is as one of the hands on the clock. I call it stick. 
            // We just give the stick a length in pixels, and this will be rotated, so effectively it becomes the radius. 
            // Idea: The stick is rotated angle radians, with the skull at the end of the stick. It will look like the skull is rotating 
            // around the clock, i.e it is rotating around a center. 
            Vector3 stick = new(80, 0, 0);

            Matrix rotationMatrix = Matrix.CreateTranslation(stick);

            // Rotate the matrix.
            rotationMatrix *= Matrix.CreateRotationZ(angle);

            // Scale the rotated stick. The scale matrix is scaling all three dimensions, but we use only x and y.
            Matrix scaleMatrix = Matrix.CreateScale(theScale);
            rotationMatrix *= scaleMatrix;

            positionMatrix += rotationMatrix;

            // Multiply a 1,1,1 vector with the matrix, to get the xyz position. 
            Vector3 v3position = Vector3.Transform(Vector3.One, positionMatrix);

            // spriteBatch.Draw() must have a Vector2 so transfer the coordinates.
            Vector2 xStickPosition = new(v3position.X + randomizer.NextSingle() / 1.0f, v3position.Y + randomizer.NextSingle() / 1.0f);

            spriteBatch.Draw(Art.Pixel, xStickPosition, Color.White);
        }

        // Draw 4 pixels showing the axis movements.
        private void DrawCircleAndAxis()
        {
            Matrix positionMatrix = Matrix.CreateTranslation(150.0f, 150.0f, 0.0f);
            Vector3 v3position = Vector3.Transform(Vector3.One, positionMatrix);
            Vector2 origPosition = new(v3position.X, v3position.Y);

            Matrix rotationMatrix = Matrix.CreateTranslation(new(80, 0, 0));
            rotationMatrix *= Matrix.CreateRotationY(angle);
            v3position = Vector3.Transform(Vector3.One, positionMatrix + rotationMatrix);
            Vector2 xStickPosition = new(v3position.X, v3position.Y);

            rotationMatrix = Matrix.CreateTranslation(new(0, 80, 0));
            rotationMatrix *= Matrix.CreateRotationX(angle - (float)Math.PI / 2);
            v3position = Vector3.Transform(Vector3.One, positionMatrix + rotationMatrix);
            Vector2 yStickPosition = new(v3position.X, v3position.Y);

            rotationMatrix = Matrix.CreateTranslation(new(80, 0, 0));
            rotationMatrix *= Matrix.CreateRotationZ(angle);
            v3position = Vector3.Transform(Vector3.One, positionMatrix + rotationMatrix);
            Vector2 zStickPosition = new(v3position.X, v3position.Y);

            // Decided to draw a single pixel to make the movement pattern clearer than using a fuzzy skull. Replace with Skull if its too tiny.
            spriteBatch.Draw(Art.Pixel, origPosition, Color.White);
            spriteBatch.Draw(Art.Pixel, xStickPosition, Color.Blue);
            spriteBatch.Draw(Art.Pixel, yStickPosition, Color.Red);
            spriteBatch.Draw(Art.Pixel, zStickPosition, Color.Green);
        }
    }
}
