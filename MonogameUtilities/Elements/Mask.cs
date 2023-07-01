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
        SpriteSortMode sortMode = SpriteSortMode.Deferred;
        BlendState blendState = null;
        SamplerState samplerState = null;
        DepthStencilState depthStencilState = null;
        RasterizerState rasterizerState = null;
        Matrix? transformMatrix = null;

        Texture2D MaskTexture;
        Effect MaskEffect;
        // If should draw the child textures with the given sprite batch first
        bool FirstPass;

        public Mask(int x, int y, int width, int height, IElement parent, Texture2D maskTexture, bool firstPass = false) : base(x, y, width, height, parent)
        {
            MaskTexture = maskTexture;
            MaskEffect = GlobalData.StaticContentReference.Load<Effect>("Effects/Mask");
            FirstPass = firstPass;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (FirstPass)
            {
                base.Draw(sb);
            }

            SpriteBatch maskedSpriteBatch = new SpriteBatch(GlobalData.StaticGraphicsDeviceReference);

            maskedSpriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, MaskEffect, transformMatrix);

            MaskEffect.Parameters["Mask"].SetValue(MaskTexture);
            base.Draw(maskedSpriteBatch);

            maskedSpriteBatch.End();
        }
    }
}
