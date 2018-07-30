using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace FirClient.Manager
{
    class PanelInfo
    {
        public UILayer layer;
        public GameObject panel;

        public PanelInfo(UILayer layer, GameObject panel)
        {
            this.layer = layer;
            this.panel = panel;
        }
    }

    public class PanelManager : BaseManager
    {
        private Dictionary<string, PanelInfo> m_panels = new Dictionary<string, PanelInfo>();
        private Dictionary<string, GameObject> m_prefabs = new Dictionary<string, GameObject>();

        public override void Initialize()
        {
        }

        public override void OnFixedUpdate(float deltaTime)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public void CreatePanel(string name, Action<GameObject> func)
        {
            StartCoroutine(OnCreatePanel(UILayer.Common, name, func));
        }

        public void CreatePanel(UILayer layer, string name, Action<GameObject> func)
        {
            StartCoroutine(OnCreatePanel(layer, name, func));
        }

        IEnumerator OnCreatePanel(UILayer layer, string name, Action<GameObject> func)
        {
            string assetName = name + "Panel";
            string path = "Prefabs/UI/" + assetName;

            GameObject prefab = null;
            m_prefabs.TryGetValue(assetName, out prefab);
            if (prefab == null)
            {
                prefab = resMgr.LoadAsset<GameObject>(path);
                m_prefabs.Add(assetName, prefab);
            }
            Transform parent = uiMgr.GetLayer(layer);
            if (parent.Find(name) != null || prefab == null)
            {
                yield break;
            }
            var go = Instantiate<GameObject>(prefab);
            go.name = assetName;
            go.transform.SetParent(parent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;

            m_panels.Add(assetName, new PanelInfo(layer, go));

            if (func != null)
            {
                func(go);   //
            }
            Debugger.LogWarning("CreatePanel::>> " + name + " " + prefab);
        }

        public void DestroyPanel(string name)
        {
            string assetName = name + "Panel";
            var trans = m_panels[assetName];
            if (trans != null)
            {
                Destroy(trans.panel);
            }
            m_panels.Remove(assetName); //

            GameObject prefab = null;
            m_prefabs.TryGetValue(assetName, out prefab);
            if (prefab != null)
            {
                m_prefabs.Remove(assetName);
                UnloadAssetForPrefab(prefab);
            }
        }

        void UnloadAssetForPrefab(GameObject prefab)
        {
            if (prefab != null)
            {
                UnloadAssetType<Text>(prefab);
                UnloadAssetType<Image>(prefab);
            }
        }

        void UnloadAssetType<T>(GameObject prefab)
        {
            var components = prefab.GetComponentsInChildren<T>();
            if (components.Length > 0)
            {
                Type compType = typeof(T);
                var assets = new List<UnityEngine.Object>();
                for (int i = 0; i < components.Length; i++)
                {
                    var c = components[i];
                    if (compType == typeof(Image))
                    {
                        var image = c as Image;
                        if (image != null && !assets.Contains(image.sprite.texture))
                        {
                            assets.Add(image.sprite.texture);
                        }
                    }
                    else if (compType == typeof(Text))
                    {
                        var text = c as Text;
                        if (text != null && !assets.Contains(text.font))
                        {
                            assets.Add(text.font);
                        }
                    }
                }
                for (int i = 0; i < assets.Count; i++)
                {
                    if (assets[i] != null)
                    {
                        Resources.UnloadAsset(assets[i]);
                    }
                }
                assets = null;
            }
        }

        public override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}