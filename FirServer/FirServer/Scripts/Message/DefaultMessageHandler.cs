using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib;
using LiteNetLib.Utils;
using MasterServer.Manager;

namespace MasterServer.Message
{
    class DefaultMessageHandler : BaseMessageHandler
    {
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
            var message = "Hello client! I cannot found Handler";
            var dw = new NetDataWriter();
            dw.Put((ushort)Protocal.Default);
            dw.Put(message);
            peer.Send(dw, DeliveryMethod.ReliableOrdered);
        }
    }
}
