

using Microsoft.Xna.Framework;
using MonogameUtilities.Elements;
using System.Security.Cryptography;

namespace MonogameUtilities
{
    public class ClickableElement : Clickable
    {
        Color color;
        public ClickableElement(int x, int y, int width, int height, IElement parent) : base (x, y, width, height, parent)
        {
            parent.TopLayer++;
            layer = parent.TopLayer;
            color = Color.Teal;
        }

        public override void Clicked()
        {
            parent.TopLayer++;
            layer = parent.TopLayer;
            color = Color.Azure;
        }

        public override void Draw()
        {
            Color color = this.color;
            if (parent is Canvas canvas && canvas.focus == this)
                color.A = 128;
            
            UtilityTesting.Draw(UtilityTesting.pixel, bound.AsRectangle(), color);
            this.color = Color.Teal;
        }
    }
}
