using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib;
using LiteNetLib.Utils;

namespace FirClient.Message
{
    class DefaultHandler : BaseMessageHandler
    {
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
        }
    }
}
