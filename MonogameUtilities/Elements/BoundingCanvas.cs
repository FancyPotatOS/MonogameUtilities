using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameUtilities.Hitboxes;
using MonogameUtilities.Information;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameUtilities.Elements
{
    public class BoundingCanvas : AnchoringCanvas
    {
        public BoundingCanvas(int x = 0, int y = 0, int width = 0, int height = 0, IElement parent = null) : base(x, y, width, height, parent) { }

        public override bool Update()
        {
            bool parentUpdate = base.Update();

            foreach (IElement e in Children)
            {
                e.Bound(Bounds);
            }

            return parentUpdate;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public override void Move(Point change)
        {
            base.Move(change);

            Bounds.X += change.X;
            Bounds.Y += change.Y;
        }
    }
}
