using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameUtilities.States
{
    public interface IState
    {
        public IState Update();

        public void Draw();
    }
}
