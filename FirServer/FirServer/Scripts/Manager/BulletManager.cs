using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterServer.View;
using UnityEngine;
using Utility;

namespace MasterServer.Manager
{
    public class BulletManager : BaseBehaviour, IManager
    {
        private readonly object bulletLock = new object();
        private Dictionary<long, BulletView> mBullets = new Dictionary<long, BulletView>();

        public BulletManager()
        {
            BulletMgr = this;
        }

        public void Initialize()
        {
        }

        /// <summary>
        /// 创建一个子弹
        /// </summary>
        public BulletView Create()
        {
            lock (bulletLock)
            {
                var objId = DateTime.UtcNow.Ticks;
                var view = new BulletView(objId);
                mBullets.Add(objId, view);
                return view;
            }
        }

        public void OnFrameUpdate()
        {
        }

        /// <summary>
        /// 销毁子弹
        /// </summary>
        public void Destroy(long objId)
        {
            lock (bulletLock)
            {
                mBullets.Remove(objId);
            }
        }

        public void OnDispose()
        {
            mBullets = null;
        }

    }
}
