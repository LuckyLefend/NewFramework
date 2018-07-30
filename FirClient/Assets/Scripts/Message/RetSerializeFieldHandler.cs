using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FirClient.View;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace FirClient.Message
{
    class RetSerializeFieldHandler : BaseMessageHandler
    {
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
            var npcid = reader.GetLong();
            var npcView = npcMgr.GetNpc(npcid) as NPCView;
            if (npcView != null)
            {
                npcView.ReadDirtyFields(reader);
            }
        }
    }
}
