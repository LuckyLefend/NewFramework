using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace FirClient.Message
{
    public abstract class BaseMessageHandler : BaseBehaviour, IMessageHandler
    {
        public abstract void OnMessage(NetPeer peer, NetDataReader reader);
    }
}

