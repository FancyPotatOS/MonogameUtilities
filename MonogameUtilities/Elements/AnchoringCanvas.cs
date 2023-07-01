using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameUtilities.Elements
{
    public class AnchoringCanvas : Canvas
    {
        public AnchoringCanvas(int x = 0, int y = 0, int width = 0, int height = 0, IElement parent = null) : base(x, y, width, height, parent) { }

        public virtual void Move(Point change)
        {
            foreach (Element element in Children.Where(x => x is Element).Cast<Element>())
            {
                element.Bounds.X += change.X;
                element.Bounds.Y += change.Y;
            }
        }
    }
}
