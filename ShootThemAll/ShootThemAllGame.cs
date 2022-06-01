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
    public struct PositionTexture
    {
        public Vector2 position;
        public Vector2 center;
        public Texture2D texture;
    }

    public class ShootThemAllGame : Game
    {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Set to true to draw the nice flower instead of the galaxy.
        readonly bool experimentalFlower = false;

        readonly int windowedWidth = 1000;
        readonly int windowedHeight = 1000;
        readonly Random randomizer = new();

        float flowerScale = 2.5f;
        float growShrink = 1.0f;

        readonly List<PositionTexture> signPositions = new();
        Color signColor = new(255, 255, 255, 210);

        float mapRot = 0.0f;
        Color mapColor = new(0, 255, 255, 255);
        Vector2 mapCenter;

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

            int w = Art.StarMap.Width / 2;
            int h = Art.StarMap.Height / 2;
            mapCenter = new(w, h);

            // We want the signs rotation center to be at the center-bottom where the arrow is.
            Vector2 signCenter = new(Art.Eridanus.Width / 2, Art.Eridanus.Height);

            // Add five star signs spread out over the galaxy.
            signPositions.Add(new PositionTexture()
            {
                position = new(randomizer.Next(-w, w), randomizer.Next(-h, h)),
                texture = Art.Eridanus,
                center = signCenter,
            });
            signPositions.Add(new PositionTexture() { position = new(randomizer.Next(-w, w), randomizer.Next(-h, h)), texture = Art.Sagittarius, center = signCenter });
            signPositions.Add(new PositionTexture() { position = new(randomizer.Next(-w, w), randomizer.Next(-h, h)), texture = Art.Cassiopeia, center = signCenter });
            signPositions.Add(new PositionTexture() { position = new(randomizer.Next(-w, w), randomizer.Next(-h, h)), texture = Art.Corvus , center = signCenter });
            signPositions.Add(new PositionTexture() { position = new(randomizer.Next(-w, w), randomizer.Next(-h, h)), texture = Art.Grus , center = signCenter });

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            mapRot += 0.01f;

            if(experimentalFlower)
                UpdateFlower(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (experimentalFlower)
            {
                DrawFlower();
                DrawCircleAndAxis();
            }
            else
            {
                DrawRotatingMap();
            }

            base.Draw(gameTime);
        }

        void DrawRotatingMap()
        {
            GraphicsDevice.Clear(Color.Black);

            Matrix RotMatrix = Matrix.CreateRotationZ(mapRot);
            Matrix ScaleMatrix = Matrix.CreateScale(0.8f);
            Matrix PosMatrix = Matrix.CreateTranslation(500, 500, 0);

            // The multiplication order is important when working with Matrixes, if you change the order you get entire different results. 
            // 
            // RotMatrix * ScaleMatrix * PosMatrix can be translated into: 
            // * Rotate the 'world'. (all images are rotated around 0,0)
            // * Scale the world. (scaling down all the images)
            // * Move the world 500,500 pixels. (This is not scaled down as we put it last in the multiplication)
            // 
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, RotMatrix * ScaleMatrix * PosMatrix);

            // The map image is rotated around its own center.
            spriteBatch.Draw(Art.StarMap, Vector2.Zero, null, mapColor, 0, mapCenter, 1.0f, 0, 0);

            // The map icons follows the map rotation. To make it look more google-maps alike their images are not rotated but kept straight.
            foreach (PositionTexture pt in signPositions)
            {
                // We 'undo' the rotation of the images by subtracting the angle here. If you leave angle at zero the images will be rotated too.
                spriteBatch.Draw(pt.texture, pt.position, null, signColor, -mapRot, pt.center, 0.6f, 0, 0);
            }

            spriteBatch.End();
        }

        private void UpdateFlower(GameTime gameTime)
        {
            flowerScale -= 0.01f;

            // Let the scale grow and shrink over time.
            flowerScale += growShrink * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 99;

            if (flowerScale > 2.0f)
            {
                growShrink = -1.0f;
                flowerScale = 2.0f;
            }
            else if (flowerScale < 0.1f)
            {
                growShrink = 1.0f;
                flowerScale = 0.1f;
            }
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
            rotationMatrix *= Matrix.CreateRotationZ(mapRot);

            // Scale the rotated stick. The scale matrix is scaling all three dimensions, but we use only x and y.
            Matrix scaleMatrix = Matrix.CreateScale(flowerScale);
            rotationMatrix *= scaleMatrix;

            positionMatrix += rotationMatrix;

            // Multiply a 1,1,1 vector with the matrix, to get the xyz position. 
            Vector3 v3position = Vector3.Transform(Vector3.One, positionMatrix);

            // spriteBatch.Draw() must have a Vector2 so transfer the coordinates.
            Vector2 xStickPosition = new(v3position.X + randomizer.NextSingle() / 1.0f, v3position.Y + randomizer.NextSingle() / 1.0f);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            spriteBatch.Draw(Art.Pixel, xStickPosition, Color.White);
            spriteBatch.End();
        }

        // Draw 4 pixels showing the axis movements.
        private void DrawCircleAndAxis()
        {
            Matrix positionMatrix = Matrix.CreateTranslation(150.0f, 150.0f, 0.0f);
            Vector3 v3position = Vector3.Transform(Vector3.One, positionMatrix);
            Vector2 origPosition = new(v3position.X, v3position.Y);

            Matrix rotationMatrix = Matrix.CreateTranslation(new(80, 0, 0));
            rotationMatrix *= Matrix.CreateRotationY(mapRot);
            v3position = Vector3.Transform(Vector3.One, positionMatrix + rotationMatrix);
            Vector2 xStickPosition = new(v3position.X, v3position.Y);

            rotationMatrix = Matrix.CreateTranslation(new(0, 80, 0));
            rotationMatrix *= Matrix.CreateRotationX(mapRot - (float)Math.PI / 2);
            v3position = Vector3.Transform(Vector3.One, positionMatrix + rotationMatrix);
            Vector2 yStickPosition = new(v3position.X, v3position.Y);

            rotationMatrix = Matrix.CreateTranslation(new(80, 0, 0));
            rotationMatrix *= Matrix.CreateRotationZ(mapRot);
            v3position = Vector3.Transform(Vector3.One, positionMatrix + rotationMatrix);
            Vector2 zStickPosition = new(v3position.X, v3position.Y);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            // Decided to draw a single pixel to make the movement pattern clearer than using a fuzzy skull. Replace with Skull if its too tiny.
            spriteBatch.Draw(Art.Pixel, origPosition, Color.White);
            spriteBatch.Draw(Art.Pixel, xStickPosition, Color.Blue);
            spriteBatch.Draw(Art.Pixel, yStickPosition, Color.Red);
            spriteBatch.Draw(Art.Pixel, zStickPosition, Color.Green);

            spriteBatch.End();
        }
    }
}
