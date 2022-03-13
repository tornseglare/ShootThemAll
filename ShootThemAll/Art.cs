﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShootThemAll
{
    static class Art
    {
        public static Texture2D Snail { get; private set; }
        public static Texture2D Skull { get; private set; }
        public static Texture2D Pixel { get; private set; }     // a single white pixel

        public static SpriteFont Font { get; private set; }

        public static void Load(ContentManager content)
        {
            Snail = content.Load<Texture2D>("Art/snail");
            Skull = content.Load<Texture2D>("Art/spontaneous");

            // If you want to build the content during startup. Make sure the snail.png is set to Copy. 
            //Snail = Texture2D.FromFile(Skull.GraphicsDevice, "Content/Art/snail.png");

            Pixel = new Texture2D(Snail.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] colorData = new[] { new Color(255, 255, 255, 255) };
            Pixel.SetData<Color>(colorData);

            Font = content.Load<SpriteFont>("Font");
        }
    }
}
