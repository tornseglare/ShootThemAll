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
        // Try to set these to false to see the movement between the stars more clearly.
        bool rotate = true;
        bool zoomInAndOut = true;  

        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        readonly int windowedWidth = 1000;
        readonly int windowedHeight = 1000;
        readonly Random randomizer = new();

        float scale = 3.0f;
        double growShrink = 1.0d;

        readonly List<PositionTexture> signPositions = new();
        Color signColor = new(255, 255, 255, 210);

        float mapRot = 0.0f;
        Color mapColor = new(0, 255, 255, 255);
        Vector2 mapCenter;

        // The camera is moving between the stars. 
        Vector2 cameraCenter;

        // Which star is the camera moving towards? 
        int travellingTo = 0;

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

            cameraCenter = signPositions[travellingTo].position;

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(rotate)
                mapRot += 0.01f;

            if(zoomInAndOut)
                ZoomInAndOut(gameTime);

            TravelToStar(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            DrawRotatingMap();
            base.Draw(gameTime);
        }

        void DrawRotatingMap()
        {
            GraphicsDevice.Clear(Color.Black);

            Matrix CameraMatrix = Matrix.CreateTranslation(-cameraCenter.X,-cameraCenter.Y, 0);
            Matrix RotMatrix = Matrix.CreateRotationZ(mapRot);
            Matrix ScaleMatrix = Matrix.CreateScale(scale);
            Matrix PosMatrix = Matrix.CreateTranslation(500, 500, 0);

            // The multiplication order is important when working with Matrixes, if you change the order you get entire different results. 
            // 
            // CameraMatrix * RotMatrix * ScaleMatrix * PosMatrix can be translated into: 
            // * Move the world to where the camera is.
            // * Rotate the 'world'. (all images are rotated around the camera's X,Y position)
            // * Scale the world. (scaling down all the images)
            // * Move the world 500,500 pixels. (This is not scaled down as we put it last in the multiplication)
            // 
            spriteBatch.Begin(
                SpriteSortMode.Deferred, 
                BlendState.NonPremultiplied, 
                null, null, null, null,
                CameraMatrix * RotMatrix * ScaleMatrix * PosMatrix);

            // The map image is rotated around its own center.
            spriteBatch.Draw(
                Art.StarMap, 
                Vector2.Zero, 
                null, mapColor, 0,
                mapCenter, 
                1.0f, 0, 0);

            // The map icons are positioned around the map center, so follows the map rotation.
            // To make it look more google-maps alike their images are not rotated but kept straight.
            foreach (PositionTexture pt in signPositions)
            {
                // We 'undo' the rotation of the images by subtracting the angle here. If you leave angle at zero the images will be rotated too.
                spriteBatch.Draw(
                    pt.texture, 
                    pt.position, 
                    null, signColor, -mapRot, 
                    pt.center, 
                    0.6f, 0, 0);
            }

            spriteBatch.End();

            // Draw the star we are travelling to at the top-left corner of the view.
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.NonPremultiplied);

            PositionTexture targetStar = signPositions[travellingTo];
            spriteBatch.Draw(
                targetStar.texture,
                Vector2.Zero,
                null, signColor, 0,
                Vector2.Zero,
                0.4f, 0, 0);

            spriteBatch.End();
        }

        private void TravelToStar(GameTime gameTime)
        {
            Vector2 target = signPositions[travellingTo].position;

            if(CloseEnough(cameraCenter.X, target.X) && CloseEnough(cameraCenter.Y, target.Y))
            {
                // We found our star. Travel to the next one.
                travellingTo++;
                if(travellingTo >= signPositions.Count)
                {
                    travellingTo = 0;
                }
            }
            else
            {
                Vector2 diff = cameraCenter - target;

                cameraCenter.X -= Move(diff.X, gameTime);
                cameraCenter.Y -= Move(diff.Y, gameTime);
            }
        }

        // Move with the minimum speed of 0.01f.
        private static float Move(float along, GameTime gameTime)
        {
            float speed = 3.0f;
            float movement = (float)(along / (speed * gameTime.ElapsedGameTime.TotalMilliseconds));

            if (along > 0 && movement < 0.02f)
                movement = 0.02f;
            if (along < 0 && movement > -0.02f)
                movement = -0.02f;

            return movement;
        }

        private static bool CloseEnough(float pos1, float pos2)
        {
            if(pos1 + 0.05f  > pos2 && pos1 - 0.05f < pos2) 
            {
                return true;
            }

            return false;
        }

        private void ZoomInAndOut(GameTime gameTime)
        {
            double speed = 800; // Larger is slower.
            float maxScale = 5.0f;

            // Let the scale grow and shrink over time.
            scale += (float)(growShrink * gameTime.ElapsedGameTime.TotalMilliseconds / speed);

            if (scale > maxScale)
            {
                growShrink = -1.0f;
                scale = maxScale;
            }
            else if (scale < 0.3f)
            {
                growShrink = 1.0f;
                scale = 0.3f;
            }
        }
    }
}
