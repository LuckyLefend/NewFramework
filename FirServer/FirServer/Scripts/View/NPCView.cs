using System;
using System.Collections.Generic;
using LiteNetLib;
using MasterServer.Manager;
using UnityEngine;
using Utility;

namespace MasterServer.View
{
    public class NPCView : BaseBehaviour, IObjectView
    {
        public long npcId;
        public NpcType npcType;
        public NetPeer mPeer;
        public Vector3 position;
        public Quaternion rotation;

        public NPCView(long npcId, NpcType npcType)
        {
            this.npcId = npcId;
            this.npcType = npcType;
            Log.Info("New NpcId:" + npcId + " NpcType:" + npcType);
        }

        public virtual void OnAwake()
        {
        }

        public virtual void OnDispose()
        {
        }
    }
}
