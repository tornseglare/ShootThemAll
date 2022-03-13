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
    public class PositionAndScale
    {
        public Vector2 position;
        public float scale;
    }

    public class ShootThemAllGame : Game
    {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        bool growing = false;
        int alpha = 255;

        int windowedWidth = 800;
        int windowedHeight = 600;

        Random randomizer = new Random();
        List<PositionAndScale> objects = new();

        public ShootThemAllGame()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                HardwareModeSwitch = true // Try to set this to false.
            };

            // Setup some event listeners, not necessary but for easier debugging.
            graphics.DeviceResetting += Graphics_DeviceResetting;
            graphics.DeviceReset += Graphics_DeviceReset;
            graphics.DeviceCreated += Graphics_DeviceCreated;
            graphics.PreparingDeviceSettings += Graphics_PreparingDeviceSettings;
            graphics.DeviceDisposing += Graphics_DeviceDisposing;

            // SupportedDisplayModes();

            //DisplayMode displayMode = GetDisplayModeBestMatch(1400, 900);
            //GoFullScreen(displayMode);

            //GoFullScreenNativeResolution();
            //GoFullScreenWithResolution(720, 400);
            GoWindowed();
            //GoWindowed(400, 400);

            // To allow for higher fps than 60, set to false. This means the screen will be redrawn as fast as possible, possibly several thousand frames per second, so you need to think about it. :-)
            IsFixedTimeStep = false;

            // Turn off the vertical sync to acheive even higher framerates at the cost of tearing issues.
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Set fullscreen and ask for the display adapters maximum resolution.
        /// </summary>
        public void GoFullScreenNativeResolution()
        {
            GoFullScreenWithResolution(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        }

        /// <summary>
        /// Set fullscreen and ask for the given resolution.
        /// </summary>
        public void GoFullScreenWithResolution(int width, int height)
        {
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();

            // This is a bug, many years old and never fixed in Monogame: You must set resolution twice for it to work properly, at least in windows, at least on some computers.
            // https://community.monogame.net/t/change-window-size-mid-game/1991/8
            // https://community.monogame.net/t/fullscreen-game-not-rendering-to-proper-size-for-resolutions-besides-1920-x-1080/8255/8
            // 
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();

            // Note that CurrentDisplayMode seem to reflect the monitor's maximum resolution, while the PreferredBackBufferXYZ reflect the resolution in which the graphics is drawn.
            Debug.WriteLine("Current, Width: " + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width + " vs preferred " + graphics.PreferredBackBufferWidth);
            Debug.WriteLine("Current, Height: " + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height + " vs preferred " + graphics.PreferredBackBufferHeight);
            Debug.WriteLine("Current, AspectRatio: " + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio);

            
            if (width != graphics.GraphicsDevice.Viewport.Width || height != graphics.GraphicsDevice.Viewport.Height)
            {
                Debug.WriteLine("The new resolution are not equal to your requested resolution.");

                // If you choose Width:720 Height:200, Viewport reports that's what you got, but in reality the screen resolution you got is one of them displayed in SupportedDisplayModes(),
                // normally the one with the closest match.
                // So question remains: Where can I ask for the actual resolution? 
                //  <-GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width and Height gives the max resolution instead of the actual values.
            }
        }

        /// <summary>
        /// Go fullscreen with the given backbuffer format and resolution. Use GetDisplayModeBestMatch() to get a proper DisplayMode object.
        /// </summary>
        public void GoFullScreen(DisplayMode displayMode)
        {
            graphics.PreferredBackBufferFormat = displayMode.Format;
            GoFullScreenWithResolution(displayMode.Width, displayMode.Height);
        }

        /// <summary>
        /// Writes out your supported resolutions in debug window.
        /// </summary>
        public static void SupportedDisplayModes()
        {
            foreach (DisplayMode sdm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                int w = sdm.Width;
                int h = sdm.Height;
                float a = sdm.AspectRatio;
                string f = sdm.Format.ToString();

                Debug.WriteLine("Width: " + w + " Height: " + h + " Aspect: " + a + " Format: " + f);
            }
        }

        /// <summary>
        /// Get the best matching DisplayMode the system can give you. 
        /// </summary>
        public static DisplayMode GetDisplayModeBestMatch(int width, int height)
        {
            // Fetch the first one matching width and height.
            DisplayMode sdMode = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.Where(dm => dm.Width >= width && dm.Height >= height).OrderBy(dm => dm.Width).ThenBy(dm => dm.Height).FirstOrDefault();

            if (sdMode == null)
            {
                // Either the requested width or the height are too large. Let's find a dmode matching at least the height or the width.
                sdMode = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.Where(dm => dm.Width >= width || dm.Height >= height).OrderBy(dm => dm.Width).ThenBy(dm => dm.Height).FirstOrDefault();

                if (sdMode == null)
                {
                    // Neither requested width or height can be satisfied, lets just find the display mode with the largest width and height.
                    sdMode = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.
                        OrderByDescending(dm => dm.Width).
                        ThenByDescending(dm => dm.Height).
                        FirstOrDefault();

                    if (sdMode == null)
                    {
                        throw new Exception("No display modes available.");
                    }
                }
            }

            return sdMode;
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

            // Please note that GraphicsAdapter.DefaultAdapter.CurrentDisplayMode is the monitor resolution, not the resolution of the window.
            Debug.WriteLine("Current, Width: " + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width + " vs preferred " + graphics.PreferredBackBufferWidth);
            Debug.WriteLine("Current, Height: " + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height + " vs preferred " + graphics.PreferredBackBufferHeight);
            Debug.WriteLine("Current, AspectRatio: " + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio);
        }

        public void GoWindowed(int width, int height)
        {
            windowedWidth = width;
            windowedHeight = height;

            GoWindowed();
        }

        private void Graphics_DeviceDisposing(object sender, EventArgs e)
        {
            Debug.WriteLine("DeviceDisposing..");
        }

        private void Graphics_DeviceCreated(object sender, EventArgs e)
        {
            Debug.WriteLine("DeviceCreated..");
        }

        private void Graphics_DeviceReset(object sender, EventArgs e)
        {
            Debug.WriteLine("DeviceReset..");

            // The ClientBounds and Viewport are the same except in some special cases. 
            // Please note that for fullscreen mode this does NOT always give the actual resolution! 
            // (Fullscreen mode with maximum resolution normally makes all three end up equal, also the GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width and Height.)
            // https://stackoverflow.com/questions/8954945/xna-get-current-screen-resolution
            // 
            if (sender is GraphicsDeviceManager m)
            {
                Debug.WriteLine("Viewport: " + m.GraphicsDevice.Viewport.ToString());
                Debug.WriteLine("GraphicsDeviceStatus: " + m.GraphicsDevice.GraphicsDeviceStatus);
                Debug.WriteLine("GraphicsProfile: " + m.GraphicsDevice.GraphicsProfile);
            }

            // ClientBounds also show the top-left position of the window in windowed mode.
            Debug.WriteLine("ClientBounds: " + Window.ClientBounds.ToString());
        }

        private void Graphics_DeviceResetting(object sender, EventArgs e)
        {
            Debug.WriteLine("DeviceResetting..");
        }

        private void Graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            Debug.WriteLine("PreparingDeviceSettings current width: " + e.GraphicsDeviceInformation.Adapter.CurrentDisplayMode.Width);
            Debug.WriteLine("PreparingDeviceSettings current height: " + e.GraphicsDeviceInformation.Adapter.CurrentDisplayMode.Height);
            Debug.WriteLine(e.GraphicsDeviceInformation.PresentationParameters.Bounds.ToString());
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Debug.WriteLine("Exiting program..");
            base.OnExiting(sender, args);
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
            // Add a new skull every now and then with random scale to see the mipmapping in action.
            // Try to set /processorParam:GenerateMipmaps to False in the file ShootThemAll.mgcb to see how pixelated they get when drawn small.
            for(int i=0;i<10;i++)
            //if (randomizer.Next(0, 100) < 100)
            {
                PositionAndScale obj = new() { position = new(randomizer.Next(0, windowedWidth), randomizer.Next(0, windowedHeight)), scale = randomizer.NextSingle() };
                objects.Add(obj);
            }

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
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            // Draw a few resized pixels and multiply with the cycling alpha.
            int side = 32;
            Color color = new((byte)255, (byte)255, (byte)255, (alpha));
            for (int i = 0; i < 22; i++)
            {
                Rectangle targetRect = new(i * side, 0, side - 1, side - 1);
                spriteBatch.Draw(Art.Pixel, targetRect, null, color);
            }
            for (int i = 1; i < 12; i++)
            {
                Rectangle targetRect = new(0, i * side, side - 1, side - 1);
                spriteBatch.Draw(Art.Pixel, targetRect, null, color);
            }

            foreach(PositionAndScale obj in objects)
            {
                spriteBatch.Draw(Art.Skull, obj.position, null, Color.White, 0f, Vector2.Zero, obj.scale, SpriteEffects.None, 0);
            }

            // Each frame takes a certain amount of time, normally around 16ms. How many times a second is that? 1000 / 16 =~ 60 times a second. That is the FPS, frames per second.
            double fps = 1000 / gameTime.ElapsedGameTime.TotalMilliseconds;
            string text = "FPS: " + (int)fps + " ObjCount: " + objects.Count;
            Vector2 textPos = new(20, 20);
            spriteBatch.DrawString(Art.Font, text, textPos, Color.DarkBlue);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
