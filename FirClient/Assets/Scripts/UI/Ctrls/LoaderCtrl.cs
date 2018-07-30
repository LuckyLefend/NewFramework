using UnityEngine;
using System.Collections;
using System;
using Common;
using FirClient.Manager;

namespace FirClient.Ctrl
{
    public class LoaderCtrl : BaseCtrl
    {
        private Action actInit;
        private GameObject gameObj;

        public override void OnAwake()
        {
            base.OnAwake();
            panelMgr.CreatePanel(UILayer.Top, CtrlNames.Loader, OnCreateOK);
        }

        private void OnCreateOK(GameObject go)
        {
            gameObj = go;
            RectTransform trans = go.transform as RectTransform;
            trans.offsetMin = Vector2.zero;
            trans.offsetMax = Vector2.zero;

            if (actInit != null)
            {
                actInit();
            }
        }

        /// <summary>
        /// 初始化Loader
        /// </summary>
        public void InitLoader(Action func)
        {
            actInit = func;
            this.OnAwake();
        }

        public void OnLeaveLevel(LevelType level, Action func)
        {
            var handler = levelMgr.GetLevelHander(level);
            if (handler != null)
            {
                handler.OnLeaveLevel();
            }
            if (func != null)
            {
                func();
            }
            Debugger.Log("OnLeaveLevel--->>" + level);
        }

        /// <summary>
        /// 进入新场景后，初始化所有数据
        /// </summary>
        /// <param name="levelName"></param>
        /// <param name="func"></param>
        public void OnEnterLevel(LevelType level, Action func)
        {
            var handler = levelMgr.GetLevelHander(level);
            if (handler != null)
            {
                handler.OnEnterLevel();
            }
            if (func != null)
            {
                func();
            }
            Debugger.Log("OnEnterLevel--->>" + level);
        }

        public void CloseLoader()
        {
            panelMgr.DestroyPanel(CtrlNames.Loader);
        }
    }
}