using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameUtilities.Elements
{
    public interface IDragListener
    {
        void OnDragStart(Draggable dr);

        void OnDragMid(Draggable dr);

        void OnDragEnd(Draggable dr);
    }
}
