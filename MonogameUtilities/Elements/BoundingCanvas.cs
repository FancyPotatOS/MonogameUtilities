using Microsoft.Xna.Framework;
using MonogameUtilities.Hitboxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameUtilities.Elements
{
    public class BoundingCanvas : Canvas
    {
        readonly Hitbox bound;

        public BoundingCanvas(int x, int y, int width, int height) : base()
        {
            bound = new Hitbox(x, y, width, height);
        }

        public override bool Update()
        {
            bool parentUpdate = base.Update();

            foreach (Element e in elements.Where(e => e is Element).Cast<Element>())
            {
                e.Bound(bound);
            }

            return parentUpdate;
        }

        public override void Draw()
        {
            UtilityTesting.Draw(UtilityTesting.pixel, bound.AsRectangle(), Color.Bisque);

            base.Draw();
        }
    }
}
