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
        public Map map;
        public SolidObject obj;

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
            view.SetPosition(view.SizeWorld / 2f);
            view.enableRounding = false;

            map = new Map("DevRoom");

            obj = new SolidObject("PlayerTest", new Vector2(32, 32), new Vector2(24, 31), new Vector2(0, 0.5f));
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

            obj.Update(ref map);

            #region Player Movement
            {
                float speed = 2;
                float jumpSpeed = 10;
                if (obj.IsOnGround)
                    obj.velocity.X = 0;
                if (My.KeyDown(Key.Left) && obj.IsOnGround)
                {
                    obj.velocity.X += -speed;
                }
                if (My.KeyDown(Key.Right) && obj.IsOnGround)
                {
                    obj.velocity.X += speed;
                } 
                if (My.KeyPress(Key.Space) && obj.IsOnGround)
                {
                    obj.velocity.Y = -jumpSpeed;
                }
            }
            #endregion

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

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    Color col = Color.Black;
                    if (map.colGrid.GetValue(x, y) == CollisionGrid.CollisionType.Solid)
                    {
                        col = Color.Red;
                    }
                    else if (map.colGrid.GetValue(x, y) == CollisionGrid.CollisionType.Platform)
                    {
                        col = Color.Blue;
                    }
                    else if (map.colGrid.GetValue(x, y) == CollisionGrid.CollisionType.Empty)
                    {
                        col = Color.Gray;
                    }

                    Spritebatch.DrawRectangle(new Vector2(x * 32, y * 32), new Vector2(32, 32), col);

                }
            }
            obj.Draw();

            Spritebatch.End();

            #region Debug Text
            QFont.Begin();
            QFontRenderOptions op = new QFontRenderOptions();
            op.Colour = Color.White;
            font.PushOptions(op);
            font.Print(String.Format(
                "vx={0} vy={1}\ng {2}\nl {3}\npx={4} py={5}",
                Math.Round(obj.velocity.X, 1),
                Math.Round(obj.velocity.Y, 1),
                obj.IsOnGround,
                obj.ColLeft,
                Math.Round(obj.position.X, 1),
                Math.Round(obj.position.Y, 1)));
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
