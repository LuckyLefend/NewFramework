
using LiteNetLib.Utils;
using UnityEngine;
namespace FirClient.View
{
    public class ObjectView : NetworkObject, IObjectView
    {
        public Transform transform;
        public GameObject gameObject;
        public ViewObject viewObject;

        public virtual void OnAwake()
        {
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnDispose()
        {
            if (viewObject != null)
            {
                Destroy(viewObject);
            }
        }

        public virtual void OnDestroy()
        {
        }

        public virtual void OnTriggerEnter2D(Collider2D coll)
        {
        }

        protected override NetDataWriter SerializeDirtyFields()
        {
            return null;
        }
    }
}

