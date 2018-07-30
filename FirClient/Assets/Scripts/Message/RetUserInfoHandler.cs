using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FirClient.Manager;
using FirClient.View;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace FirClient.Message
{
    class RetUserInfoHandler : BaseMessageHandler
    {
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
            var npcid = reader.GetLong();
            var spawnPos = reader.GetVector3();
            var rotation = reader.GetQuaternion();

            var npcView = npcMgr.CreateNpc<TankView>();
            var mainRoleView = npcView as TankView;
            if (mainRoleView != null)
            {
                mainRoleView.Initialize(npcid, true);  //创建主角自身
                mainRoleView.InitInterpolateFields(spawnPos, rotation);
            }
            //lockstepMgr.Identity = npcid;

            ///启动Lockstep同步
            //lockstepMgr.Initialize();
            //lockstepMgr.StartLockstep();
            Debugger.LogWarning("RetUserInfoHandler  npcid:>" + npcid + " spawnPos:>" + spawnPos + " rotation:>" + rotation);
        }
    }
}
