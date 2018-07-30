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
    class ReqHitTargetHandler : BaseMessageHandler
    {
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
            base.OnMessage(peer, reader);
            var bulletId = reader.GetLong();
            var npcId = reader.GetLong();

            base.IntactTransterMessage(peer, reader, true);
            Log.Info("Bullet:>" + bulletId + " npcId:>" + npcId);
        }
    }
}
