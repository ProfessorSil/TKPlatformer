using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;

namespace TKPlatformer
{
    class CollisionGrid
    {
        public static enum CollisionType
        {
            Solid,
            Empty,
            Platform
        }

        private CollisionType[,] values;

        private int width, height;

        public int Width
        {
            get
            {
                return width;
            }
        }
        public int Height
        {
            get
            {
                return height;
            }
        }

        public CollisionGrid(int Width, int Height)
        {
            this.width = Width;
            this.height = Height;
            values = new CollisionType[width, height];
            SetValues(0, 0, width - 1, height - 1, CollisionType.Empty);
        }
        public CollisionGrid(ref CollisionGrid grid)
        {
            CopyFrom(ref grid);
        }

        public CollisionType GetValue(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                return values[x, y];
            }
            else
            {
                Console.WriteLine("Attempted to access collisionType outside of the grid at " + x.ToString() + "," + y.ToString() + ".");
                Console.WriteLine("Returning CollisionType.Solid");
                return CollisionType.Solid;
            }
        }
        public void SetValue(int x, int y, CollisionType value)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                values[x, y] = value;
            }
            else
            {
                Console.WriteLine("Attempted to set collisionType outside of the grid at " + x.ToString() + "," + y.ToString() + ".");
                Console.WriteLine("Ignoring.");
            }
        }
        public void CopyFrom(ref CollisionGrid grid)
        {
            this.width = grid.Width;
            this.height = grid.Height;
            values = new CollisionType[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    values[x, y] = grid.GetValue(x, y);
                }
            }
        }
        public void SetValues(int x1, int y1, int x2, int y2, CollisionType value)
        {
            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    SetValue(x, y, value);
                }
            }
        }

        //First time trying the Square Bracker operator overload. Hope it works alright
        public CollisionType this[int x, int y]
        {
            get
            {
                return GetValue(x, y);
            }
            set
            {
                SetValue(x, y, value);
            }
        }
    }
}
