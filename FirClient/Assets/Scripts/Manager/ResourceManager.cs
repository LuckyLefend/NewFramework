using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UObject = UnityEngine.Object;

namespace FirClient.Manager
{
    public enum AssetType
    {
        GameObject = 0,
        Sprite = 1,
        Audio = 2,
    }
    public class AssetBundleInfo
    {
        public AssetBundle m_AssetBundle;
        public int m_ReferencedCount;

        public AssetBundleInfo(AssetBundle assetBundle)
        {
            m_AssetBundle = assetBundle;
            m_ReferencedCount = 1;
        }
    }
    public class ResourceManager : BaseManager
    {
        private string[] m_Variants = { };
        private AssetBundle assetbundle;
        private Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();
        private Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles;

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            string uri = string.Empty;
            m_LoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
            uri = Util.DataPath + AppConst.AssetDirname;
        }

        public override void OnFixedUpdate(float deltaTime)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public UnityEngine.Object LoadAsset(string prefabPath)
        {
            return Resources.Load(prefabPath);
        }

        public T LoadAsset<T>(string path) where T : UObject
        {
            Object o = Resources.Load<T>(path);
            if (o == null)
            {
                return null;
            }
            return o as T;
        }

        public T[] LoadAssets<T>(string path) where T : UObject
        {
            return Resources.LoadAll<T>(path);
        }

        public ResourceRequest LoadAssetAsync<T>(string path) where T : UObject
        {
            return Resources.LoadAsync<T>(path);
        }

        /// <summary>
        /// 载入AssetBundle
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        AssetBundle LoadAssetBundle(string abName)
        {
            if (!abName.EndsWith(AppConst.ExtName))
            {
                abName += AppConst.ExtName;
            }
            AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);
            if (bundleInfo == null)
            {
                string uri = Util.DataPath + abName;
                Debug.LogWarning("LoadFile::>> " + uri);

                AssetBundle bundle = AssetBundle.LoadFromFile(uri); //关联数据的素材绑定
                bundleInfo = new AssetBundleInfo(bundle);
                m_LoadedAssetBundles.Add(abName, bundleInfo);
            }
            else
            {
                bundleInfo.m_ReferencedCount++;
            }
            return bundleInfo.m_AssetBundle;
        }

        // Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
        public AssetBundleInfo GetLoadedAssetBundle(string assetBundleName)
        {
            AssetBundleInfo bundle = null;
            m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle == null)
                return null;

            // No dependencies are recorded, only the bundle itself is required.
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
                return bundle;

            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies)
            {
                // Wait all the dependent assetBundles being loaded.
                AssetBundleInfo dependentBundle;
                m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null)
                    return null;
            }
            return bundle;
        }

        // Unload assetbundle and its dependencies.
        public void UnloadAssetBundle(string assetBundleName)
        {
            if (!assetBundleName.EndsWith(AppConst.ExtName))
            {
                assetBundleName += AppConst.ExtName;
            }
            assetBundleName = assetBundleName.ToLower();
            Debugger.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + assetBundleName);
        }

        /// <summary>
        /// 销毁资源
        /// </summary>
        void OnDestroy()
        {
            Debugger.Log("~ResourceManager was destroy!");
        }

        public override void OnDispose()
        {
            throw new System.NotImplementedException();
        }
    }
}