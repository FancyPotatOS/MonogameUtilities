using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonogameUtilities.Elements;
using MonogameUtilities.Hitboxes;

namespace MonogameUtilities.Demos
{
    public static class DemoStaticMask
    {
        public static void InitializeInCanvas(ContentManager Content, Canvas canvas)
        {
            Texture2D rainWorldTex = Content.Load<Texture2D>("rain_world");
            Texture2D rainWorldMask = Content.Load<Texture2D>("rain_world_mask");
            Hitbox maskBounds = new(10, 10, rainWorldTex.Width, rainWorldTex.Height);
            StaticMask staticMaskElement = new(10, 10, rainWorldTex.Width, rainWorldTex.Height, null, rainWorldMask, maskBounds, 1);
            ImageElement imgElement = new(10, 10, rainWorldTex.Width, rainWorldTex.Height, null, rainWorldTex);

            Draggable dragElement = new(10, 10, rainWorldTex.Width, rainWorldTex.Height, null, false);

            staticMaskElement.AddChild(imgElement);
            dragElement.AddChild(staticMaskElement);
            canvas.AddChild(dragElement);
        }
    }
}
