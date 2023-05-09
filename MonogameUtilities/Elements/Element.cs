

using MonogameUtilities.Hitboxes;
using System;

namespace MonogameUtilities.Elements
{
    public class Element : IElement
    {
        public Hitbox bound;
        public IElement parent;
        public int layer;

        public int TopLayer { get; set; }

        public Element(int x, int y, int width, int height, IElement parent)
        {
            bound = new Hitbox(x, y, width, height);
            this.parent = parent;

            layer = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Whether should be removed from parent</returns>
        public virtual bool Update() { return false; }

        public virtual void Draw() { }

        public virtual int GetLayer() { return layer; }

        public void Bound(Hitbox bounding)
        {
            if (!bounding.Contains(bound))
            {
                bound.X = Math.Max(bound.X, bounding.X);
                bound.Y = Math.Max(bound.Y, bounding.Y);
                if (bounding.RightSide < bound.RightSide)
                {
                    bound.X = bounding.RightSide - bound.Width;
                }
                if (bounding.BottomSide < bound.BottomSide)
                {
                    bound.Y = bounding.BottomSide - bound.Height;
                }
            }
        }
    }
}
