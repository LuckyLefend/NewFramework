using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using MasterServer.View;
using Utility;
using UnityEngine;

namespace MasterServer.Message
{
    class ReqUserInfoHandler : BaseMessageHandler
    {
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
            var npcid = peer.ConnectId;
            var list = ConfigMgr.HeroSpawnPoints.Values.ToList();

            var index = AppUtil.Random(0, list.Count);
            var spawnPos = list[index];

            var npcView = new NPCView(npcid, NpcType.Player);
            npcView.mPeer = peer;
            npcView.position = spawnPos;
            Log.Warn("ReqUserInfoHandler Player:> " + npcid + " SpwanPos:> " + spawnPos);

            var dw = new NetDataWriter();
            FillInfo(ref dw, Protocal.RetNewPlayer, npcView);

            var npcs = NpcMgr.mNpcs;
            foreach (var de in npcs)
            {
                var mPeer = de.Value.mPeer;
                if (mPeer != null && mPeer.ConnectionState == ConnectionState.Connected)
                {
                    mPeer.Send(dw, DeliveryMethod.ReliableOrdered);
                }
            }
            //------------------------------------------------------------------------------------

            FillInfo(ref dw, Protocal.ReqUserInfo, npcView);
            peer.Send(dw, DeliveryMethod.ReliableOrdered);

            ///添加到NPC管理器
            NpcMgr.AddNpc(npcid, npcView);
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        void FillInfo(ref NetDataWriter dw, Protocal protocal, NPCView npcView)
        {
            dw.Reset();
            dw.Put((ushort)protocal);
            dw.Put(npcView.npcId);
            dw.Put(npcView.position);
            dw.Put(Quaternion.identity);
        }
    }
}
