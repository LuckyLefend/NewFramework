using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using UnityEngine;
using Utility;

namespace MasterServer.Manager
{
    public class ConfigManager : BaseBehaviour, IManager
    {
        public Dictionary<string, Vector3> HeroSpawnPoints = new Dictionary<string, Vector3>();
        public Dictionary<string, Vector3> MonsterSpawnPoints = new Dictionary<string, Vector3>();

        public ConfigManager()
        {
            ConfigMgr = this;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            InitMapConfig();
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        void InitMapConfig()
        {
            HeroSpawnPoints.Clear();
            MonsterSpawnPoints.Clear();
            var xml = XmlHelper.LoadXml("Config/Mapdata/Map001");
            for (int i = 0; i < xml.Children.Count; i++)
            {
                var child = xml.Children[i] as SecurityElement;
                if (child.Tag == "heroSpawnPos")
                {
                    for (int j = 0; j < child.Children.Count; j++)
                    {
                        var sun = child.Children[j] as SecurityElement;
                        var name = sun.Attribute("name");
                        var pos_x = float.Parse(sun.Attribute("pos_x"));
                        var pos_y = float.Parse(sun.Attribute("pos_y"));
                        var pos_z = float.Parse(sun.Attribute("pos_z"));
                        HeroSpawnPoints.Add(name, new Vector3(pos_x, pos_y, pos_z));
                    }
                }
                if (child.Tag == "monsterSpawnPos")
                {
                    for (int j = 0; j < child.Children.Count; j++)
                    {
                        var sun = child.Children[j] as SecurityElement;
                        var name = sun.Attribute("name");
                        var pos_x = float.Parse(sun.Attribute("pos_x"));
                        var pos_y = float.Parse(sun.Attribute("pos_y"));
                        var pos_z = float.Parse(sun.Attribute("pos_z"));
                        MonsterSpawnPoints.Add(name, new Vector3(pos_x, pos_y, pos_z));
                    }
                }
            }
            Log.Warn("InitMapConfig OK!!! HeroSpawnPoints:>" + HeroSpawnPoints.Count + " MonsterSpawnPoints:>" + MonsterSpawnPoints.Count);
        }

        public void OnFrameUpdate()
        {
        }

        public void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
