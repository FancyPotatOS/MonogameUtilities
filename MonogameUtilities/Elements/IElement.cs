using Microsoft.Xna.Framework;
using MonogameUtilities.Hitboxes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameUtilities.Elements
{
    public interface IElement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Whether should be removed from parent</returns>
        public bool Update();

        public void Draw();

        public int GetLayer();

        public Hitbox GetBounds();

        public void Bound(Hitbox bound);
        public void Intersect(Hitbox bound, int margin);

        public void Click();
        public bool DragStart();
        public bool DragMid();
        public bool DragEnd();

        public Point GetPos();
        public void SetPos(Point pos);
        public void AddPos(Point pos);

        public void SetParent(IElement element);
        public IElement GetParent();
        public bool IsFocus();
        public void SetFocus(IElement focus);

        public int TopLayer { get; set; }
    }
}
