using Microsoft.Xna.Framework;

namespace MonogameUtilities.Elements
{
    public class BoundingElement : Element
    {
        public BoundingElement(int x, int y, int width, int height, IElement parent) : base(x, y, width, height, parent) { }

        public override bool Update()
        {
            foreach (IElement child in Children)
            {
                child.Bound(Bounds);
            }

            return base.Update();
        }

        public override void SetPos(Point pos)
        {
            base.SetPos(pos);

            foreach (IElement child in Children)
            {
                child.Bound(Bounds);
            }
        }

        public override void AddPos(Point pos)
        {
            base.AddPos(pos);

            foreach (IElement child in Children)
            {
                child.Bound(Bounds);
            }
        }

        public override void AddChild(IElement child)
        {
            base.AddChild(child);

            child.Bound(Bounds);
        }
    }
}
