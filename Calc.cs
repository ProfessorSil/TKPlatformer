using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKPlatformer
{
    class Calc
    {
        /// <summary>
        /// Rotates the Point Clockwise around the Origin point 
        /// Only clockwise if y+ is down and x+ is right
        /// </summary>
        public static Vector2 RotateAround(Vector2 point, Vector2 origin, float radians)
        {
            Vector2 p = point - origin;

            double rotation = DirectionTo(origin, point) + radians;
            double test = MathHelper.RadiansToDegrees(rotation);

            Vector2 dX = new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));
            Vector2 dY = new Vector2((float)Math.Cos(radians + MathHelper.PiOver2), (float)Math.Sin(radians + MathHelper.PiOver2));

            p = (p.X * dX) + (p.Y * dY);

            p += origin;

            return p;
        }

        /// <summary>
        /// Returns the heading angle from 'from' to 'to' in radians
        /// </summary>
        public static double DirectionTo(Vector2 from, Vector2 to)
        {
            return Math.Atan2(to.Y - from.Y, to.X - from.X);
        }
    }
}
