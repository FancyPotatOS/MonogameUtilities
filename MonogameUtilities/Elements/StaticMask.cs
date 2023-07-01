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
    public class StaticMask : Element
    {
        // Sprite batch options
        internal SpriteSortMode sortMode = SpriteSortMode.Deferred;
        internal BlendState blendState = null;
        internal SamplerState samplerState = null;
        internal DepthStencilState depthStencilState = null;
        internal RasterizerState rasterizerState = null;
        internal Matrix? transformMatrix = null;

        internal Texture2D MaskTexture;
        internal Effect MaskEffect;
        // If should draw the child textures with the given sprite batch first
        internal bool FirstPass;
        float BoundsAlpha;

        Hitbox MaskBounds;

        public StaticMask(int x, int y, int width, int height, IElement parent, Texture2D maskTexture, Hitbox maskBounds, float boundsAlpha = 0, bool firstPass = false) : base(x, y, width, height, parent)
        {
            MaskTexture = maskTexture;

            MaskEffect = GlobalData.StaticContentReference.Load<Effect>("Effects/StaticMask");
            MaskEffect.Parameters["Mask"].SetValue(MaskTexture);

            MaskBounds = maskBounds;

            FirstPass = firstPass;
            BoundsAlpha = boundsAlpha;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (FirstPass)
            {
                base.Draw(sb);
            }

            MaskEffect.Parameters["MaskLocationX"].SetValue(MaskBounds.X);
            MaskEffect.Parameters["MaskLocationY"].SetValue(MaskBounds.Y);
            MaskEffect.Parameters["MaskWidth"].SetValue(MaskBounds.Width);
            MaskEffect.Parameters["MaskHeight"].SetValue(MaskBounds.Height);
            MaskEffect.Parameters["BoundsAlpha"].SetValue(BoundsAlpha);

            SpriteBatch maskedSpriteBatch = new(GlobalData.StaticGraphicsDeviceReference);

            maskedSpriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, MaskEffect, transformMatrix);
            
            foreach (IElement child in Children)
            {
                Hitbox childHitbox = child.GetBounds();
                MaskEffect.Parameters["BaseTextureLocationX"].SetValue(childHitbox.X);
                MaskEffect.Parameters["BaseTextureLocationY"].SetValue(childHitbox.Y);
                MaskEffect.Parameters["BaseTextureWidth"].SetValue(childHitbox.Width);
                MaskEffect.Parameters["BaseTextureHeight"].SetValue(childHitbox.Height);

                child.Draw(maskedSpriteBatch);
            }

            maskedSpriteBatch.End();
        }
    }
}
