using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib;
using LiteNetLib.Utils;
using MasterServer.Manager;

namespace MasterServer.Message
{
    class ReqNpcsHandler : BaseMessageHandler
    {
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
            var npcViews = NpcMgr.mNpcs.Where(r => r.Key != peer.ConnectId);

            var dw = new NetDataWriter();
            dw.Put((ushort)Protocal.ReqNpcInfo);
            dw.Put(npcViews.Count());

            foreach (var de in npcViews)
            {
                if (peer == de.Value.mPeer)
                {
                    continue;
                }
                dw.Put(de.Key);
                dw.Put((ushort)de.Value.npcType);
                dw.Put(de.Value.position);
                dw.Put(de.Value.rotation);
            }
            peer.Send(dw, DeliveryMethod.ReliableOrdered);
        }
    }
}
