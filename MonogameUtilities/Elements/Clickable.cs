using Microsoft.Xna.Framework;
using MonogameUtilities.Information;
using System.Collections.Generic;

namespace MonogameUtilities.Elements
{
    /// <summary>
    /// Removed the drag functionality from Draggable
    /// </summary>
    public class Clickable : Draggable
    {
        List<IClickListener> clickListeners;

        public Clickable(int x, int y, int width, int height, IElement parent) : base(x, y, width, height, parent, false, null) 
        {
            clickListeners = new();
        }

        public void AddClickListener(IClickListener icl)
        {
            clickListeners.Add(icl);
        }

        public void RemoveClickListener(IClickListener icl)
        {
            clickListeners.Remove(icl);
        }

        public override void Click() 
        {
            foreach (IClickListener icl in clickListeners)
            {
                icl.OnClick(this);
            }
        }

        public override bool DragMid()
        {
            int startX = Bounds.X;
            int startY = Bounds.Y;

            bool consumed = base.DragMid();

            Bounds.X = startX;
            Bounds.Y = startY;

            return consumed;
        }

        public override bool DragEnd()
        {
            Point mousePos = MouseManager.point;

            bool consumed = base.DragEnd();

            if (consumed && GetBounds().Contains(mousePos))
            {
                foreach (IClickListener icl in clickListeners)
                {
                    icl.OffClick(this);
                }
            }

            return consumed;
        }
    }
}
