using UnityEngine;
using System.Collections;
using Common;
using FirClient.Manager;

namespace FirClient.Ctrl {
    public class MessageCtrl : BaseCtrl {
        private GameObject gameObj;
        private MessagePanel panel;

        // Use this for initialization
        public override void OnAwake() {
            panelMgr.CreatePanel(CtrlNames.Message, OnCreateOK);
        }

        /// <summary>
        /// 创建对象完成
        /// </summary>
        /// <param name="go"></param>
        private void OnCreateOK(GameObject go) {
            gameObj = go;
            panel = go.AddComponent<MessagePanel>();
            panel.btnButton.onClick.AddListener(OnCloseButton);
            Debug.LogWarning("OnCreateOK---->>>>" + go.name);
        }

        /// <summary>
        /// 单击关闭按钮
        /// </summary>
        void OnCloseButton() {
            if (gameObj != null) {
                GameObject.Destroy(gameObj);
            }
            Debug.LogWarning("OnCloseButton------>>>>");
        }
    }
}