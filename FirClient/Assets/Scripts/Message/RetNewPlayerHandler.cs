//File creation in 2017年11月25日 09:25:32 Saturday
using System.Collections;
using System.Collections.Generic;
using FirClient.View;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace FirClient.Message
{
    public class RetNewPlayerHandler : BaseMessageHandler
    {
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
            var playerId = reader.GetLong();
            var spawnPos = reader.GetVector3();
            var rotation = reader.GetQuaternion();

            string name = "Player_" + playerId;
            var npcView = npcMgr.CreateNpc<TankView>();
            var playerView = npcView as TankView;
            if (playerView != null)
            { 
                playerView.Initialize(playerId);
                playerView.InitInterpolateFields(spawnPos, rotation);
            }
            //lockstepMgr.UpdateClientCount(playerId);
            Debugger.LogWarning("OnNewPlayer--->>>" + name + " " + spawnPos);
        }
    }
}

