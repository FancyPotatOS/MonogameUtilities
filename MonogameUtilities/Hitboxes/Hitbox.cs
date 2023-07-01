using Microsoft.Xna.Framework;

namespace MonogameUtilities.Hitboxes
{
    public class Hitbox
    {
        public int X, Y, Width, Height;

        public Hitbox() { }

        public Hitbox(int x, int y, int w, int h) 
        {
            X = x;
            Y = y;
            Width = w; 
            Height = h;
        }

        public Point GetPoint()
        {
            return new Point(X, Y);
        }

        public int RightSide
        {
            get { return X + Width; }
        }

        public int BottomSide
        {
            get { return Y + Height; }
        }

        /// <summary>
        /// Shifts hitbox position by point
        /// </summary>
        /// <param name="pos"></param>
        public void ShiftBy(Point pos)
        {
            X += pos.X;
            Y += pos.Y;
        }

        /// <summary>
        /// Whether the provided hitbox is contained
        /// </summary>
        /// <param name="hitbox">Hitbox that is contained</param>
        public bool Contains(Hitbox hitbox)
        {
            return X <= hitbox.X && hitbox.RightSide <= RightSide && Y <= hitbox.Y && hitbox.BottomSide <= BottomSide;
        }

        /// <summary>
        /// Whether the provided point is contained
        /// </summary>
        /// <param name="hitbox">Point that is contained</param>
        public bool Contains(Point point)
        {
            return X <= point.X && point.X <= RightSide && Y <= point.Y && point.Y <= BottomSide;
        }

        /// <summary>
        /// Whether the provided hitbox is strictly contained, and not on the edges
        /// </summary>
        /// <param name="hitbox">Hitbox that is contained</param>
        public bool StrictlyContained(Hitbox hitbox)
        {
            return X < hitbox.X && hitbox.RightSide < RightSide && Y < hitbox.Y && hitbox.BottomSide < BottomSide;
        }

        /// <summary>
        /// Whether the provided point is strictly contained, and not on the edges
        /// </summary>
        /// <param name="hitbox">Point that is contained</param>
        public bool StrictlyContained(Point point)
        {
            return X < point.X && point.X < RightSide && Y < point.Y && point.Y < BottomSide;
        }

        /// <summary>
        /// Whether the provided hitbox is outside
        /// </summary>
        /// <param name="hitbox">Hitbox that is outside</param>
        public bool Outside(Hitbox hitbox)
        {
            return hitbox.RightSide <= X || RightSide <= hitbox.X || hitbox.BottomSide <= Y || BottomSide <= hitbox.Y;
        }

        /// <summary>
        /// Whether the provided point is outside
        /// </summary>
        /// <param name="hitbox">Point that is outside</param>
        public bool Outside(Point point)
        {
            return point.X <= X || RightSide <= point.X || point.Y <= Y || BottomSide <= point.Y;
        }

        /// <summary>
        /// Whether the provided hitbox is strictly outside, and not on the edges
        /// </summary>
        /// <param name="hitbox">Hitbox that is outside</param>
        public bool StrictOutside(Hitbox hitbox)
        {
            return hitbox.RightSide < X || RightSide < hitbox.X || hitbox.BottomSide < Y || BottomSide < hitbox.Y;
        }

        /// <summary>
        /// Whether the provided point is strictly outside, and not on the edges
        /// </summary>
        /// <param name="hitbox">Point that is outside</param>
        public bool StrictOutside(Point point)
        {
            return point.X < X || RightSide < point.X || point.Y < Y || BottomSide < point.Y;
        }

        public Rectangle AsRectangle()
        {
            return new Rectangle(X, Y, Width, Height);
        }
    }
}
