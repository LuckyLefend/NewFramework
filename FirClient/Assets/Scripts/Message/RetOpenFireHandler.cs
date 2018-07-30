//File creation in 2017年12月10日 10:02:31 Sunday
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace FirClient.Message
{
    public class RetOpenFireHandler : BaseMessageHandler
    {
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
            var npcId = reader.GetLong();
            var bulletId = reader.GetLong();
            Messenger.Broadcast<long, long>(EventNames.EvOpenFire, npcId, bulletId);
        }
    }
}