using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShootThemAll
{
    static class Art
    {
        public static Texture2D Snail { get; private set; }
        public static Texture2D Skull { get; private set; }
        public static Texture2D Pixel { get; private set; }     // a single white pixel

        public static void Load(ContentManager content)
        {
            Snail = content.Load<Texture2D>("Art/snail");
            Skull = content.Load<Texture2D>("Art/spontaneous");

            Pixel = new Texture2D(Snail.GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });
        }
    }
}
