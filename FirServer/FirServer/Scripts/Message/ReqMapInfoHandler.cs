using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib;
using LiteNetLib.Utils;
using MasterServer;
using MasterServer.Manager;
using MasterServer.View;
using Utility;

namespace MasterServer.Message
{
    class ReqMapInfoHandler : BaseMessageHandler
    {
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
        }
    }
}
