using System.Collections;
using System.Collections.Generic;
using FirClient.View;
using UnityEngine;

namespace FirClient.Manager
{
    public class EffectManager : BaseManager
    {
        private Dictionary<string, EffectView> mEffects = new Dictionary<string, EffectView>();

        public override void Initialize()
        {
            var effectList = cfgMgr.GetEffectList();
            foreach (var de in effectList)
            {
                var poolName = de.Value.name;
                var effectPrefab = resMgr.LoadAsset<GameObject>("Prefabs/Effect/" + de.Value.resource);
                objMgr.CreatePool(poolName, 5, 10, effectPrefab);
            }
        }

        public override void OnFixedUpdate(float deltaTime)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// 获取特效脚本
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EffectView GetView(string name)
        {
            EffectView view = null;
            mEffects.TryGetValue(name, out view);
            return view;
        }

        /// <summary>
        /// 播放一个特效
        /// </summary>
        /// <param name="name"></param>
        public EffectView Create(string name, Vector3 spawnPos)
        {
            var data = cfgMgr.GetEffectData(name);
            var effectObj = objMgr.Get(PoolNames.EFFECT);
            if (effectObj != null)
            {
                effectObj.transform.SetParent(io.battleObject);
                effectObj.transform.SetLayer("Effect");

                var effect = new EffectView();
                effect.Initialize(data, spawnPos);
                mEffects.Add(name, effect);

                effectObj.AddComponent<ViewObject>().BindView(effect);

                effect.OnAwake();
                return effect;
            }
            return null;
        }

        public override void OnDispose()
        {
            throw new System.NotImplementedException();
        }
    }
}