using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Common;
using System;
using FirClient.Ctrl;
using FirClient.Manager;
using Anthill.Animation;
using FirClient.Controller;
using FirClient.Animator;

public enum DirType { 
    Left,
    Right,
    Top,
    Bottom
}

public class NetState {
    public float timestamp;
    public Vector2 pos;
    public Quaternion rot;

    public NetState() {
        timestamp = 0.0f;
        pos = Vector3.zero;
        rot = Quaternion.identity;
    }
     
    public NetState(float time , Vector3 pos, Quaternion rot) {
        timestamp = time;
        this.pos = pos;
        this.rot = rot;
    }
}

namespace FirClient.View
{
    public class TankView : NPCView
    {
        private bool canMove = false;
        private float moveSpeed = 2f;
        private float refTime = 0f;
        private float syncTime = 0f;
        private float syncDelay = 0f;
        private float syncInterval = 0f;
        private float lastSyncTime = 0f;
        private float pingMargin = 0.5f;
        private float positionErrorThreshold = 0.2f;

        private Vector3 syncPos = Vector2.zero;
        private Vector3 startPos;

        private Vector3 endPos;
        private Vector3 lastPos;
        private SmoothFollow follow;
        private SmoothFollow miniCamera;
        private BoxCollider2D collider;
        private NetState[] serverStateBuffer = new NetState[20];

        SmoothFollow FollowCamera
        {
            get
            {
                if (follow == null)
                {
                    var mainCameraView = GameObject.FindWithTag("MainCameraView");
                    if (mainCameraView != null)
                    {
                        follow = mainCameraView.AddComponent<SmoothFollow>();
                    }
                }
                return follow;
            }
        }

