using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameUtilities.Hitboxes;
using MonogameUtilities.Information;

namespace MonogameUtilities.Elements
{
    public class ImageElement : Element
    {
        public Texture2D Texture;
        public Hitbox ImageSize;
        public Hitbox ImageBounds;
        public Color Tint;

        public ImageElement(int x, int y, int width, int height, IElement parent, Texture2D texture, Hitbox imageSize = null, Hitbox imageBounds = null, Color? tint = null) : base(x, y, width, height, parent)
        {
            Texture = texture;

            if (imageSize != null)
            {
                ImageSize = imageSize;
            }
            else
            {
                ImageSize = new Hitbox(x, y, width, height);
            }

            if (imageBounds != null)
            {
                ImageBounds = imageBounds;
            }
            else
            {
                ImageBounds = Bounds;
            }

            if (tint.HasValue)
            {
                Tint = tint.Value; 
            }
            else
            {
                Tint = Color.White;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            Rectangle rect = ImageBounds.AsRectangle();
            GlobalData.Draw(Texture, rect, Tint, sb);

            base.Draw(sb);
        }
    }
}
