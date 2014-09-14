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

namespace TKPlatformer
{
    struct RectF
    {
        //TODO: Add this to OpenTKTest project
        private Vector2 position;
        private Vector2 size;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }
        public float Width
        {
            get { return size.X; }
            set { size.X = value; }
        }
        public float Height
        {
            get { return size.Y; }
            set { size.Y = value; }
        }
        public float Left
        {
            get { return position.X; }
            set { position.X = value; }
        }
        public float Top
        {
            get { return position.Y; }
            set { position.Y = value; }
        }
        public float Right
        {
            get { return position.X + size.X; }
            set { position.X = value - size.X; }
        }
        public float Bottom
        {
            get { return position.Y + size.Y; }
            set { position.Y = value - size.Y; }
        }
        public Vector2 Center
        {
            get { return position + size / 2f; }
            set { position = value - size / 2f; }
        }
        public Vector2 TopLeft
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 TopRight
        {
            get { return new Vector2(position.X + size.X, position.Y); }
            set { position = new Vector2(value.X - size.X, value.Y); ; }
        }
        public Vector2 BottomLeft
        {
            get { return new Vector2(position.X, position.Y + size.Y); }
            set { position = new Vector2(value.X, value.Y - size.Y); ; }
        }
        public Vector2 BottomRight
        {
            get { return new Vector2(position.X + size.X, position.Y + size.Y); }
            set { position = new Vector2(value.X - size.X, value.Y - size.Y); ; }
        }

        public RectF(float x, float y, float w, float h)
        {
            this.position = new Vector2(x, y);
            this.size = new Vector2(w, h);
        }
        public RectF(Vector2 position, float w, float h)
        {
            this.position = position;
            this.size = new Vector2(w, h);
        }
        public RectF(Vector2 p1, Vector2 p2)
        {
            this.position = new Vector2(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
            Vector2 max = new Vector2(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
            this.size = max - position;
        }

        public static RectF Translate(RectF rec, Vector2 amount)
        {
            return RectF.Translate(rec, amount.X, amount.Y);
        }
        public static RectF Translate(RectF rec, float x, float y)
        {
            return new RectF(rec.position.X + x, rec.position.Y + y, rec.Width, rec.Height);
        }
        public void Translate(Vector2 amount)
        {
            Translate(amount.X, amount.Y);
        }
        public void Translate(float x, float y)
        {
            position.X += x;
            position.Y += y;
        }

        public static RectF Combine(RectF rec1, RectF rec2)
        {
            float left = Math.Min(rec1.Left, rec2.Left);
            float top = Math.Min(rec1.Top, rec2.Top);
            float right = Math.Max(rec1.Right, rec2.Right);
            float bottom = Math.Max(rec1.Bottom, rec2.Bottom);
            return new RectF(left, top, right - left, bottom - top);
        }
        public RectF Combine(RectF rec)
        {
            float left = Math.Min(this.Left, rec.Left);
            float top = Math.Min(this.Top, rec.Top);
            float right = Math.Max(this.Right, rec.Right);
            float bottom = Math.Max(this.Bottom, rec.Bottom);
            return new RectF(left, top, right - left, bottom - top);
        }

    }
}
