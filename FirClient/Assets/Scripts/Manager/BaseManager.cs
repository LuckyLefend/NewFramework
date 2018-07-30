using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirClient.Manager
{
    public abstract class BaseManager : BaseBehaviour, IManager
    {
        public bool isOnUpdate = false;
        public bool isFixedUpdate = false;
        public bool isLateUpdate = false;

        public abstract void Initialize();
        public abstract void OnFixedUpdate(float deltaTime);
        public abstract void OnUpdate(float deltaTime);
        public abstract void OnDispose();
    }
}

