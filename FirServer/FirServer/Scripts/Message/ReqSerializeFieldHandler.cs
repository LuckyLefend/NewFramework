using System;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MasterServer.Message
{
    class ReqSerializeFieldHandler : BaseMessageHandler
    {
        private byte[] _readDirtyFlags = new byte[1];

        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
            var npcid = reader.GetLong();
            reader.GetBytes(_readDirtyFlags, 1);
            var npcView = NpcMgr.GetNpc(npcid);
            if (npcView != null)
            {
                if ((0x1 & _readDirtyFlags[0]) != 0)
                {
                    npcView.position = reader.GetVector3();
                }
                if ((0x2 & _readDirtyFlags[0]) != 0)
                {
                    npcView.rotation = reader.GetQuaternion();
                }
            }
            base.IntactTransterMessage(peer, reader);
        }
    }
}
