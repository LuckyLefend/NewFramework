using System.Collections;
using System.Collections.Generic;
using FirClient.View;
using LiteNetLib.Utils;
using UnityEngine;

namespace FirClient.Manager
{
    public class BattleManager : BaseManager
    {
        public override void Initialize()
        {
            Messenger.AddListener(EventNames.BtnOpenFire, OnFightClick);
            Messenger.AddListener<long, long>(EventNames.EvOpenFire, OnFireBullet);
        }

        public override void OnFixedUpdate(float deltaTime)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        void OnFightClick()
        {
            var npcView = npcMgr.Owner as NPCView;
            if (npcView != null)
            {
                var gameObj = npcView.roleObject;
                var gunMuzzle = gameObj.transform.Find("GunMuzzle");

                var buffer = new NetDataWriter();
                buffer.Put(gunMuzzle.position);
                buffer.Put(npcView.transform.localEulerAngles);

                networkMgr.SendData(Protocal.ReqOpenFire, buffer);
                Debugger.Log("OnBattleClick--->>>");
            }
        }

        /// <summary>
        /// 接收到开火事件
        /// </summary>
        public void OnFireBullet(long npcId, long objId)
        {
            var npc = npcMgr.GetNpc(npcId);
            var npcView = npc as NPCView;
            if (npcView != null)
            {
                var gameObj = npcView.roleObject;
                var animObj = npcView.animObject;
                var isOwner = npcView.NetworkId == npcMgr.Owner.NetworkId;

                var bullet = bulletMgr.Create("bullet2", objId);
                if (bullet != null)
                {
                    var angle = npcView.transform.localEulerAngles;
                    var gunMuzzle = gameObj.transform.Find("GunMuzzle");

                    bullet.OnAwake();
                    bullet.OpenFire(gunMuzzle.position, angle, 250, isOwner);
                }
                animObj.SetPlay(AnimKey.Attack);
            }
        }

        public override void OnDispose()
        {
            Messenger.RemoveListener(EventNames.BtnOpenFire, OnFightClick);
            Messenger.RemoveListener<long, long>(EventNames.EvOpenFire, OnFireBullet);
        }
    }
}

