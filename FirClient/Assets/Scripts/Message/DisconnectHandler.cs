using System.Collections;
using System.Collections.Generic;
using FirClient.Manager;
using FirClient.Manager;
using FirClient.View;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace FirClient.Message
{
    public class DisconnectHandler : BaseMessageHandler
    {
        /// <summary>
        /// 玩家掉线
        /// </summary>
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
            var npcid = reader.GetLong();

            var playerName = "Player_" + npcid;
            npcMgr.RemoveNpc<TankView>(npcid);

            //lockstepMgr.UpdateClientCount(npcid);
            Debugger.LogWarning("PlayerDisconnect--->>>" + playerName);
        }
    }
}
