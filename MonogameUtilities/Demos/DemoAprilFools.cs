using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonogameUtilities.Elements;

namespace MonogameUtilities.Demos
{
    public static class DemoAprilFools
    {
        public static void InitializeInCanvas(ContentManager Content, Canvas canvas)
        {
            /**/
            Texture2D spyglass = Content.Load<Texture2D>("spyglass32");
            ImageElement spyglassElement = new(0, 0, spyglass.Width, spyglass.Height, null, spyglass)
            {
                Layer = 1
            };
            canvas.AddChild(spyglassElement);
            /**/

            /**/
            Texture2D map = Content.Load<Texture2D>("map");
            ImageElement mapElement = new(5, 5, map.Width, map.Height, null, map);
            /**/

            Draggable dragElement = new(5, 5, map.Width, map.Height, canvas, true, spyglassElement.Bounds)
            {
                Layer = 2,
                dragScale = -1
            };
            canvas.AddChild(dragElement);

            IntersectingElement intersectElement = new(5, 5, spyglass.Width, spyglass.Height, dragElement, spyglass.Width / 2, spyglassElement.Bounds);
            intersectElement.AddChild(mapElement);
            dragElement.AddChild(intersectElement);
        }
    }
}
