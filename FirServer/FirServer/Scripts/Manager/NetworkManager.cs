using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MasterServer.Manager
{
    public class NetworkManager : BaseBehaviour, IManager
    {
        public NetworkManager()
        {
            NetworkMgr = this;
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
