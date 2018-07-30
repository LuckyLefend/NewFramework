using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FirClient.View;

namespace FirClient.Manager
{
    public class NPCManager : BaseManager
    {
        private NPCView _mOwner;
        private readonly object npcLock = new object();
        private Dictionary<long, INPCView> mNpcs = new Dictionary<long, INPCView>();

        public NPCView Owner
        {
            get
            {
                return _mOwner;
            }
            set
            {
                _mOwner = value;
            }
        }

        public Dictionary<long, INPCView> Npcs
        {
            get { return mNpcs; }
        }

        public override void Initialize()
        {
        }

        public override void OnFixedUpdate(float deltaTime)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public void AddNpc(long npcid, INPCView view)
        {
            lock (npcLock)
            {
                if (!mNpcs.ContainsKey(npcid))
                {
                    mNpcs.Add(npcid, view);
                }
            }
        }

        public INPCView GetNpc(long npcid)
        {
            lock (npcLock)
            {
                if (mNpcs.ContainsKey(npcid))
                {
                    return mNpcs[npcid];
                }
                return null;
            }
        }

        /// <summary>
        /// 创建NPC
        /// </summary>
        public T CreateNpc<T>() where T : NPCView, new()
        {
            var go = objMgr.Get(PoolNames.NPC);     //实例化一个客户端对象
            go.transform.SetParent(io.battleObject);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.gameObject.SetActive(true);

            var npcView = new T();
            var viewObject = go.AddComponent<ViewObject>();
            viewObject.BindView(npcView);
            return npcView;
        }

        /// <summary>
        /// 移除角色
        /// </summary>
        public void RemoveNpc<T>(long npcid) where T : NPCView
        {
            var view = this.GetNpc(npcid);
            if (view != null)
            {
                var npcView = view as NPCView;
                if (npcView != null)
                {
                    npcView.OnDispose();
                    objMgr.Release(npcView.gameObject);
                }
            }
        }

        public override void OnDispose()
        {
        }
    }
}