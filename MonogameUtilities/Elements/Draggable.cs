using Microsoft.Xna.Framework;
using MonogameUtilities.Information;

namespace MonogameUtilities.Elements
{
    public class Draggable : Element
    {
        bool dragging = false;
        Point offsetStart;
        Point dragStart;

        public Draggable(int x, int y, int width, int height, IElement parent) : base(x, y, width, height, parent) { }

        public virtual void Clicked() { }

        /// <summary>
        /// Function runs at the start of a drag event
        /// </summary>
        /// <param name="pos">Mouse position relative to the screen</param>
        /// <returns>Whether drag event was consumed</returns>
        public virtual bool DragStart()
        {
            Microsoft.Xna.Framework.Point mouse = MouseManager.point;

            if (bound.Contains(mouse))
            {
                dragging = true;
                offsetStart = new Point((int)bound.X, (int)bound.Y) - mouse;
                dragStart = mouse;

                return true;
            }
            return false;
        }

        /// <summary>
        /// Function runs during a drag event
        /// </summary>
        /// <param name="pos">Mouse position relative to the screen</param>
        /// <returns>Whether drag event was consumed</returns>
        public virtual bool DragMid()
        {
            if (dragging)
            {
                Point mouse = MouseManager.point;
                bound.X = mouse.X + offsetStart.X;
                bound.Y = mouse.Y + offsetStart.Y;

                return true;
            }
            return false;
        }

        /// <summary>
        /// Function runs at the end of a drag event
        /// </summary>
        /// <param name="pos">Mouse position relative to the screen</param>
        /// <returns>Whether drag event was consumed</returns>
        public virtual bool DragEnd()
        {
            if (dragging)
            {
                dragging = false;
                Point mouse = MouseManager.point;

                if (dragStart == mouse)
                {
                    Clicked();
                    return true;
                }

                return true;
            }
            return false;
        }
    }
}
