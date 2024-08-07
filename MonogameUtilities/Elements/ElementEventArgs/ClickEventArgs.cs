

using System;

namespace MonogameUtilities.Elements.ElementEventArgs
{
    public class ClickEventArgs : EventArgs
    {
        public int X { get; }
        public int Y { get; }

        public bool LeftMouse { get; }
        public bool MiddleMouse { get; }
        public bool RightMouse { get; }

        public ClickEventArgs(int x, int y, bool left, bool middle, bool right)
        {
            X = x;
            Y = y;

            LeftMouse = left;
            MiddleMouse = middle;
            RightMouse = right;
        }
    }
}
