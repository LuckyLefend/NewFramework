using UnityEngine;
using System.Collections;
using FirClient.Manager;

namespace FirClient.View
{
    public class BulletView : ObjectView
    {
        private BulletData data;
        private GameObject gameObj;
        private Rigidbody2D rigidbody;

        public bool IsOwner = false;

        public void Initialize(BulletData data, long objId)
        {
            this.data = data;
            base.Initialize(objId);
        }

        public override void OnAwake()
        {
            transform.SetLayer("Effect");
            gameObject.name = "Bullet_" + NetworkId;
            gameObject.SetActive(true);

            gameObj = objMgr.Get(data.name);
            gameObj.transform.SetParent(transform);
            gameObj.transform.localScale = Vector3.one;
            gameObj.transform.localPosition = Vector3.zero;
            gameObj.transform.localEulerAngles = new Vector3(0, 0, 90);
            gameObj.SetActive(true);

            rigidbody = gameObject.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0;

            var spriteRender = gameObj.GetComponent<SpriteRenderer>();
            spriteRender.sortingOrder = LayerMask.NameToLayer("Effect");
            spriteRender.sortingLayerName = "Effect";

            soundMgr.PlaySound("Sounds/" + data.sound);
        }

        /// <summary>
        /// 开火
        /// </summary>
        public void OpenFire(Vector3 startPos, Vector3 eulerAngle, float speed, bool isOwner)
        {
            this.IsOwner = isOwner;
            transform.localPosition = startPos;
            transform.localEulerAngles = eulerAngle;
            rigidbody.AddForce(transform.up * speed, ForceMode2D.Force);

            //bullet.GetComponent<Rigidbody>().velocity = -transform.forward * 4;

            timerMgr.AddTimer("BulletView", 10f, 0f, delegate(object o)
            {
                this.OnDispose();
            });
        }

        /// <summary>
        /// 爆炸
        /// </summary>
        public void OnBomb(object param = null)
        {
            effectMgr.Create("effect1", transform.localPosition);
            this.OnDispose();   //回收子弹
        }

        public override void OnDispose()
        {
            IsOwner = false;

            timerMgr.RemoveTimer("BulletView");

            Destroy(rigidbody);
            Destroy(viewObject);
            objMgr.Release(gameObj);
            objMgr.Release(gameObject);
            this.data = null;
        }
    }
}