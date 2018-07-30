using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MasterServer.Message
{
    class BaseMessageHandler : BaseBehaviour, IMessageHandler
    {
        public virtual void OnMessage(NetPeer peer, NetDataReader reader)
        {
        }

        /// <summary>
        /// 原样转发
        /// </summary>
        protected void IntactTransterMessage(NetPeer peer, NetDataReader reader, bool includeMe = false)
        {
            var npcs = NpcMgr.mNpcs;
            if (npcs.Count() > 0)
            {
                var dw = new NetDataWriter();
                dw.Put(reader.Data);    //直接转发
                foreach (var de in npcs)
                {
                    var mPeer = de.Value.mPeer;
                    if (!includeMe && peer == mPeer)
                    {
                        continue;
                    }
                    if (mPeer != null && mPeer.ConnectionState == ConnectionState.Connected)
                    {
                        mPeer.Send(dw, DeliveryMethod.ReliableOrdered);
                    }
                }
            }
        }
    }
}
