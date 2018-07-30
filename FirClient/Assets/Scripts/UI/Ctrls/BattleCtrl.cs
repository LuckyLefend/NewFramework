using System.Collections;
using System.Collections.Generic;
using Common;
using FirClient.View;
using UnityEngine;

namespace FirClient.Ctrl
{
    public class BattleCtrl : BaseCtrl
    {
        private Minimap miniMap;
        private GameObject gameObj;
        private List<NPCView> cachedNpcView = new List<NPCView>();

        public override void OnAwake()
        {
            base.OnAwake();
            base.isFrameUpdate = true;

            panelMgr.CreatePanel(CtrlNames.Battle, OnCreateOK);
        }

        private void OnCreateOK(GameObject obj)
        {
            this.gameObj = obj;
            this.InitEnvir();
            var battleView = obj.AddComponent<BattlePanel>();
            battleView.btnBattle.onClick.AddListener(OnOpenFire);

            this.InitJoystick();
            //////////////////////////////////////////////////////
            this.miniMap = new Minimap(battleView);
            for (int i = 0; i < cachedNpcView.Count; i++)
            {
                var view = cachedNpcView[i];
                if (view != null)
                {
                    AddMiniMapItem(view);
                }
            }
            cachedNpcView.Clear();

            var trans = obj.transform as RectTransform;
            trans.offsetMin = Vector2.zero;
            trans.offsetMax = Vector2.zero;

            var mainCamera = io.manager.transform.Find("MainCamera/Camera");
            if (mainCamera != null)
            {
                mainCamera.GetComponent<Camera>().enabled = true;
            }
        }

        /// <summary>
        /// 开火
        /// </summary>
        private void OnOpenFire()
        {
            Messenger.Broadcast(EventNames.BtnOpenFire);
        }

        /// <summary>
        /// 初始化摇杆
        /// </summary>
        void InitJoystick()
        {
            if (gameObj != null)
            {
                var joystick = gameObj.transform.Find("Joystick");
                joystick.gameObject.AddComponent<Joystick>();
            }
        }

        /// <summary>
        /// 初始化环境
        /// </summary>
        void InitEnvir()
        {
            var mapInfo = mapMgr.GetMapInfo();
            var gameView = GameObject.FindWithTag("GameView");
            if (gameView != null && mapInfo != null)
            {
                var rectView = gameView.transform as RectTransform;
                rectView.sizeDelta = new Vector2(mapInfo.mapWidth, mapInfo.mapHeight);
            }
            //更新
            uiMgr.UpdateMapAbout();
        }

        /// <summary>
        /// 帧更新
        /// </summary>
        /// <param name="deltaTime"></param>
        //public override void OnFrameUpdate(float deltaTime)
        //{
        //    base.OnFrameUpdate(deltaTime);
        //    if (miniMap != null)
        //    {
        //        miniMap.UpdateMiniMapPosition();
        //    }
        //}

        /// <summary>
        /// 初始化小地图
        /// </summary>
        public void AddMiniMapItem(NPCView npcView)
        {
            if (miniMap == null)
            {
                cachedNpcView.Add(npcView);
            }
            else
            {
                //miniMap.AddMinimap(npcView);
            }
        }

        public void RemoveMiniMapItem(NPCView npcView)
        {
            if (miniMap != null)
            {
                miniMap.RemoveMinimap(npcView);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            panelMgr.DestroyPanel(CtrlNames.Battle);
        }
    }
}