using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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

        /// <summary>
        /// Does a continuous collision check between two Rectangles (one of them moving, other static)
        /// </summary>
        /// <param name="staticRec">The static rectangle</param>
        /// <param name="moveRec">The moving rectangle (with 'velocity')</param>
        /// <param name="velocity">Velocity of the moving rectangle</param>
        /// <param name="solution">The rectangle representing where moveRec should be at the end of the step</param>
        /// <param name="isPlatform">True = only collide with top of statRec</param>
        /// <returns>whether they collided</returns>
        public static bool SweepTest(RectangleF staticRec, RectangleF moveRec, Vector2 velocity, out RectangleF solution, bool isPlatform = false)
        {
            RectangleF startRec = moveRec;
            RectangleF endRec = new RectangleF(moveRec.X + velocity.X, moveRec.Y + velocity.Y, moveRec.Width, moveRec.Height);
            RectangleF unionRec = RectangleF.Union(startRec, endRec);

            if (!staticRec.IntersectsWith(unionRec))
            {
                // Static rectangle was nowhere inside the area we wanted to move through
                solution = endRec;
                return false;
            }

            #region JointX
            //displacement along the x-axis where the two rectangle will begin to be joined
            float jointX = 0;
            if (velocity.X > 0)
                jointX = staticRec.Left - startRec.Right;
            else
                //We make it negative because the displacement is in the -x direction
                jointX = -(startRec.Left - staticRec.Right);
            #endregion

            #region JointY
            //displacement along the y-axis where the two rectangle will begin to be joined
            float jointY = 0;
            if (velocity.Y > 0)
                jointY = staticRec.Top - startRec.Bottom;
            else
                //We make it negative because the displacement is in the -y direction
                jointY = -(startRec.Top - staticRec.Bottom);
            #endregion

            #region MaxJointTime
            //Find the 'times' at which the this joints occur
            //By time i mean 0-1 where 0 is begining of step and 1 is end of step with
            //full velocity added
            double jointTimeX = jointX / velocity.X;
            double jointTimeY = jointY / velocity.Y;

            if ((jointTimeX < 0 && jointTimeY < 0) || (jointTimeX > 1 && jointTimeY > 1))
            {
                //continuing this velocity will run into it eventually but not in this step
                //or the times are in the past (we are moving away from staticRec)
                solution = endRec;
                return false;
            }

            //This will hold the joint time that happens last/second
            //It will tell us the absolute latest they could be joined on both axises
            double maxJointTime;
            if (double.IsInfinity(jointTimeX) || double.IsInfinity(jointTimeY))
            {
                if (double.IsInfinity(jointTimeX) && !double.IsInfinity(jointTimeY))
                    maxJointTime = jointTimeY;
                else if (double.IsInfinity(jointTimeY) && !double.IsInfinity(jointTimeX))
                    maxJointTime = jointTimeX;
                else
                {
                    //Both joint times were infinite
                    //Usually happens if velocity was 0,0
                    solution = endRec;
                    return false;
                }
            }
            else
            {
                //Neither were infinite so take the greater one
                maxJointTime = Math.Max(jointTimeX, jointTimeY);
            }
            #endregion

            #region DisjointX
            //Now do the same process but for when they will 'disjoint'
            float disjointX = 0;
            if (velocity.X > 0)
                disjointX = staticRec.Right - startRec.Left;
            else
                disjointX = -(startRec.Right - staticRec.Left);
            #endregion

            #region DisjointY
            float disjointY = 0;
            if (velocity.Y > 0)
                disjointY = staticRec.Bottom - startRec.Top;
            else
                disjointY = -(startRec.Bottom - staticRec.Top);
            #endregion

            #region MinDisjointTime
            double disjointTimeX = disjointX / velocity.X;
            double disjointTimeY = disjointY / velocity.Y;

            //This time we want to know the earliest they will disjoint
            double minDisjointTime;
            if (double.IsInfinity(disjointTimeX) || double.IsInfinity(disjointTimeY))
            {
                if (double.IsInfinity(disjointTimeX) && !double.IsInfinity(disjointTimeY))
                    minDisjointTime = disjointTimeY;
                else if (double.IsInfinity(disjointTimeY) && !double.IsInfinity(disjointTimeX))
                    minDisjointTime = disjointTimeX;
                else
                {
                    //Both disjoint times were infinite
                    //Usually happens if velocity was 0,0
                    //We should never really hit this since it should 
                    //have been handled when going through jointTimes
                    solution = endRec;
                    return false;
                }
            }
            else
            {
                //Neither were infinite so take the lesser one
                minDisjointTime = Math.Min(disjointTimeX, disjointTimeY);
            }
            #endregion


            if (minDisjointTime < maxJointTime)
            {
                //One of the axises disjointed before both were joined
                //No overlap of joint periods from both axises
                solution = endRec;
                return false;
            }

            //We have a solution if we got here
            Vector2 solEndPosition = new Vector2(startRec.X, startRec.Y) + (velocity * (float)maxJointTime);

            if (isPlatform)
            {
                //We need to make sure it was a top collision
                #region Platform Handling
                if (jointTimeX > jointTimeY && !double.IsInfinity(jointTimeX))
                {
                    //We collided with either left or right face
                    solution = endRec;
                    return false;
                }

                if (jointTimeY > jointTimeX && !double.IsInfinity(jointTimeY) && velocity.Y < 0)
                {
                    //We collided with the bottom
                    solution = endRec;
                    return false;
                }

                if (double.IsInfinity(jointTimeY))
                {
                    solution = endRec;
                    return false;
                }
                if (double.IsInfinity(jointTimeX) && velocity.Y <= 0)
                {
                    solution = endRec;
                    return false;
                }
                #endregion
            }

            #region Rounding Solution 
            //I added this so the edge values of the static and moving recs
            //will align if there was a collision
            //It helps when I want to know what blocks I am immediately next
            //to after the collision
            if (jointTimeX > jointTimeY && !double.IsInfinity(jointTimeX))
            {
                //Collision was in the x direction
                if (velocity.X > 0)
                    solEndPosition.X = staticRec.Left - startRec.Width;
                else if (velocity.X < 0)
                    solEndPosition.X = staticRec.Right;
            }
            if (jointTimeY > jointTimeX && !double.IsInfinity(jointTimeY))
            {
                //Collision was in the y direction
                if (velocity.Y > 0)
                    solEndPosition.Y = staticRec.Top - startRec.Height;
                else if (velocity.Y < 0)
                    solEndPosition.Y = staticRec.Bottom;
            }
            #endregion

            solution = new RectangleF(solEndPosition.X, solEndPosition.Y, startRec.Width, startRec.Height);
            return true;
        }
    }
}
