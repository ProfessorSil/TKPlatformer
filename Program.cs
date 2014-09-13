using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;

namespace TKPlatformer
{
    class Program
    {
        static void Main(string[] args)
        {
            GameWindow window = new GameWindow(Game1.WINDOW_WIDTH, Game1.WINDOW_HEIGHT, GraphicsMode.Default, Game1.WINDOW_NAME, Game1.WINDOW_FLAGS);
            Game1 game = new Game1(window);
            window.Run(Game1.WINDOW_FRAMERATE);
        }
    }
}
