using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;

namespace TKPlatformer
{
    class My
    {
        public static GameWindow window;
        private static List<Key> keysPressed;
        private static List<Key> keysPressedLast;
        private static List<MouseButton> mousePressed;
        private static List<MouseButton> mousePressedLast;

        public static int NumKeysPress
        {
            get
            {
                return keysPressed.Count;
            }
        }
        public static int NumKeysPressLast
        {
            get
            {
                return keysPressedLast.Count;
            }
        }
        public static Key[] KeysPressed
        {
            get
            {
                Key[] o = new Key[NumKeysPress];
                keysPressed.CopyTo(o);
                return o;
            }
        }
        public static Key[] KeysPressedLast
        {
            get
            {
                Key[] o = new Key[NumKeysPressLast];
                keysPressed.CopyTo(o);
                return o;
            }
        }

        public static Texture2D dot;

        public static void Initialize(GameWindow gameWindow)
        {
            window = gameWindow;
            keysPressedLast = new List<Key>();
            keysPressed = new List<Key>();
            mousePressedLast = new List<MouseButton>();
            mousePressed = new List<MouseButton>();
            window.Keyboard.KeyDown += Keyboard_KeyDown;
            window.Keyboard.KeyUp += Keyboard_KeyUp;
            window.Mouse.ButtonDown += Mouse_ButtonDown;
            window.Mouse.ButtonUp += Mouse_ButtonUp;

            #region Create dot texture
            {
                int id = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, id);

                Bitmap bmp = new Bitmap(1, 1);
                bmp.SetPixel(0, 0, Color.White);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, 1, 1), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    1, 1, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte, bmpData.Scan0);

                dot = new Texture2D("dot", id, 1, 1);

                bmp.UnlockBits(bmpData);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            }
            #endregion
        }

        static void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            mousePressed.Add(e.Button);
        }
        static void Mouse_ButtonUp(object sender, MouseButtonEventArgs e)
        {
            mousePressed.Remove(e.Button);
        }

        private static void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            keysPressed.Add(e.Key);
        }
        static void Keyboard_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            keysPressed.Remove(e.Key);
        }

        /// <summary>
        /// This should be done at the beginning of the Update function
        /// before other things update. Otherwise the Key and Mouse functions
        /// might not work properly
        /// </summary>
        public static void Update()
        {
            //Not currently used. Consider getting rid of?
        }

        /// <summary>
        /// This should be done at the end of each Update function
        /// after everything has been updated. Otherwise the Key and Mouse
        /// functions might not work properly
        /// </summary>
        public static void UpdateAfter()
        {
            keysPressedLast = new List<Key>();
            //NOt sure if there is a function that will automatically
            //copy one list to the other but thought it was more safe
            //to make sure I don't make a reference on accident
            for (int i = 0; i < keysPressed.Count; i++)
            {
                keysPressedLast.Add(keysPressed[i]);
            }
            mousePressedLast = new List<MouseButton>();
            for (int i = 0; i < mousePressed.Count; i++)
            {
                mousePressedLast.Add(mousePressed[i]);
            }
        }

        public static bool KeyPress(Key key)
        {
            return (keysPressed.Contains(key) && !keysPressedLast.Contains(key));
        }
        public static bool KeyRelease(Key key)
        {
            return (!keysPressed.Contains(key) && keysPressedLast.Contains(key));
        }
        public static bool KeyDown(Key key)
        {
            return (keysPressed.Contains(key));
        }

        public static bool MousePress(bool left)
        {
            return (left ? MousePress(MouseButton.Left) : MousePress(MouseButton.Right));
        }
        public static bool MousePress(MouseButton button)
        {
            return (mousePressed.Contains(button) && !mousePressedLast.Contains(button));
        }
        public static bool MouseRelease(MouseButton button)
        {
            return (!mousePressed.Contains(button) && mousePressedLast.Contains(button));
        }
        public static bool MouseRelease(bool left)
        {
            return (left ? MouseRelease(MouseButton.Left) : MouseRelease(MouseButton.Right));
        }
        public static bool MouseDown(MouseButton button)
        {
            return (mousePressed.Contains(button));
        }
        public static bool MouseDown(bool left)
        {
            return (left ? MouseDown(MouseButton.Left) : MouseDown(MouseButton.Right));
        }
    }
}
