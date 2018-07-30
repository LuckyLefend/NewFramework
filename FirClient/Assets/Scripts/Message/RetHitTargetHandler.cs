//File creation in 2018年04月07日 08:54:30 Saturday
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace FirClient.Message
{
    public class RetHitTargetHandler : BaseMessageHandler
    {
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
            var bulletId = reader.GetLong();    //子弹ID
            var targetId = reader.GetLong();    //对象ID
            ///销毁子弹
            var bullet = bulletMgr.GetView(bulletId);
            if (bullet != null)
            {
                bullet.OnBomb();
            }
            ///销毁NPC
            var npcView = npcMgr.GetNpc(targetId);
            if (npcView != null)
            {
                npcView.OnDispose();
            }
        }
    }
}