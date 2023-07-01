using Microsoft.Xna.Framework;
using MonogameUtilities.Information;

namespace MonogameUtilities.Elements
{
    /// <summary>
    /// Removed the drag functionality from Draggable
    /// </summary>
    public class Clickable : Draggable
    {
        public Clickable(int x, int y, int width, int height, IElement parent) : base(x, y, width, height, parent, false, null) { }

        public override void Click() { }

        public override bool DragMid()
        {
            int startX = Bounds.X;
            int startY = Bounds.Y;

            bool consumed = base.DragMid();

            Bounds.X = startX;
            Bounds.Y = startY;

            return consumed;
        }
    }
}
