using System;
using LiteNetLib;
using LiteNetLib.Utils;

namespace FirClient.Message
{
    public interface IMessageHandler
    {
        void OnMessage(NetPeer peer, NetDataReader reader);
    }
}