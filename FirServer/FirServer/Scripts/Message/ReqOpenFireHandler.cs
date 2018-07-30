using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using Utility;

namespace MasterServer.Message
{
    class ReqOpenFireHandler : BaseMessageHandler
    {
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
            base.OnMessage(peer, reader);

            var spawnPos = reader.GetVector3();
            var spawnAngle = reader.GetVector3();

            var bullet = BulletMgr.Create();
            bullet.startPos = spawnPos;
            bullet.position = spawnPos;
            bullet.angle = spawnAngle;

            var dw = new NetDataWriter();
            dw.Put((ushort)Protocal.ReqOpenFire);
            dw.Put(peer.ConnectId);
            dw.Put(bullet.ObjectId);

            var npcs = NpcMgr.mNpcs;
            foreach (var de in npcs)
            {
                var mPeer = de.Value.mPeer;
                if (mPeer != null && mPeer.ConnectionState == ConnectionState.Connected)
                {
                    mPeer.Send(dw, DeliveryMethod.ReliableOrdered);
                }
            }
            Log.Info("ReqOpenFireHandler--->>" + bullet.ObjectId);
        }
    }
}
