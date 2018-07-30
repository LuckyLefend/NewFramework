using UnityEngine;
using System.Collections;
using Common;
using System.Collections.Generic;
using UnityEngine.UI;
using FirClient.Manager;
using FirClient.View;

namespace FirClient.Ctrl
{
    public class MainCtrl : BaseCtrl
    {
        private MainPanel mainView;
        private GameObject gameObj;

        // Use this for initialization
        public override void OnAwake()
        {
            panelMgr.CreatePanel(CtrlNames.Main, OnCreateOK);
        }

        /// <summary>
        /// 创建对象完成
        /// </summary>
        /// <param name="obj"></param>
        private void OnCreateOK(GameObject obj)
        {
            gameObj = obj;
            var trans = obj.transform as RectTransform;
            trans.offsetMin = Vector2.zero;
            trans.offsetMax = Vector2.zero;

            mainView = obj.AddComponent<MainPanel>();
            mainView.btnBattle.onClick.AddListener(OnStartBattle);
            Debugger.LogWarning("OnCreateOK---->>>>" + obj.name);
        }

        void OnSettingClick()
        {
            Debugger.Log("OnSettingClick--->>>");
        }

        void OnStartBattle()
        {
            levelMgr.LoadLevel(LevelType.Battle);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            panelMgr.DestroyPanel(CtrlNames.Main);
        }
    }
}