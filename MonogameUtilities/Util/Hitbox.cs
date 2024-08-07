using Microsoft.Xna.Framework;
using System;

namespace MonogameUtilities.Util
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

        public Point GetPosition()
        {
            return new(X, Y);
        }

        public void SetPosition(Point pos)
        {
            X = pos.X;
            Y = pos.Y;
        }

        public int RightSide
        {
            get { return X + Width; }
        }

        public int BottomSide
        {
            get { return Y + Height; }
        }

        public Point Position
        {
            get { return new(X, Y); }
        }

        public Point Size
        {
            get { return new(Width, Height); }
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
        /// Shifts this hitbox to fit inside the provided hitbox.<br/>
        /// If the size does not permit the hitbox to fit, the top-left corner will take priority in being within the provided hitbox.
        /// </summary>
        /// <param name="hitbox"></param>
        /// <returns></returns>
        public void ForceContainedBy(Hitbox hitbox)
        {
            X = Math.Min(RightSide, hitbox.RightSide) - Width;
            Y = Math.Min(BottomSide, hitbox.BottomSide) - Height;

            X = Math.Max(hitbox.X, X);
            Y = Math.Max(hitbox.Y, Y);
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

        public int LeftEdgeToHitboxDelta(Hitbox hitbox)
        {
            if (hitbox == null) throw new ArgumentNullException(nameof(hitbox));

            return hitbox.RightSide - X;
        }

        public int RightEdgeToHitboxDelta(Hitbox hitbox)
        {
            if (hitbox == null) throw new ArgumentNullException(nameof(hitbox));

            return hitbox.X - Width - X;
        }

        public int TopEdgeToHitboxDelta(Hitbox hitbox)
        {
            if (hitbox == null) throw new ArgumentNullException(nameof(hitbox));

            return hitbox.BottomSide - Y;
        }

        public int BottomEdgeToHitboxDelta(Hitbox hitbox)
        {
            if (hitbox == null) throw new ArgumentNullException(nameof(hitbox));

            return hitbox.Y - BottomSide;
        }

        public Rectangle GetRectangle(float scale = 1.0f)
        {
            return new(new Point((int)(X * scale), (int)(Y * scale)), new Point((int)(Width * scale), (int)(Height * scale)));
        }

        public static Hitbox FromRectangle(Rectangle rectangle)
        {
            return new Hitbox(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public Hitbox Copy()
        {
            return new Hitbox(X, Y, Width, Height);
        }

        public override string ToString()
        {
            return $"{X} {Y} {Width} {Height}";
        }
    }
}
