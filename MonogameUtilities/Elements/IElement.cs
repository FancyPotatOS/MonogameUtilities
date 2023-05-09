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

        public void Bound(Hitbox bound);

        public void SetParent(IElement element) { }
        public int TopLayer { get; set; }
    }
}
