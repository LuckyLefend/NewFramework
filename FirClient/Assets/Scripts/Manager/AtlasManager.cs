using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FirClient.Manager
{
    public class AtlasManager : BaseManager
    {
        private Dictionary<string, UIAtlas> atlases = new Dictionary<string, UIAtlas>();

        /// <summary>
        /// 添加一个图集
        /// </summary>
        public UIAtlas InitAtlas(string name, string path)
        {
            UIAtlas atlas = null;
            if (!atlases.ContainsKey(name))
            {
                atlas = new UIAtlas(path);
                atlases.Add(name, atlas);
            }
            else
            {
                atlas = atlases[name];
            }
            return atlas;
        }

        /// <summary>
        /// 获取图集
        /// </summary>
        public UIAtlas GetAltas(string name)
        {
            if (atlases.ContainsKey(name))
            {
                return atlases[name];
            }
            return null;
        }

        /// <summary>
        /// 图集是否存在
        /// </summary>
        public bool ExistAltas(string name)
        {
            return atlases.ContainsKey(name);
        }

        /// <summary>
        /// 移除图集
        /// </summary>
        /// <param name="name"></param>
        public void RemoveAtlas(string name)
        {
            if (atlases.ContainsKey(name))
            {
                atlases.Remove(name);
            }
        }

        public override void Initialize()
        {
        }

        public override void OnFixedUpdate(float deltaTime)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnDispose()
        {
        }
    }
}