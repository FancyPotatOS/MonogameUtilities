using Microsoft.Xna.Framework;
using MonogameUtilities.Elements;
using System.Reflection.Emit;

namespace MonogameUtilities
{
    public class DraggableElement : Draggable
    {
        Color color;

        public DraggableElement(int x, int y, int width, int height, Color col, IElement parent) : base(x, y, width, height, parent)
        {
            parent.TopLayer++;
            layer = parent.TopLayer;
            color = col;
        }

        bool clicked = false;
        public override void Clicked()
        {
            clicked = true;
            color = Color.Purple;
        }

        public override bool DragStart()
        {
            bool consumed = base.DragStart();

            // Drag start logic here
            if (consumed)
            {
                parent.TopLayer++;
                layer = parent.TopLayer;
                color = Color.DarkRed;
            }

            return consumed;
        }

        public override bool DragMid()
        {
            bool consumed = base.DragMid();

            // Drag middle logic here
            if (consumed)
                color = Color.DarkBlue;

            return consumed;
        }

        public override bool DragEnd()
        {
            bool consumed = base.DragEnd();

            // Drag end logic here
            if (consumed && !clicked)
                color = Color.DarkGreen;

            clicked = false;

            return consumed;
        }

        public override void Draw()
        {
            Color color = this.color;
            if (parent is Canvas canvas && canvas.focus == this)
                color.A = 128;

            UtilityTesting.Draw(UtilityTesting.pixel, bound.AsRectangle(), color);
        }
    }
}
