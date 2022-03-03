using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ShootThemAll
{
    public class ShootThemAllGame : Game
    {
        GraphicsDeviceManager graphics;

        public ShootThemAllGame()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080
            };
        }
    }
}
