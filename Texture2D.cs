using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace TKPlatformer
{
    struct Texture2D
    {
        private int id;
        private Vector2 size;
        private string fileName;

        public string FileName
        {
            get { return fileName; }
        }
        public Vector2 Size
        {
            get { return size; }
        }
        public int Id
        {
            get { return id; }
        }
        public int Width
        {
            get
            {
                return (int)size.X;
            }
        }
        public int Height
        {
            get
            {
                return (int)size.Y;
            }
        }

        public Texture2D(string fileName, int id, int width, int height)
        {
            this.fileName = fileName;
            this.id = id;
            this.size = new Vector2(width, height);
        }
    }
}
