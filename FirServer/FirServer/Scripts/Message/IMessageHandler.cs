using System;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MasterServer
{
    public interface IMessageHandler
    {
        void OnMessage(NetPeer peer, NetDataReader reader);
    }
}
