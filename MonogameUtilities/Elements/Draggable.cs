using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonogameUtilities.Hitboxes;
using MonogameUtilities.Information;

namespace MonogameUtilities.Elements
{
    public class Draggable : Element
    {
        bool dragging = false;
        Point offsetStart;
        Point dragStart;
        Hitbox DragRegion;
        public int dragScale = 1;
        bool LockMouse;

        public Draggable(int x, int y, int width, int height, IElement parent, bool lockMouseDuringDrag, Hitbox dragRegion) : base(x, y, width, height, parent) 
        { 
            if (dragRegion != null)
            {
                DragRegion = dragRegion;
            }
            else
            {
                DragRegion = Bounds;
            }

            LockMouse = lockMouseDuringDrag;
        }

        public override void Click() { }

        /// <summary>
        /// Function runs at the start of a drag event
        /// </summary>
        /// <param name="pos">Mouse position relative to the screen</param>
        /// <returns>Whether drag event was consumed</returns>
        public override bool DragStart()
        {
            Point mouse = MouseManager.point;

            if (base.DragStart())
            {
                return true;
            }

            if (DragRegion.Contains(mouse))
            {
                SetFocus(this);

                dragging = true;
                offsetStart = mouse;
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
        public override bool DragMid()
        {
            if (dragging)
            {
                Point mouse = MouseManager.point;
                Point dMouse = new(((mouse.X - offsetStart.X) * dragScale), ((mouse.Y - offsetStart.Y) * dragScale));

                Bounds.X += dMouse.X;
                Bounds.Y += dMouse.Y;

                foreach (IElement child in Children)
                {
                    child.AddPos(dMouse);
                }

                // If locking mouse, set mouse position back. Otherwise, set our 'offset' to the current mouse position
                if (LockMouse)
                {
                    Point rounded = MouseManager.GetMouseRounded();
                    Mouse.SetPosition(offsetStart.X * GlobalData.Scale + rounded.X, offsetStart.Y * GlobalData.Scale + rounded.Y);
                    MouseManager.point = offsetStart;
                }
                else
                {
                    offsetStart = mouse;
                }

                return true;
            }
            else
            {
                base.DragMid();
            }

            return false;
        }

        /// <summary>
        /// Function runs at the end of a drag event
        /// </summary>
        /// <param name="pos">Mouse position relative to the screen</param>
        /// <returns>Whether drag event was consumed</returns>
        public override bool DragEnd()
        {
            if (dragging)
            {
                dragging = false;
                Point mouse = MouseManager.point;

                if (dragStart == mouse)
                {
                    Click();
                    return true;
                }

                return true;
            }
            else
            {
                base.DragEnd();
            }

            return false;
        }

        public override void AddPos(Point pos)
        {
            base.AddPos(pos);

            foreach (IElement child in Children)
            {
                child.AddPos(pos);
            }
        }
    }
}
