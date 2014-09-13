using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;
using QuickFont;

namespace TKPlatformer
{
    class Game1
    {
        public static double WINDOW_FRAMERATE = 60.0;
        public static int WINDOW_WIDTH = 640;
        public static int WINDOW_HEIGHT = 480;
        public static string WINDOW_NAME = "OpenTK Test";
        public static GameWindowFlags WINDOW_FLAGS = GameWindowFlags.Default;

        public GameWindow window;
        public View view;
        public QFont font;

        /// <summary>
        /// This automatically binds Game1's function to window's events
        /// </summary>
        /// <param name="window">The GameWindow that will control this Game1</param>
        public Game1(OpenTK.GameWindow window)
        {
            //I believe this is just a reference to the window object
            this.window = window;
            window.Load += this.Load;
            window.Resize += this.Resize;
            window.RenderFrame += this.Render;
            window.UpdateFrame += this.Update;

            window.VSync = VSyncMode.Off;

            My.Initialize(window);

            Spritebatch.Initialize(window);

            view = new View(window, Vector2.Zero, 10.0, 1, 0.0);
            view.enableRounding = false;
        }

        public void Load(object sender, EventArgs e)
        {
            font = new QFont("Content\\TIMESBD.ttf", 16);


        }

        public void Close()
        {
            
        }

        public void Update(object sender, EventArgs e)
        {
            My.Update();
            if (window.Keyboard[Key.Escape])
            {
                this.Close();
                window.Exit();
            }



            #region View Movement
            view.BasicMovement(5.0f, true, false);
            if (My.KeyDown(Key.E))
            {
                view.Rotate(1);
            }
            if (My.KeyDown(Key.Q))
            {
                view.Rotate(-1);
            }
            if (My.KeyDown(Key.Z))
            {
                view.zoom += 0.01;
            }
            if (My.KeyDown(Key.X))
            {
                view.zoom -= 0.01;
            }
            #endregion
            view.Update();
            My.UpdateAfter();
        }

        public void Render(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Spritebatch.Begin(4.0f, view);

            

            Spritebatch.End();

            #region Debug Text
            QFont.Begin();
            QFontRenderOptions op = new QFontRenderOptions();
            op.Colour = Color.Red;
            font.PushOptions(op);
            font.Print(String.Format(
                "x {0}\ny {1}\nz {2}\nr {3}", 
                Math.Round(view.position.X, 1),
                Math.Round(view.position.Y, 1),
                Math.Round(view.zoom, 4),
                Math.Round(view.rotation, 0)),
                100f, QFontAlignment.Left, new Vector2(0, 0));
            QFont.End();
            #endregion

            window.SwapBuffers();
        }

        public void Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, window.Width, window.Height);
        }
    }
}
