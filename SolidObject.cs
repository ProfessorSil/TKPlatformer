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

    class SolidObject
    {
        public string name;
        public Vector2 position, velocity, size;
        public Vector2 gravity;
        public bool solid;
        public List<Solid.CollisionBlock> blocks;
        private bool colLeft, colRight, colBottom, colTop;

        public RectangleF ColRec
        {
            get
            {
                return new RectangleF(position.X, position.Y, size.X, size.Y);
            }
        }
        public bool ColLeft
        {
            get { return colLeft; }
        }
        public bool ColRight
        {
            get { return colRight; }
        }
        public bool ColBottom
        {
            get { return colBottom; }
        }
        public bool IsOnGround
        {
            get { return colBottom; }
        }
        public bool ColTop
        {
            get { return colTop; }
        }

        public SolidObject(string name, Vector2 position, Vector2 size, Vector2 gravity, bool solid = true)
        {
            this.name = name;
            this.position = position;
            this.size = size;
            this.gravity = gravity;
            this.solid = solid;
            this.velocity = Vector2.Zero;
            blocks = new List<Solid.CollisionBlock>();
        }

        public virtual void Update(ref Map map)
        {
            if (!IsOnGround)
                velocity += gravity;

            SweepMove(ref map, velocity);
        }

        /// <summary>
        /// Moves this solid object using Calc.SweepTest to take
        /// into account the map collisionTypes grid
        /// </summary>
        private void SweepMove(ref Map map, Vector2 amount)
        {
            RectangleF startRec = ColRec;
            position += amount;
            RectangleF endRec = ColRec;
            position -= amount;
            RectangleF uni = RectangleF.Union(startRec, endRec);
            int minX = Math.Max(0, (int)Math.Floor(uni.Left / (float)map.GridSize));
            int maxX = Math.Min(map.Width - 1, (int)Math.Ceiling(uni.Right / (float)map.GridSize));
            int minY = Math.Max(0, (int)Math.Floor(uni.Top / (float)map.GridSize));
            int maxY = Math.Min(map.Height - 1, (int)Math.Ceiling(uni.Bottom / (float)map.GridSize));

            Vector2 solution = amount;
            bool hitBlock = false;
            #region Loop Through Tiles
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    RectangleF tileRec = map.GetColRec(x, y);
                    if (map.colGrid[x, y] == CollisionGrid.CollisionType.Solid
                        || map.colGrid[x,y] == CollisionGrid.CollisionType.Platform)
                    {
                        RectangleF solutionRec = new RectangleF();
                        hitBlock = (hitBlock ||
                            Calc.SweepTest(tileRec, startRec, amount, out solutionRec, map.colGrid[x, y] == CollisionGrid.CollisionType.Platform));
                        if (!hitBlock)
                        {
                            continue;
                        }
                        else
                        {
                            if (Math.Abs(solutionRec.Left - startRec.Left) < Math.Abs(solution.X))
                            {
                                //We found a closer solution along the X-axis
                                solution.X = solutionRec.Left - startRec.Left;
                            }
                            if (Math.Abs(solutionRec.Top - startRec.Top) < Math.Abs(solution.Y))
                            {
                                //We found a closer solution along the Y-axis
                                solution.Y = solutionRec.Top - startRec.Top;
                            }
                        }
                    }
                }
            }
            #endregion

            position += solution;

            //This region does the calls to AddCollisionBlock
            #region Update CollisionBlocks
            blocks = new List<Solid.CollisionBlock>();
            colLeft = false; colRight = false; colTop = false; colBottom = false;

            //re-using the old min and max variables
            minX = Math.Max(0, (int)Math.Floor(ColRec.Left / (float)map.GridSize) - 1);
            maxX = Math.Min(map.Width - 1, (int)Math.Ceiling(ColRec.Right / (float)map.GridSize) + 1);
            minY = Math.Max(0, (int)Math.Floor(ColRec.Top / (float)map.GridSize) - 1);
            maxY = Math.Min(map.Height - 1, (int)Math.Ceiling(ColRec.Bottom / (float)map.GridSize) + 1);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    bool isPlatform = map.colGrid[x, y] == CollisionGrid.CollisionType.Platform;
                    if (map.colGrid[x, y] == CollisionGrid.CollisionType.Solid || isPlatform)
                    {
                        bool sTop = map.colGrid.GetValue(x, y - 1) == CollisionGrid.CollisionType.Solid;
                        bool sBottom = map.colGrid.GetValue(x, y + 1) == CollisionGrid.CollisionType.Solid;
                        bool sLeft = map.colGrid.GetValue(x - 1, y) == CollisionGrid.CollisionType.Solid;
                        bool sRight = map.colGrid.GetValue(x + 1, y) == CollisionGrid.CollisionType.Solid;
                        RectangleF tileRec = map.GetColRec(x, y);
                        bool checkX = (ColRec.Right >= tileRec.Left && ColRec.Left <= tileRec.Right);
                        bool checkY = (ColRec.Bottom >= tileRec.Top && ColRec.Top <= tileRec.Bottom);

                        if (tileRec.Top == ColRec.Bottom && checkX && !sTop)
                        {
                            AddCollisionBlock(new Solid.CollisionBlock(x, y, Solid.Side.Bottom, ref map));
                        }
                        if (!isPlatform && tileRec.Bottom == ColRec.Top && checkX && !sBottom)
                        {
                            AddCollisionBlock(new Solid.CollisionBlock(x, y, Solid.Side.Top, ref map));
                        }
                        if (!isPlatform && tileRec.Left == ColRec.Right && checkY && !sLeft)
                        {
                            AddCollisionBlock(new Solid.CollisionBlock(x, y, Solid.Side.Right, ref map));
                        }
                        if (!isPlatform && tileRec.Right == ColRec.Left && checkY && !sRight)
                        {
                            AddCollisionBlock(new Solid.CollisionBlock(x, y, Solid.Side.Left, ref map));
                        }
                    }
                }
            }
            #endregion
        }

        private void AddCollisionBlock(Solid.CollisionBlock block)
        {
            blocks.Add(block);
            if (block.side == Solid.Side.Bottom)
            {
                colBottom = true;
                if (velocity.Y > 0)
                    velocity.Y = 0;
            }
            if (block.side == Solid.Side.Top)
            {
                colTop = true;
                if (velocity.Y < 0)
                    velocity.Y = 0;
            }
            if (block.side == Solid.Side.Right)
            {
                colRight = true;
                if (velocity.X > 0)
                    velocity.X = 0;
            }
            if (block.side == Solid.Side.Left)
            {
                colLeft = true;
                if (velocity.X < 0)
                    velocity.X = 0;
            }
        }
        public List<Solid.CollisionBlock> GetBlocksOnSide(Solid.Side side)
        {
            List<Solid.CollisionBlock> bs = new List<Solid.CollisionBlock>();
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].side == side)
                    bs.Add(blocks[i]);
            }
            return bs;
        }
        public bool IsCollidingOn(Solid.Side side)
        {
            if (side == Solid.Side.Bottom)
                return colBottom;
            else if (side == Solid.Side.Top)
                return colTop;
            else if (side == Solid.Side.Left)
                return colLeft;
            else
                return colRight;
        }

        public virtual void Draw()
        {
            Spritebatch.DrawRectangle(ColRec, IsOnGround ? Color.Purple : Color.Blue);
        }
    }
}

namespace TKPlatformer.Solid
{
    enum Side
    {
        Top, Bottom, Left, Right
    }

    struct CollisionBlock
    {
        public Point gridPos;
        public Side side;
        public CollisionGrid.CollisionType type;
        public RectangleF ColRec;

        public int X
        {
            get { return gridPos.X; }
        }
        public int Y
        {
            get { return gridPos.Y; }
        }

        /// <summary>
        /// Will automatically detect which side and type
        /// using functions in Map and SolidObject
        /// </summary>
        public CollisionBlock(int x, int y, Side side, ref Map map)
        {
            this.gridPos = new Point(x, y);
            this.ColRec = map.GetColRec(x, y);
            this.type = map.colGrid[x, y];
            this.side = side;
        }
    }
}
