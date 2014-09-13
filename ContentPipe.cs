using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace TKPlatformer
{
    class ContentPipe
    {

        public static Texture2D LoadTexture(string filename, bool pixelated = true)
        {
            if (String.IsNullOrEmpty(filename))
            {
                throw new ArgumentException(filename);
                return new Texture2D();
            }

            if (!System.IO.File.Exists(filename))
            {
                throw new ArgumentException(filename);
                return new Texture2D();
            }

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(filename);
            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                bmpData.Width, bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, bmpData.Scan0);

            Texture2D tex = new Texture2D(filename, id, bmp.Width, bmp.Height);

            bmp.UnlockBits(bmpData);

            //We haven't unloaded mipmaps, so disavle mipmapping (otherwise the texture will not appear).
            //On newer video cards, we can use GL.GenerateMipmaps() of GL.Ext.GenerateMipmaps() to create
            //mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, pixelated ? (int)TextureMinFilter.Nearest : (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, pixelated ? (int)TextureMinFilter.Nearest : (int)TextureMinFilter.Linear);

            return tex;
        }
    }
}
