

using MonogameUtilities.Util;
using System;
using System.Collections.Generic;

namespace MonogameUtilities.Networking
{
    public interface IRollbackObject<T>
    {

        public void SaveState();

        public void RestoreState();

        public JsonWrapper GetSaveState();

        public void LoadFromSaveState(JsonWrapper state);

        public void Update(Dictionary<Guid, (List<T> newInp, List<T> preInp, List<T> accInp)> inputs);

    }
}
