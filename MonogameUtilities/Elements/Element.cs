

using Microsoft.Xna.Framework;
using MonogameUtilities.Hitboxes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonogameUtilities.Elements
{
    public class Element : IElement
    {
        public Hitbox Bounds;
        public IElement Parent;
        public List<IElement> Children;
        public int Layer;
        public int Margin;

        public int TopLayer { get; set; }

        public Element(int x, int y, int width, int height, IElement parent, int margin = 0)
        {
            Bounds = new Hitbox(x, y, width, height);
            if (parent != null)
            {
                SetParent(parent);
                SetPos(parent.GetPos());
            }
            Children = new List<IElement>();

            Layer = 0;
            Margin = margin;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Whether should be removed from parent</returns>
        public virtual bool Update() 
        {
            foreach (IElement child in Children.OrderBy(element => element.GetLayer()).Reverse())
            {
                if (child.Update())
                {
                    return true;
                }
            }
            return false; 
        }

        public virtual void Draw()
        {
            foreach (IElement child in Children.OrderBy(element => element.GetLayer()).Reverse())
            {
                child.Draw();
            }
        }
        public virtual void Click() { }
        public virtual bool DragStart() 
        {
            foreach (IElement child in Children.OrderBy(element => element.GetLayer()).Reverse())
            {
                if (child.DragStart())
                {
                    return true;
                }
            }
            return false;
        }
        public virtual bool DragMid()
        {
            foreach (IElement child in Children.OrderBy(element => element.GetLayer()).Reverse())
            {
                if (child.DragMid())
                {
                    return true;
                }
            }

            return false; 
        }
        public virtual bool DragEnd()
        {
            foreach (IElement child in Children.OrderBy(element => element.GetLayer()).Reverse())
            {
                if (child.DragEnd())
                {
                    return true;
                }
            }
            return false;
        }

        public virtual int GetLayer() { return Layer; }

        public virtual Hitbox GetBounds() { return Bounds; }

        public virtual void Bound(Hitbox bounding)
        {
            if (!bounding.Contains(Bounds))
            {
                int dX = Math.Max(Bounds.X, bounding.X) - Bounds.X;
                int dY = Math.Max(Bounds.Y, bounding.Y) - Bounds.Y;
                Bounds.X = Math.Max(Bounds.X, bounding.X);
                Bounds.Y = Math.Max(Bounds.Y, bounding.Y);

                if (bounding.RightSide < Bounds.RightSide)
                {
                    dX = bounding.RightSide - Bounds.Width - Bounds.X;
                    Bounds.X = bounding.RightSide - Bounds.Width;
                }
                if (bounding.BottomSide < Bounds.BottomSide)
                {
                    dY = bounding.BottomSide - Bounds.Height - Bounds.Y;
                    Bounds.Y = bounding.BottomSide - Bounds.Height;
                }

                foreach (IElement child in Children)
                {
                    child.AddPos(new Point(dX, dY));
                }
            }
        }

        /// <summary>
        /// Ensure this bounding hitbox intersects the given hitbox
        /// </summary>
        /// <param name="bounding">Hitbox to intersect with</param>
        public virtual void Intersect(Hitbox bounding, int margin)
        {
            Hitbox boundWithMargins = new Hitbox(Bounds.X + margin, Bounds.Y + margin, Bounds.Width - 2 * margin, Bounds.Height - 2 * margin);
            if (boundWithMargins.Outside(bounding))
            {
                int dX = 0;
                int dY = 0;
                if (bounding.RightSide <= Bounds.X + margin)
                {
                    dX = bounding.RightSide - margin - Bounds.X;
                }
                else if (Bounds.RightSide - margin <= bounding.X)
                {
                    dX = bounding.X + margin - Bounds.Width - Bounds.X;
                }
                if (bounding.BottomSide <= Bounds.Y + margin)
                {
                    dY = bounding.BottomSide - margin - Bounds.Y;
                }
                else if (Bounds.BottomSide - margin <= bounding.Y)
                {
                    dY = bounding.Y + margin - Bounds.Height - Bounds.Y;
                }

                Bounds.X += dX;
                Bounds.Y += dY;

                foreach (IElement child in Children)
                {
                    child.AddPos(new Point(dX, dY));
                }
            }
        }

        public virtual Point GetPos()
        {
            return new Point(Bounds.X, Bounds.Y);
        }

        public virtual void SetPos(Point pos)
        {
            Bounds.X = pos.X;
            Bounds.Y = pos.Y;
        }

        public virtual void AddPos(Point pos)
        {
            Bounds.ShiftBy(pos);
        }

        public virtual void AddChild(IElement child)
        {
            child.SetParent(this);
            Children.Add(child);
        }

        public virtual void RemoveChild(IElement child)
        {
            child.SetParent(null);
            Children.Remove(child);
        }

        public virtual void SetParent(IElement element)
        {
            Parent = element;
        }

        public virtual bool IsFocus()
        {
            IElement curr = Parent;
            while (curr != null)
            {
                if (curr is Canvas canvas && canvas.focus == this)
                {
                    return true;
                }

                curr = curr.GetParent();
            }

            return false;
        }

        public virtual IElement GetParent()
        {
            return Parent;
        }

        public virtual void SetFocus(IElement focus)
        {
            IElement curr = Parent;
            while (curr != null)
            {
                if (curr is Canvas canvas)
                {
                    canvas.focus = focus;
                }

                curr = curr.GetParent();
            }
        }
    }
}
