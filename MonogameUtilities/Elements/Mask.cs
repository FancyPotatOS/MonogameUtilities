using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameUtilities.Information;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameUtilities.Elements
{
    public class Mask : Element
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

        public Mask(int x, int y, int width, int height, IElement parent, Texture2D maskTexture, bool firstPass = false, Effect maskEffect = null) : base(x, y, width, height, parent)
        {
            MaskTexture = maskTexture;
            if (maskEffect == null)
            {
                MaskEffect = GlobalData.StaticContentReference.Load<Effect>("Effects/Mask");
            }
            else
            {
                MaskEffect = maskEffect;
            }
            FirstPass = firstPass;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (FirstPass)
            {
                base.Draw(sb);
            }

            SpriteBatch maskedSpriteBatch = new(GlobalData.StaticGraphicsDeviceReference);

            maskedSpriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, MaskEffect, transformMatrix);

            MaskEffect.Parameters["Mask"].SetValue(MaskTexture);
            base.Draw(maskedSpriteBatch);

            maskedSpriteBatch.End();
        }
    }
}
