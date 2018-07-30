using UnityEngine;
using System.Collections;
using Common;
using FirClient.Manager;

namespace FirClient.Ctrl
{
    public class LoginCtrl : BaseCtrl
    {
        private LoginPanel panel;

        public override void OnAwake()
        {
            base.OnAwake();
            panelMgr.CreatePanel(CtrlNames.Login, OnCreateOK);
        }

        /// <summary>
        /// 创建UI
        /// </summary>
        /// <param name="go"></param>
        private void OnCreateOK(GameObject go)
        {
            panel = go.AddComponent<LoginPanel>();
            panel.btnButton.onClick.AddListener(OnLoginClick);

            RectTransform rect = go.transform as RectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition3D = Vector3.zero;
        }

        void OnLoginClick()
        {
            ///开始连接服务器
            networkMgr.Connect(delegate()
            {
                levelMgr.LoadLevel(LevelType.Main);
            });
            Debugger.Log("OnLoginClick...");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            panelMgr.DestroyPanel(CtrlNames.Login);
        }
    }
}