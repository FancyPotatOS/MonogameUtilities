using Microsoft.Xna.Framework;
using MonogameUtilities.Information;

namespace MonogameUtilities.Elements
{
    /// <summary>
    /// Removed the drag functionality from Draggable
    /// </summary>
    public class Clickable : Draggable
    {
        public Clickable(int x, int y, int width, int height, IElement parent) : base(x, y, width, height, parent) { }

        public override void Clicked() { }

        public override bool DragMid()
        {
            float startX = bound.X;
            float startY = bound.Y;

            bool consumed = base.DragMid();

            bound.X = startX;
            bound.Y = startY;

            return consumed;
        }
    }
}
