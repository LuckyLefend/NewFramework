using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer.Manager
{
    public class EffectManager : BaseBehaviour, IManager
    {
        public EffectManager()
        {
            EffectMgr = this;
        }

        public void Initialize()
        {
        }

        public void OnFrameUpdate()
        {
        }

        public void OnDispose()
        {
            throw new NotImplementedException();
        }

    }
}
