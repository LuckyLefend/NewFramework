using System.Collections;
using System.Collections.Generic;
using FirClient.Manager;
using UnityEngine;
using DG.Tweening;
using LiteNetLib.Utils;

public enum AnimalType 
{
    None = 0,
    MeleeCreeps = 1,            //近战小兵
    RemoteSoldier = 2,          //远程小兵
    TheArtilleryCorps = 3,      //炮车兵
    SuperSoldier = 4,           //超级兵
}

namespace FirClient.View
{
    public class AnimalView : NPCView
    {
        private GameObject gameObj;
        private CircleCollider2D collider;

        public override void OnAwake()
        {
            gameObject.name = "Animal_" + NetworkId;

            gameObj = objMgr.Get(PoolNames.ANIMAL);
            gameObj.name = "Monster";
            gameObj.transform.SetParent(transform);
            gameObj.transform.localPosition = Vector3.zero;
            gameObj.transform.SetLayer(AppConst.GameplayLayer);

            base.OnAwake();

            collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            npcMgr.AddNpc(NetworkId, this);
        }

        public override void OnStart()
        {
            var animData = cfgMgr.GetAnimalData("animal1");
            var animAtlas = atlasMgr.GetAltas(animData.atlas);

            var frameObj = gameObj.transform.Find("Frame");
            frameObj.localPosition = animData.frame_pos;
            frameObj.localScale = animData.frame_size;

            var frameRender = frameObj.GetComponent<SpriteRenderer>();
            frameRender.sprite = animAtlas.GetSprite(animData.frame);
            frameRender.material = animAtlas.AtlasMaterial;
            frameRender.sortingOrder = LayerMask.NameToLayer(AppConst.GameplayLayer);
            frameRender.sortingLayerName = AppConst.GameplayLayer;

            ///----------------------------------------------------------
            var bodyObj = gameObj.transform.Find("Body");
            bodyObj.localPosition = animData.body_pos;

            var bodyRender = bodyObj.GetComponent<SpriteRenderer>();
            bodyRender.sprite = animAtlas.GetSprite(animData.body);
            bodyRender.material = animAtlas.AtlasMaterial;
            bodyRender.sortingOrder = LayerMask.NameToLayer(AppConst.GameplayLayer);
            bodyRender.sortingLayerName = AppConst.GameplayLayer;

            gameObj.transform.DOScale(0.9f, 1f).SetLoops(-1, LoopType.Yoyo);
            gameObj.SetActive(true);

            base.OnStart();
        }

        public override void OnTriggerEnter2D(Collider2D coll)
        {
            var name = coll.gameObject.name;
            var data = name.Split('_');
            var objId = long.Parse(data[1]);
            switch (data[0])
            {
                case "Bullet":
                    OnBullet(objId);
                break;
                case "Effect":
                    OnEffect(objId);
                break;
            }
        }

        void OnEffect(long objId)
        {
        }

        void OnBullet(long objId)
        {
            var bullet = bulletMgr.GetView(objId);
            if (bullet != null && bullet.IsOwner)
            {
                var writer = new NetDataWriter();
                writer.Put(objId);      //子弹ID
                writer.Put(NetworkId);  //目标ID
                networkMgr.SendData(Protocal.ReqHitTarget, writer);
            }
        }

        public override void OnDispose()
        {
            base.OnDispose();
            if (gameObj != null)
            {
                objMgr.Release(gameObj);
            }
            if (collider != null)
            {
                Destroy(collider);
            }
            objMgr.Release(gameObject);
        }
    }
}