using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer.Manager
{
    interface IManager
    {
        void Initialize();
        void OnFrameUpdate();
        void OnDispose();
    }
}
