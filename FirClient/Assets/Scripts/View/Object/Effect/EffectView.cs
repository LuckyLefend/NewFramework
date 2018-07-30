using System.Collections;
using System.Collections.Generic;
using Anthill.Animation;
using FirClient.Manager;
using UnityEngine;

namespace FirClient.View
{
    public class EffectView : ObjectView
    {
        private AntActor antActor;
        private EffectData data;
        private GameObject gameObj;
        private Vector3 spawnPos;

        public void Initialize(EffectData data, Vector3 spawnPos)
        {
            this.data = data;
            this.spawnPos = spawnPos;
        }

        public override void OnAwake()
        {
            transform.localPosition = spawnPos;
            gameObject.name = "Effect_" + 1;
            gameObject.SetActive(true);

            gameObj = objMgr.Get(data.name);
            gameObj.transform.SetParent(transform);
            gameObj.transform.localScale = data.scale;
            gameObj.transform.localPosition = Vector3.zero;
            gameObj.SetActive(true);

            var spriteRender = gameObj.GetComponent<SpriteRenderer>();
            spriteRender.sortingOrder = LayerMask.NameToLayer("Effect");
            spriteRender.sortingLayerName = "Effect";

            antActor = gameObj.GetComponent<AntActor>();
            antActor.timeScale = 0.8f;
            antActor.EventAnimationComplete += OnCompleted;

            soundMgr.PlaySound("Sounds/" + data.sound);
        }

        void OnCompleted(AntActor aActor, string aAnimationName)
        {
            this.OnDispose();
        }

        public override void OnDispose()
        {
            Destroy(viewObject);
            objMgr.Release(gameObj);
            objMgr.Release(gameObject);
            antActor.EventAnimationComplete -= OnCompleted;
            this.data = null;
        }
    }

}
