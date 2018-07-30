using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FirClient.ObjectPool;
using UnityEngine.Events;
using LiteNetLib.Utils;
using FirClient.Network;

namespace FirClient.Manager
{
    public class ObjectBase
    {
        public void OnStart() { }
        public void OnDispose() { }
    }

    public class ObjectManager : BaseManager
    {
        private Transform m_PoolRootObject = null;
        private Dictionary<string, object> m_ObjectPools = new Dictionary<string, object>();
        private Dictionary<string, GameObjectPool> m_GameObjectPools = new Dictionary<string, GameObjectPool>();

        Transform PoolRootObject
        {
            get
            {
                if (m_PoolRootObject == null)
                {
                    var objectPool = new GameObject("ObjectPool");
                    objectPool.transform.SetParent(io.manager.transform);
                    objectPool.transform.localScale = Vector3.one;
                    objectPool.transform.localPosition = Vector3.zero;
                    m_PoolRootObject = objectPool.transform;
                }
                return m_PoolRootObject;
            }
        }

        public override void Initialize()
        {
            var npcPrefab = resMgr.LoadAsset<GameObject>("Prefabs/Object/NPCObject");
            this.CreatePool(PoolNames.NPC, 5, 10, npcPrefab);

            var bulletPrefab = resMgr.LoadAsset<GameObject>("Prefabs/Object/BulletObject");
            this.CreatePool(PoolNames.BULLET, 5, 10, bulletPrefab);

            var effectPrefab = resMgr.LoadAsset<GameObject>("Prefabs/Object/EffectObject");
            this.CreatePool(PoolNames.EFFECT, 5, 10, effectPrefab);

            var tankPrefab = resMgr.LoadAsset<GameObject>("Prefabs/Character/" + PoolNames.TANK);
            this.CreatePool(PoolNames.TANK, 5, 10, tankPrefab);

            var animalPrefab = resMgr.LoadAsset<GameObject>("Prefabs/Character/" + PoolNames.ANIMAL);
            this.CreatePool(PoolNames.ANIMAL, 5, 10, animalPrefab);

            ///创建包对象池
            var packetPool = this.CreatePool<PacketData>(delegate(PacketData packet) { }, delegate(PacketData packet) {
                packet.Reset(); //重置
            });
            for (int i = 0; i < AppConst.NetMessagePoolMax; i++)
            {
                packetPool.Release(new PacketData());
            }
        }

        public override void OnFixedUpdate(float deltaTime)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public GameObjectPool CreatePool(string poolName, int initSize, int maxSize, GameObject prefab)
        {
            var pool = new GameObjectPool(poolName, prefab, initSize, maxSize, PoolRootObject);
            m_GameObjectPools[poolName] = pool;
            return pool;
        }

        public GameObjectPool GetPool(string poolName)
        {
            if (m_GameObjectPools.ContainsKey(poolName))
            {
                return m_GameObjectPools[poolName];
            }
            return null;
        }

        public GameObject Get(string poolName)
        {
            GameObject result = null;
            if (m_GameObjectPools.ContainsKey(poolName))
            {
                GameObjectPool pool = m_GameObjectPools[poolName];
                var poolObj = pool.NextAvailableObject();
                if (poolObj == null)
                {
                    Debug.LogWarning("No object available in pool. Consider setting fixedSize to false.: " + poolName);
                }
                else
                {
                    result = poolObj.gameObject;
                }
            }
            else
            {
                Debug.LogError("Invalid pool name specified: " + poolName);
            }
            return result;
        }

        public void Release(GameObject gameObj)
        {
            if (gameObj == null)
            {
                return;
            }
            var poolObject = gameObj.GetComponent<PoolObject>();
            if (poolObject != null)
            {
                var poolName = poolObject.poolName;
                if (m_GameObjectPools.ContainsKey(poolName))
                {
                    GameObjectPool pool = m_GameObjectPools[poolName];
                    pool.ReturnObjectToPool(poolObject);
                }
                else
                {
                    Debug.LogWarning("No pool available with name: " + poolName);
                }
            }
        }

        ///-----------------------------------------------------------------------------------------------
        public ObjectPool<T> CreatePool<T>(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease) where T : class
        {
            var type = typeof(T);
            var pool = new ObjectPool<T>(actionOnGet, actionOnRelease);
            m_ObjectPools[type.Name] = pool;
            return pool;
        }

        public ObjectPool<T> GetPool<T>() where T : class
        {
            var type = typeof(T);
            ObjectPool<T> pool = null;
            if (m_ObjectPools.ContainsKey(type.Name))
            {
                pool = m_ObjectPools[type.Name] as ObjectPool<T>;
            }
            return pool;
        }

        public bool Exist<T>()
        {
            var type = typeof(T);
            return m_ObjectPools.ContainsKey(type.Name);
        }

        public T Get<T>() where T : class
        {
            var pool = GetPool<T>();
            if (pool != null)
            {
                return pool.Get();
            }
            return default(T);
        }

        public void Release<T>(T obj) where T : class
        {
            var pool = GetPool<T>();
            if (pool != null)
            {
                pool.Release(obj);
            }
        }

        public override void OnDispose()
        {
            throw new System.NotImplementedException();
        }
    }
}