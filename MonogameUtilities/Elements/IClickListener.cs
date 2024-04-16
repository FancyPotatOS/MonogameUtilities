using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameUtilities.Elements
{
    public interface IClickListener
    {
        void OnClick(Clickable cl);

        void OffClick(Clickable cl);
    }
}