        long timestamp
        {
            get
            {
                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
                return (long)Math.Round((DateTime.Now - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);
            }
        }

        public override void OnStart()
        {
            syncInterval = (1000 / AppConst.SyncRate) / 1000;
            refTime = syncInterval;     //first send once

            if (base.IsOwner)
            {
                Joystick.OnJoystickStart += OnJoystickStart;
                Joystick.OnJoystickMove += OnJoystickMove;
                Joystick.OnJoystickEnd += OnJoystickEnd;

                Messenger.AddListener<Vector2>(EventNames.Joystick_Start, OnJoystickStart);
                Messenger.AddListener<Vector2, float>(EventNames.Joystick_Move, OnJoystickMove);
                Messenger.AddListener(EventNames.Joystick_End, OnJoystickEnd);

                handlerMgr.isRunning = true;
            }
            base.OnStart();
        }

        public override void OnUpdate()
        {
            if (base.IsOwner)
            {
                if (lastPos != Vector3.zero)
                {
                    rigidbody.MovePosition(lastPos);
                }
            }
            else
            {
                if (endPos != Vector3.zero)
                {
                    //syncTime += Time.fixedDeltaTime;
                    //float deltaTime = syncTime / syncDelay;
                    //rect.anchoredPosition = Vector2.Lerp(startPos, endPos, deltaTime);
                }
            }
            base.OnUpdate();
        }

        //void Update() {
        //    if (tno.isMine) {
        //        refTime += Time.deltaTime;
        //        if (refTime >= syncInterval) {
        //            refTime = 0;
        //            tno.Send("SyncMove", Target.OthersSaved, name, rect.anchoredPosition, rigidbody.velocity, timestamp, false);
        //        }
        //    } else {
        //        var clientPing = ((float)TNManager.ping / 100) + pingMargin;
        //        var interpolationTime = timestamp - clientPing;
        //        if (serverStateBuffer[0] == null) {
        //            serverStateBuffer[0] = new NetState(0, rect.anchoredPosition, Quaternion.identity);
        //        }
        //        //Try interpolation if possible.
        //        //If the latest serverStateBuffer timestamp is smaller than the latencytime))
        //        //we're not slow enough to really lag out and just extrapolate.
        //        //尝试插值如果可能的话。
        //        //如果最近serverStateBuffer时间戳小于延迟时间
        //        //我们不是足够缓慢滞后,只是推断。
        //        if (serverStateBuffer[0].timestamp > interpolationTime) {
        //            for (int i = 0; i < serverStateBuffer.Length; i++) {
        //                if (serverStateBuffer[i] == null) {
        //                    continue;
        //                }
        //                // Find the state which matches the interp. time or use last state
        //                // 找到匹配插值函数的状态。时间或使用最后的状态
        //                if (serverStateBuffer[i].timestamp <= interpolationTime || i == serverStateBuffer.Length - 1) {
        //                    // The state one frame newer than the best playback state
        //                    // 这一帧更新比最好的重放状态
        //                    var bestTarget = serverStateBuffer[Mathf.Max(i - 1, 0)];

        //                    // The best playback state (closest current network time))
        //                    // 最好的重放状态(接近当前的网络时间))
        //                    var bestStart = serverStateBuffer[i];
        //                    var timediff = bestTarget.timestamp - bestStart.timestamp;
        //                    var lerpTime = 0.0F;

        //                    // Increase the interpolation amount by growing ping
        //                    // Reverse that for more smooth but less accurate positioning
        //                    // 插值数量增加越来越平逆转,光滑但不精确定位
        //                    if (timediff > 0.0001) {
        //                        lerpTime = ((interpolationTime - bestStart.timestamp) / timediff);
        //                    }
        //                    var newPos = Vector2.Lerp(bestStart.pos, bestTarget.pos, lerpTime);
        //                    FixedPos(ref newPos);
        //                    rect.anchoredPosition = newPos;
        //                    //found our way through to lerp the positions, lets return here
        //                    //发现在插值的位置,我们的方法可以这里返回
        //                    return;
        //                }
        //            }
        //        } else {
        //            var latest = serverStateBuffer[0];
        //            var distance = Vector2.Distance(rect.anchoredPosition, latest.pos);
        //            if (distance >= positionErrorThreshold) {
        //                var lerp = ((1 / distance) * 250f) / 100f;
        //                var newPos = Vector2.Lerp(rect.anchoredPosition, latest.pos, lerp);
        //                FixedPos(ref newPos);
        //                rect.anchoredPosition = newPos;
        //            }
        //        }
        //    }
        //}

        void OnJoystickStart(Vector2 vec)
        {
            Debugger.Log("OnJoystickStart-->>" + vec);
        }

        void OnJoystickMove(Vector2 vec, float angle)
        {
            base.OnRotate(angle);
            lastPos = rigidbody.position + vec * moveSpeed * Time.deltaTime;

            //Debugger.Log("OnJoystickMove-->>" + lastPos);
        }

        void OnJoystickEnd()
        {
            lastPos = Vector3.zero;
            Debugger.Log("OnJoystickEnd-->>");
        }

        //void SyncMove(string target, Vector2 pos, Vector2 velocity, float timestamp, bool isDirect) {
        //    syncTime = 0f;
        //    syncDelay = Time.time - lastSyncTime;
        //    lastSyncTime = Time.time;
        //    if (velocity == Vector2.zero) {
        //        velocity = Vector2.one;
        //    }
        //    if (isDirect) {
        //        rect.anchoredPosition = pos;
        //    } else {
        //        startPos = rect.anchoredPosition;
        //        endPos = pos + velocity * syncDelay;

        //        //record netstate
        //        for (int i = serverStateBuffer.Length - 1; i >= 1; i-- ) {
        //            serverStateBuffer[i] = serverStateBuffer[i - 1];
        //        }
        //        //Override the first element with the latest server info
        //        serverStateBuffer[0] = new NetState(timestamp, pos, Quaternion.identity);
        //    }
        //    Debugger.LogWarning("target:>" + target + " velocity:>" + velocity + " outPos:>" + pos);
        //}

        /// <summary>
        /// 主角出生
        /// </summary>
        public override void OnAwake()
        {
            gameObject.name = "Player_" + NetworkId;
            gameObject.transform.SetLayer(AppConst.GameplayLayer);
            //gameObject.transform.position = spawnPos;

            roleObject = objMgr.Get(PoolNames.TANK);
            roleObject.transform.SetParent(transform);
            roleObject.transform.SetLayer(AppConst.GameplayLayer);
            roleObject.transform.localPosition = Vector3.zero;
            roleObject.SetActive(false);

            if (base.IsOwner)
            {
                FollowCamera.SetActiveTarget(transform, 0, 0);
            }
            //gameObject.tag = base.IsOwner ? "Character " : "Player";
            this.OnSpawnDone();
            base.OnAwake();
        }

        /// <summary>
        /// 出生结束
        /// </summary>
        void OnSpawnDone()
        {
            roleObject.SetActive(true);
            npcMgr.AddNpc(NetworkId, this);
            this.InitTankView();

            collider = gameObject.AddComponent<BoxCollider2D>();

            uiMgr.AddNameHint(gameObject.name, transform);
            uiMgr.AddHealthBar(gameObject.name, transform);

            var battleCtrl = io.GetCtrl<BattleCtrl>();
            if (battleCtrl != null)
            {
                battleCtrl.AddMiniMapItem(this);
            }
        }

        public void InitTankView()
        {
            var data = cfgMgr.GetTankData("tank1");
            var atlas = atlasMgr.GetAltas(data.atlas);

            var baseObj = roleObject.transform.Find("Base");
            baseObj.localPosition = data.base_pos;

            var baseRender = baseObj.GetComponent<SpriteRenderer>();
            baseRender.sprite = atlas.GetSprite(data.bases[0]);
            baseRender.material = atlas.AtlasMaterial;

            var anim = baseObj.gameObject.AddComponent<AntActor>();
            anim.animations = new AntActor.AntAnimation[1];

            anim.animations[0].name = "Move";
            anim.animations[0].frames = new Sprite[data.bases.Length];
            for (int i = 0; i < data.bases.Length; i++)
            {
                anim.animations[0].frames[i] = atlas.GetSprite(data.bases[i]);
            }
            anim.timeScale = 0.3f;
            anim.initialAnimation = "Move";
            anim.SwitchAnimation("Move");

            var armorObj = roleObject.transform.Find("Armor");
            armorObj.localPosition = data.armor_pos;

            var armorRender = armorObj.GetComponent<SpriteRenderer>();
            armorRender.sprite = atlas.GetSprite(data.armor);
            armorRender.material = atlas.AtlasMaterial;

            var canonObj = roleObject.transform.Find("Canon");
            canonObj.localPosition = data.canon_pos;

            var canonRender = canonObj.GetComponent<SpriteRenderer>();
            canonRender.sprite = atlas.GetSprite(data.canon);
            canonRender.material = atlas.AtlasMaterial;

            roleObject.transform.localScale = data.scale;
            animObject = roleObject.AddComponent<TankAnimator>();
        }

        /// <summary>
        /// 主角死亡
        /// </summary>
        public override void OnDispose()
        {
            base.OnDispose();
            uiMgr.RemoveNameHint(gameObject.name);
            uiMgr.RemoveHealthBar(gameObject.name);

            var battleCtrl = io.GetCtrl<BattleCtrl>();
            if (battleCtrl != null)
            {
                battleCtrl.RemoveMiniMapItem(this);
            }
            if (animObject != null)
            {
                Destroy(animObject);
            }
            if (roleObject != null)
            {
                var baseObj = roleObject.transform.Find("Base");
                var anim = baseObj.GetComponent<AntActor>();
                if (anim != null)
                {
                    Destroy(anim);
                }
                objMgr.Release(roleObject);
            }
            if (collider != null)
            {
                Destroy(collider);
            }
            objMgr.Release(gameObject);
        }

        void OnUpdate(Tweener tweener, RectTransform rectOther, AnimalView monster)
        {
            var distance = Vector3.Distance(transform.position, rectOther.anchoredPosition);
            if (distance <= 30)
            {
                tweener.Kill();
                objMgr.Release(monster.gameObject);
                soundMgr.PlaySound("Sounds/sfx_click");
            }
        }

        void OnDestroy()
        {
            if (base.IsOwner)
            {
                Joystick.OnJoystickStart -= OnJoystickStart;
                Joystick.OnJoystickMove -= OnJoystickMove;
                Joystick.OnJoystickEnd -= OnJoystickEnd;

                Messenger.RemoveListener<Vector2>(EventNames.Joystick_Start, OnJoystickStart);
                Messenger.RemoveListener<Vector2, float>(EventNames.Joystick_Move, OnJoystickMove);
                Messenger.RemoveListener(EventNames.Joystick_End, OnJoystickEnd);
            }
        }
    }
}