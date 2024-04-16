using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameUtilities.Contexts
{
    public interface IContext
    {
        public IContext Update();

        public void Draw();
    }
}
