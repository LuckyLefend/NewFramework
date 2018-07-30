using System;
using System.Collections.Generic;
using UnityEngine;

namespace MasterServer.View
{
    public class BulletView : BaseBehaviour, IObjectView
    {
        public long ObjectId { get; private set; }
        public float disappearDistance = 5;
        public Vector3 position;
        public Vector3 angle;
        public Vector3 startPos;

        public BulletView(long objId)
        {
            ObjectId = objId;
        }

        public void OnAwake()
        {
        }

        public void OnDispose()
        {
        }
    }
}
