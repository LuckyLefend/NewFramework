using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib;
using LiteNetLib.Utils;
using MasterServer.Manager;
using Utility;

namespace MasterServer.Message
{
    class DisconnectHandler : BaseMessageHandler
    {
        /// <summary>
        /// 玩家断链接
        /// </summary>
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
            var npcId = peer.ConnectId;
            var npcView = NpcMgr.GetNpc(npcId);
            NpcMgr.RemoveNpc(npcId);

            var npcs = NpcMgr.mNpcs.Where(r => r.Key != npcId);
            if (npcs.Count() > 0)
            {
                var dw = new NetDataWriter();
                dw.Put((ushort)Protocal.Disconnect);
                dw.Put(npcId);

                foreach (var de in npcs)
                {
                    var mPeer = de.Value.mPeer;
                    if (mPeer != null && mPeer.ConnectionState == ConnectionState.Connected)
                    {
                        mPeer.Send(dw, DeliveryMethod.ReliableOrdered);
                    }
                }
            }
        }
    }
}
