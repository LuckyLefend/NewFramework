using System.Collections;
using System.Collections.Generic;
using FirClient.View;
using UnityEngine;

namespace FirClient.Manager
{
    public class BulletManager : BaseManager
    {
        private Dictionary<long, BulletView> mBullets = new Dictionary<long, BulletView>();

        public override void Initialize()
        {
            var bulletList = cfgMgr.GetBulletList();
            foreach (var de in bulletList)
            {
                var poolName = de.Value.name;
                var bulletPrefab = resMgr.LoadAsset<GameObject>("Prefabs/Bullet/" + de.Value.resource);
                objMgr.CreatePool(poolName, 5, 10, bulletPrefab);
            }
        }

        public override void OnFixedUpdate(float deltaTime)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public BulletView Create(string bulletName, long objId)
        {
            var data = cfgMgr.GetBulletData(bulletName);
            var bulletObj = objMgr.Get(PoolNames.BULLET);
            if (bulletObj != null)
            {
                bulletObj.transform.SetParent(io.battleObject);
                bulletObj.transform.SetLayer(AppConst.GameplayLayer);

                var bullet = new BulletView();
                bullet.Initialize(data, objId);

                bulletObj.AddComponent<ViewObject>().BindView(bullet);

                mBullets.Add(objId, bullet);
                return bullet;
            }
            return null;
        }

        public BulletView GetView(long objId)
        {
            BulletView view = null;
            mBullets.TryGetValue(objId, out view);
            return view;
        }

        public bool RemoveView(long objId)
        {
            return mBullets.Remove(objId);
        }

        public override void OnDispose()
        {
        }
    }
}
