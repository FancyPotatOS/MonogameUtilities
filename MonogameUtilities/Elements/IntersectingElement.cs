using Microsoft.Xna.Framework;
using MonogameUtilities.Hitboxes;

namespace MonogameUtilities.Elements
{
    public class IntersectingElement : Element
    {
        Hitbox IntersectionHitbox;

        public IntersectingElement(int x, int y, int width, int height, IElement parent, int margin = 0, Hitbox intersectionHitbox = null) : base(x, y, width, height, parent, margin) 
        {
            if (intersectionHitbox != null)
            {
                IntersectionHitbox = intersectionHitbox;
            }
            else
            {
                IntersectionHitbox = Bounds;
            }
        }

        public override bool Update()
        {
            foreach (IElement child in Children)
            {
                child.Intersect(IntersectionHitbox, Margin);
            }

            return base.Update();
        }

        public override void SetPos(Point pos)
        {
            base.SetPos(pos);

            if (Children != null)
            {
                foreach (IElement child in Children)
                {
                    child.Intersect(IntersectionHitbox, Margin);
                }
            }
        }

        public override void AddPos(Point pos)
        {
            base.AddPos(pos);

            foreach (IElement child in Children)
            {
                child.AddPos(pos);
                child.Intersect(IntersectionHitbox, Margin);
            }
        }

        public override void AddChild(IElement child)
        {
            base.AddChild(child);

            child.Intersect(IntersectionHitbox, Margin);
        }
    }
}
