using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace FirClient.Manager
{
    public enum UILayer
    {
        MapAbout = 10,   //地图相关
        Common = 11,     //公用层
        Effect = 12,     //特效层
        Movie = 13,      //电影层
        Top = 14,        //顶层
    }

    public enum UILayerLayout 
    {
        Stretch,
        LeftBottom,
    }

    public class UIManager : BaseManager
    {
        private Transform mapAboutView;
        private GameObject prefabHealthBar;
        private GameObject prefabNameHint;
        private RectTransform rectNameHint;
        private RectTransform rectHealthBar;

        private Dictionary<UILayer, Transform> mlayers = new Dictionary<UILayer, Transform>();
        private Dictionary<string, GameObject> mNameHints = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> mHealthBars = new Dictionary<string, GameObject>();

        /// <summary>
        /// 初始化层
        /// </summary>
        public override void Initialize()
        {
            mlayers.Add(UILayer.MapAbout, CreateLayer(UILayer.MapAbout, UILayerLayout.LeftBottom));
            mlayers.Add(UILayer.Common, CreateLayer(UILayer.Common));
            mlayers.Add(UILayer.Movie, CreateLayer(UILayer.Movie));
            mlayers.Add(UILayer.Effect, CreateLayer(UILayer.Effect));
            mlayers.Add(UILayer.Top, CreateLayer(UILayer.Top));

            this.InitMapAbout();
        }

        public override void OnFixedUpdate(float deltaTime)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// 创建层
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        Transform CreateLayer(UILayer layerType, UILayerLayout layout = UILayerLayout.Stretch)
        {
            string layerName = layerType.ToString() + "_Layer";
            GameObject layer = new GameObject(layerName);
            layer.layer = LayerMask.NameToLayer("UI");
            layer.transform.SetParent(io.uiCanvas.transform);
            layer.transform.localScale = Vector3.one;

            var rect = layer.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            switch (layout)
            {
                case UILayerLayout.Stretch:
                    rect.anchorMax = Vector2.one;
                    rect.sizeDelta = Vector2.zero;
                break;
                case UILayerLayout.LeftBottom:
                    rect.anchorMax = Vector2.zero;
                    rect.sizeDelta = new Vector2(1136, 640);
                break;
            }
            rect.anchoredPosition3D = Vector3.zero;
            rect.SetSiblingIndex((int)layerType);

            return layer.transform;
        }

        /// <summary>
        /// 获取一个层
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Transform GetLayer(UILayer type)
        {
            Transform layer = null;
            mlayers.TryGetValue(type, out layer);
            return layer;
        }

        /// <summary>
        /// 初始化地图相关
        /// </summary>
        public void InitMapAbout()
        {
            mapAboutView = GetLayer(UILayer.MapAbout);
            var scaler = io.uiCanvas.GetComponent<CanvasScaler>();

            ////HealthBar
            GameObject healthBar = new GameObject("HealthBars");
            healthBar.transform.SetParent(mapAboutView);
            healthBar.transform.localScale = Vector3.one;
            healthBar.layer = LayerMask.NameToLayer("UI");
            rectHealthBar = healthBar.AddComponent<RectTransform>();
            rectHealthBar.anchorMin = new Vector2(0f, 0f);
            rectHealthBar.anchorMax = new Vector2(0f, 0f);
            rectHealthBar.sizeDelta = scaler.referenceResolution;

            prefabHealthBar = resMgr.LoadAsset<GameObject>("Prefabs/HUD/HealthBar");

            ////NameHint
            GameObject nameHint = new GameObject("NameHints");
            nameHint.transform.SetParent(mapAboutView);
            nameHint.transform.localScale = Vector3.one;
            nameHint.layer = LayerMask.NameToLayer("UI");
            rectNameHint = nameHint.AddComponent<RectTransform>();
            rectNameHint.anchorMin = new Vector2(0f, 0f);
            rectNameHint.anchorMax = new Vector2(0f, 0f);
            rectHealthBar.sizeDelta = scaler.referenceResolution;

            prefabNameHint = resMgr.LoadAsset<GameObject>("Prefabs/HUD/NameText");
        }

        public void UpdateMapAbout()
        {
            var mapInfo = mapMgr.GetMapInfo();
            if (mapInfo != null)
            {
                if (rectHealthBar != null)
                {
                    rectHealthBar.sizeDelta = new Vector2(mapInfo.mapWidth, mapInfo.mapHeight);
                }
                if (rectNameHint != null)
                {
                    rectNameHint.sizeDelta = new Vector2(mapInfo.mapWidth, mapInfo.mapHeight);
                }
            }
        }

        /// <summary>
        /// 添加血条
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gameObj"></param>
        public void AddHealthBar(string name, Transform gameObj)
        {
            var healthBar = Instantiate<GameObject>(prefabHealthBar);
            healthBar.name = name;
            healthBar.transform.SetParent(rectHealthBar);
            healthBar.transform.localScale = Vector3.one;
            healthBar.transform.localPosition = Vector3.zero;

            var healthRect = healthBar.transform as RectTransform;
            healthRect.anchorMin = new Vector2(0f, 0f);
            healthRect.anchorMax = new Vector2(0f, 0f);

            var hud = healthBar.AddComponent<HUDObject>();
            hud.InitHUD(HUDType.HealthBar, gameObj);

            mHealthBars.Add(name, healthBar);
        }

        public void RemoveHealthBar(string name)
        {
            if (mHealthBars.ContainsKey(name))
            {
                GameObject gameObj = mHealthBars[name];
                if (gameObj != null)
                {
                    Destroy(gameObj);
                    mHealthBars.Remove(name);
                }
            }
        }

        /// <summary>
        /// 添加名称
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gameObj"></param>
        public void AddNameHint(string name, Transform gameObj)
        {
            var nameHint = Instantiate<GameObject>(prefabNameHint);
            nameHint.name = name;
            nameHint.transform.SetParent(rectNameHint);
            nameHint.transform.localScale = Vector3.one;
            nameHint.transform.localPosition = Vector3.zero;

            var nameRect = nameHint.transform as RectTransform;
            nameRect.anchorMin = new Vector2(0f, 0f);
            nameRect.anchorMax = new Vector2(0f, 0f);

            var hud = nameHint.AddComponent<HUDObject>();
            hud.InitHUD(HUDType.NameHint, gameObj);

            nameHint.GetComponent<Text>().text = name;
            mNameHints.Add(name, nameHint);
        }

        /// <summary>
        /// 移除名称
        /// </summary>
        /// <param name="name"></param>
        public void RemoveNameHint(string name)
        {
            if (mNameHints.ContainsKey(name))
            {
                GameObject gameObj = mNameHints[name];
                if (gameObj != null)
                {
                    Destroy(gameObj);
                    mNameHints.Remove(name);
                }
            }
        }

        public override void OnDispose()
        {
            throw new System.NotImplementedException();
        }
    }
}