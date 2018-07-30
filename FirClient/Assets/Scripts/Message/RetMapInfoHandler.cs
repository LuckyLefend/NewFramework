using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FirClient.Manager;
using FirClient.View;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace FirClient.Message
{
    public class RetMapInfoHandler : BaseMessageHandler
    {
        public override void OnMessage(NetPeer peer, NetDataReader reader)
        {
        }
    }
}
