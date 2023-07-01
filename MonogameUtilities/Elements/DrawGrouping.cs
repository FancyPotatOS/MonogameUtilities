using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameUtilities.Hitboxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameUtilities.Elements
{
    public class DrawGrouping : Element
    {
        public DrawGrouping(int x, int y, int width, int height) : base(x, y, width, height, null) { }

        public override void AddPos(Point pos) { }

        public override void Bound(Hitbox bound) { }

        public override void Click() { }

        public override bool DragEnd() { return false; }

        public override bool DragMid() { return false; }

        public override bool DragStart() { return false; }

        public override void Draw(SpriteBatch sb)
        {
            Children.ForEach(c => c.Draw(sb));
        }

        public override void Intersect(Hitbox bound, int margin) { }

        public override bool IsFocus() { return false; }

        public override bool Update() { return false; }
    }
}
