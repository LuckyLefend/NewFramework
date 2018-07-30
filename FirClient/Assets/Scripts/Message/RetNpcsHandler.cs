using System;
using System.Collections.Generic;
using FirClient.Manager;
using FirClient.View;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace FirClient.Message
{
    class RetNpcsHandler : BaseMessageHandler
    {
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
            var count = reader.GetInt();
            for (int i = 0; i < count; i++)
            {
                var npcid = reader.GetLong();
                var npcType = (NpcType)reader.GetUShort();
                var position = reader.GetVector3();
                var rotation = reader.GetQuaternion();

                NPCView npcView = null;
                switch (npcType)
                {
                    case NpcType.Player:
                        npcView = npcMgr.CreateNpc<TankView>();
                    break;
                    case NpcType.Monster:
                        npcView = npcMgr.CreateNpc<AnimalView>();
                    break;
                }
                if (npcView != null)
                {
                    npcView.Initialize(npcid);
                    npcView.InitInterpolateFields(position, rotation);
                }
                Debugger.LogWarning("OnResNpcs--->>>" + npcid + " " + npcType + " " + position);
            }
        }
    }
}
