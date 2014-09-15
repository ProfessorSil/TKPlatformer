using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace TKPlatformer
{
    /// <summary>
    /// This is a 2D view class to be used in conjunction with 
    /// 'My', 'Game1', and 'Spritebatch' classes made by Taylor
    /// </summary>
    class View
    {
        //TODO: Add renderViewport on screen funcionality

        private GameWindow window;

        #region Variables
        /// <summary>
        /// The current position of the screen (Always at the center)
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// The position the screen is moving towards
        /// </summary>
        public Vector2 positionGoto;
        /// <summary>
        /// The position of the screen last frame
        /// </summary>
        public Vector2 positionLast;
        /// <summary>
        /// 0 = No Auto Movment; 1 = no lag; 2 = half lag;
        /// </summary>
        public double lag;
        /// <summary>
        /// 1 = regular; 2 = 2x zoom in; 0.5 = 2x zoom out;
        /// </summary>
        public double zoom;
        /// <summary>
        /// Rotation of the screen in Degrees
        /// </summary>
        public double rotation;
        /// <summary>
        /// These are for limiting the screen to a certain rectangle; float.MaxValue/MinValue means it is turned off
        /// </summary>
        public float minX, minY, maxX, maxY;
        public bool enableLimits, enableRotation, enableZoom;
        /// <summary>
        /// This will round the position of the camera to whole
        /// numbers when updating. Useful to prevent some weird
        /// rendering inconsistancies
        /// </summary>
        public bool enableRounding;
        /// <summary>
        /// The amount of decimal places it will round the
        /// position to each step
        /// </summary>
        public int numDecimalsToRound;
        #endregion

        #region Properties
        /// <summary>
        /// The size of the view on the screen
        /// </summary>
        public Size SizeScreen
        {
            get
            {
                return window.ClientSize;
            }
        }
        /// <summary>
        /// The size of the view relative to world coordinates
        /// </summary>
        public Vector2 SizeWorld
        {
            get
            {
                return new Vector2(SizeScreen.Width / (float)zoom, SizeScreen.Height / (float)zoom);
            }
        }
        /// <summary>
        /// It will return the left side of the view's x-value
        /// </summary>
        public float LeftWorld
        {
            get
            {
                if (rotation == 0 || enableRotation == false)
                {
                    return position.X - SizeWorld.X / 2f;
                }
                else
                {
                    return Math.Min(TopLeft.X, Math.Min(TopRight.X, Math.Min(BottomLeft.X, BottomRight.X)));
                }
            }
        }
        /// <summary>
        /// It will return the right side of the view's x-value
        /// </summary>
        public float RightWorld
        {
            get
            {
                if (rotation == 0 || enableRotation == false)
                {
                    return position.X + SizeWorld.X / 2f;
                }
                else
                {
                    return Math.Max(TopLeft.X, Math.Max(TopRight.X, Math.Max(BottomLeft.X, BottomRight.X)));
                }
            }
        }
        /// <summary>
        /// It will return the top side of the view's x-value
        /// </summary>
        public float TopWorld
        {
            get
            {
                if (rotation == 0 || enableRotation == false)
                {
                    return position.Y - SizeWorld.Y / 2f;
                }
                else
                {
                    return Math.Min(TopLeft.Y, Math.Min(TopRight.Y, Math.Min(BottomLeft.Y, BottomRight.Y)));
                }
            }
        }
        /// <summary>
        /// It will return the bottom side of the view's x-value
        /// </summary>
        public float BottomWorld
        {
            get
            {
                if (rotation == 0 || enableRotation == false)
                {
                    return position.Y + SizeWorld.Y / 2f;
                }
                else
                {
                    return Math.Max(TopLeft.Y, Math.Max(TopRight.Y, Math.Max(BottomLeft.Y, BottomRight.Y)));
                }
            }
        }
        /// <summary>
        /// Returns the point at the TopLeft corner of the view in world Coordinates
        /// Works with rotation
        /// </summary>
        public Vector2 TopLeft
        {
            get
            {
                if (rotation == 0 || enableRotation == false)
                {
                    return new Vector2(LeftWorld, TopWorld);
                }
                else
                {
                    return Calc.RotateAround(new Vector2(LeftWorld, TopWorld), new Vector2(0, 0), (float)MathHelper.DegreesToRadians(rotation));
                }
            }
        }
        /// <summary>
        /// Returns the point at the TopRight corner of the view in world Coordinates
        /// Works with rotation
        /// </summary>
        public Vector2 TopRight
        {
            get
            {
                if (rotation == 0 || enableRotation == false)
                {
                    return new Vector2(RightWorld, TopWorld);
                }
                else
                {
                    return Calc.RotateAround(new Vector2(RightWorld, TopWorld), new Vector2(0, 0), (float)MathHelper.DegreesToRadians(rotation));
                }
            }
        }
        /// <summary>
        /// Returns the point at the BottomLeft corner of the view in world Coordinates
        /// Works with rotation
        /// </summary>
        public Vector2 BottomLeft
        {
            get
            {
                if (rotation == 0 || enableRotation == false)
                {
                    return new Vector2(LeftWorld, BottomWorld);
                }
                else
                {
                    return Calc.RotateAround(new Vector2(LeftWorld, BottomWorld), new Vector2(0, 0), (float)MathHelper.DegreesToRadians(rotation));
                }
            }
        }
        /// <summary>
        /// Returns the point at the BottomRight corner of the view in world Coordinates
        /// Works with rotation
        /// </summary>
        public Vector2 BottomRight
        {
            get
            {
                if (rotation == 0 || enableRotation == false)
                {
                    return new Vector2(RightWorld, BottomWorld);
                }
                else
                {
                    return Calc.RotateAround(new Vector2(RightWorld, BottomWorld), new Vector2(0, 0), (float)MathHelper.DegreesToRadians(rotation));
                }
            }
        }
        #endregion

        #region Enables, Sets, and other helper functions
        public void EnableRotation(bool on = true)
        {
            enableRotation = on;
        }
        public void EnableZoom(bool on = true)
        {
            enableZoom = on;
        }
        public void EnableLimits(bool on = true, float minX = float.MinValue, float minY = float.MinValue, float maxX = float.MaxValue, float maxY = float.MaxValue)
        {
            enableLimits = on;
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }
        public void SetLag(double value)
        {
            this.lag = value;
        }
        public void SetRotation(double value)
        {
            this.rotation = value;
            this.enableRotation = true;
        }
        /// <summary>
        /// Rotates the camera set amount clockwise or counter-clockwise
        /// </summary>
        /// <param name="clockWise">Assuming x+ is right and y+ is down </param>
        public void Rotate(double addValue, bool clockWise = true)
        {
            this.rotation += (clockWise ? (1) : (-1)) * addValue;
        }
        public void SetZoom(double value)
        {
            this.zoom = value;
            this.enableZoom = true;
        }
        public void SetLimits(float minX, float minY, float maxX, float maxY)
        {
            enableLimits = true;
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }
        /// <summary>
        /// This sets the 'positionGoto' not the position.
        /// This means the camera will still interpolate from where it was
        /// </summary>
        public void SetTarget(Vector2 value)
        {
            this.positionGoto = value;
        }
        /// <summary>
        /// This sets both the Position and PositionGoto
        /// </summary>
        public void SetPosition(Vector2 value)
        {
            this.positionGoto = value;
            this.position = value;
        }
        #endregion

        public View(GameWindow window, Vector2 start, double lag = 1.0, double zoom = 1.0, double rotation = 0.0)
        {
            this.window = window;

            this.position = start;
            this.positionGoto = start;
            this.positionLast = start;
            this.lag = lag;
            this.zoom = zoom;
            this.rotation = rotation;

            this.minX = float.MinValue;
            this.minY = float.MinValue;
            this.maxX = float.MaxValue;
            this.maxY = float.MaxValue;

            enableLimits = false;
            enableRotation = true;
            enableZoom = true;
            enableRounding = false;
            numDecimalsToRound = 0;
        }

        /// <summary>
        /// Should be called after the camera values have 
        /// been changed to desired values for game
        /// </summary>
        public void Update()
        {
            if (lag == 1)
            {
                position = positionGoto;
            }
            else
            {
                position += (positionGoto - position) / (float)lag;
            }

            if (enableLimits)
            {
                CheckLimits();
            }

            if (enableRounding)
            {
                position = new Vector2((float)Math.Round(position.X, 0), (float)Math.Round(position.Y, 0));
            }
        }
        /// <summary>
        /// Checks and rectifies if the view is outside the defined min and max values for position
        /// </summary>
        public void CheckLimits()
        {
            if (rotation == 0 || enableRotation == false)
            {
                if (minX != float.MinValue)
                {
                    if (LeftWorld < minX)
                    {
                        position.X = minX + SizeWorld.X / 2f;
                    }
                }
                if (minY != float.MinValue)
                {
                    if (TopWorld < minY)
                    {
                        position.Y = minY + SizeWorld.Y / 2f;
                    }
                }
                if (maxX != float.MaxValue)
                {
                    if (RightWorld > maxX)
                    {
                        position.X = maxX - SizeWorld.X / 2f;
                    }
                }
                if (maxY != float.MaxValue)
                {
                    if (BottomWorld > maxY)
                    {
                        position.Y = maxY - SizeWorld.Y / 2f;
                    }
                }

                //We need to handle if the bounds are smaller than the view size
                if (minX != float.MinValue && maxX != float.MaxValue && maxX - minX <= SizeWorld.X)
                {
                    //Center the position X between minX and maxX
                    position.X = (maxX + minX) / 2f;
                }
                if (minY != float.MinValue && maxY != float.MaxValue && maxY - minY <= SizeWorld.Y)
                {
                    //Center the position Y between minY and maxY
                    position.Y = (maxY + minY) / 2f;
                }
            }
            else
            {
                Vector2 tl = TopLeft, tr = TopRight, bl = BottomLeft, br = BottomRight;
                float mostX = Math.Max(tl.X, Math.Max(tr.X, Math.Max(bl.X, br.X)));
                float leastX = Math.Min(tl.X, Math.Min(tr.X, Math.Min(bl.X, br.X)));
                float mostY = Math.Max(tl.Y, Math.Max(tr.Y, Math.Max(bl.Y, br.Y)));
                float leastY = Math.Min(tl.Y, Math.Min(tr.Y, Math.Min(bl.Y, br.Y)));

                if (minX != float.MinValue)
                {
                    if (leastX < minX)
                    {
                        position.X = position.X + (minX - leastX);
                    }
                }
                if (minY != float.MinValue)
                {
                    if (leastY < minY)
                    {
                        position.Y = position.Y + (minY - leastY);
                    }
                }
                if (maxX != float.MaxValue)
                {
                    if (mostX > maxX)
                    {
                        position.X = position.X - (mostX - maxX);
                    }
                }
                if (maxY != float.MaxValue)
                {
                    if (mostY > maxY)
                    {
                        position.Y = position.Y - (mostY - maxY);
                    }
                }

                //We need to handle if the bounds are smaller than the view size
                if (minX != float.MinValue && maxX != float.MaxValue && maxX - minX <= mostX - leastX)
                {
                    //Center the position X between minX and maxX
                    position.X = (maxX + minX) / 2f;
                }
                if (minY != float.MinValue && maxY != float.MaxValue && maxY - minY <= mostY - leastY)
                {
                    //Center the position Y between minY and maxY
                    position.Y = (maxY + minY) / 2f;
                }
            }
        }

        /// <summary>
        /// Call this function on update to enable basic camera movemtn with WASD/Arrow keys
        /// </summary>
        /// <param name="speed">Spped at which the camera will move each step</param>
        /// <param name="WASDKeys">true = WASD keys; false = Arrow Keys;</param>
        /// <param name="dependantOnZoom">If true then speed will be scaled depending on zoom value</param>
        public void BasicMovement(float speed, bool WASDKeys = true, bool dependantOnZoom = false)
        {
            float realSpeed = speed * (dependantOnZoom ? (1.0f / (float)zoom) : (1.0f));

            if (WASDKeys)
            {
                if (My.KeyDown(Key.W))
                {
                    positionGoto.Y -= realSpeed;
                }
                if (My.KeyDown(Key.S))
                {
                    positionGoto.Y += realSpeed;
                }
                if (My.KeyDown(Key.A))
                {
                    positionGoto.X -= realSpeed;
                }
                if (My.KeyDown(Key.D))
                {
                    positionGoto.X += realSpeed;
                }
            }
            else
            {
                if (My.KeyDown(Key.Up))
                {
                    positionGoto.Y -= realSpeed;
                }
                if (My.KeyDown(Key.Down))
                {
                    positionGoto.Y += realSpeed;
                }
                if (My.KeyDown(Key.Left))
                {
                    positionGoto.X -= realSpeed;
                }
                if (My.KeyDown(Key.Right))
                {
                    positionGoto.X += realSpeed;
                }
            }
        }

        /// <summary>
        /// This should be called just before drawing in OpenGL
        /// It applies the values of this class to the transform
        /// matrix OpenGL uses to draw everything
        /// </summary>
        public void ApplyTransform()
        {
            Matrix4 world = Matrix4.Identity;

            world = Matrix4.Mult(world, Matrix4.CreateTranslation(-position.X, -position.Y, 0));

            if (enableRotation)
                world = Matrix4.Mult(world, Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(rotation)));

            float aspectRatio = (float)window.Height / (float)window.Width;
            float aspectRatio2 = (float)window.Width / (float)window.Height;

            if (enableZoom)
                world = Matrix4.Mult(world, Matrix4.CreateScale((float)zoom, (float)zoom, 0.0f));

            GL.MultMatrix(ref world);
        }
    }
}
