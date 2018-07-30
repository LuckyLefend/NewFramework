using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

namespace FirClient.Manager
{
    public class ConfigManager : BaseManager
    {
        Dictionary<string, TankData> tankDatas = new Dictionary<string, TankData>();
        Dictionary<string, AnimalData> animalDatas = new Dictionary<string, AnimalData>();
        Dictionary<string, BulletData> bulletDatas = new Dictionary<string, BulletData>();
        Dictionary<string, EffectData> effectDatas = new Dictionary<string, EffectData>();

        public override void Initialize()
        {
            this.InitNpcData();
            this.InitItemData();
        }

        public override void OnFixedUpdate(float deltaTime)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// 初始化NPC数据
        /// </summary>
        void InitNpcData()
        {
            string tankDataPath = "Datas/Npcs";
            var asset = XmlHelper.LoadXml(tankDataPath);
            if (asset != null)
            {
                for (int i = 0; i < asset.Children.Count; i++)
                {
                    var root = asset.Children[i] as SecurityElement;
                    if (root != null)
                    {
                        if (root.Tag == "tank")
                        {
                            for (int j = 0; j < root.Children.Count; j++)
                            {
                                var item = root.Children[j] as SecurityElement;
                                TankData data = new TankData();
                                data.name = item.Attribute("name");
                                data.atlas = item.Attribute("atlas");

                                data.bases = item.Attribute("base").Split('|');
                                var base_postr = item.Attribute("base_pos");
                                if (base_postr == null)
                                {
                                    base_postr = "0_0_0";
                                }
                                var base_pos = base_postr.Split('_');
                                data.base_pos = new Vector3(float.Parse(base_pos[0]), float.Parse(base_pos[1]), float.Parse(base_pos[2]));

                                data.armor = item.Attribute("armor");
                                var armor_postr = item.Attribute("armor_pos");
                                if (armor_postr == null)
                                {
                                    armor_postr = "0_0_0";
                                }
                                var armor_pos = armor_postr.Split('_');
                                data.armor_pos = new Vector3(float.Parse(armor_pos[0]), float.Parse(armor_pos[1]), float.Parse(armor_pos[2]));

                                data.canon = item.Attribute("canon");
                                var canon_postr = item.Attribute("canon_pos");
                                if (canon_postr == null)
                                {
                                    canon_postr = "0_0_0";
                                }
                                var canon_pos = canon_postr.Split('_');
                                data.canon_pos = new Vector3(float.Parse(canon_pos[0]), float.Parse(canon_pos[1]), float.Parse(canon_pos[2]));

                                var scale_str = item.Attribute("scale");
                                if (scale_str == null)
                                {
                                    scale_str = "1_1_1";
                                }
                                var scale = scale_str.Split('_');
                                data.scale = new Vector3(float.Parse(scale[0]), float.Parse(scale[1]), float.Parse(scale[2]));

                                tankDatas.Add(data.name, data);
                            }

                        }
                        if (root.Tag == "animal")
                        {
                            for (int j = 0; j < root.Children.Count; j++)
                            {
                                var item = root.Children[j] as SecurityElement;
                                AnimalData data = new AnimalData();
                                data.name = item.Attribute("name");
                                data.atlas = item.Attribute("atlas");

                                data.frame = item.Attribute("frame");
                                var frame_postr = item.Attribute("frame_pos");
                                if (frame_postr == null)
                                {
                                    frame_postr = "0_0_0";
                                }
                                var frame_pos = frame_postr.Split('_');
                                data.frame_pos = new Vector3(float.Parse(frame_pos[0]), float.Parse(frame_pos[1]), float.Parse(frame_pos[2]));

                                var frame_scale_str = item.Attribute("frame_size");
                                if (frame_scale_str == null)
                                {
                                    frame_scale_str = "1_1_1";
                                }
                                var frame_scale = frame_scale_str.Split('_');
                                data.frame_size = new Vector3(float.Parse(frame_scale[0]), float.Parse(frame_scale[1]), float.Parse(frame_scale[2]));

                                data.body = item.Attribute("body");
                                var body_postr = item.Attribute("body_pos");
                                if (body_postr == null)
                                {
                                    body_postr = "0_0_0";
                                }
                                var body_pos = body_postr.Split('_');
                                data.body_pos = new Vector3(float.Parse(body_pos[0]), float.Parse(body_pos[1]), float.Parse(body_pos[2]));

                                var scale_str = item.Attribute("scale");
                                if (scale_str == null)
                                {
                                    scale_str = "1_1_1";
                                }
                                var scale = scale_str.Split('_');
                                data.scale = new Vector3(float.Parse(scale[0]), float.Parse(scale[1]), float.Parse(scale[2]));

                                animalDatas.Add(data.name, data);
                            }
                        }
                    }
                }
            }
        }

        public TankData GetTankData(string name)
        {
            TankData data = null;
            tankDatas.TryGetValue(name, out data);
            return data;
        }

        public AnimalData GetAnimalData(string name)
        {
            AnimalData data = null;
            animalDatas.TryGetValue(name, out data);
            return data;
        }

        /// <summary>
        /// 初始化道具数据
        /// </summary>
        void InitItemData()
        {
            string tankDataPath = "Datas/Items";
            var asset = XmlHelper.LoadXml(tankDataPath);
            if (asset != null)
            {
                for (int i = 0; i < asset.Children.Count; i++)
                {
                    var root = asset.Children[i] as SecurityElement;
                    if (root != null)
                    {
                        if (root.Tag == "bullet")
                        {
                            for (int j = 0; j < root.Children.Count; j++)
                            {
                                var item = root.Children[j] as SecurityElement;
                                BulletData data = new BulletData();
                                data.name = item.Attribute("name");
                                data.resource = item.Attribute("resource");
                                data.animName = item.Attribute("animName");
                                var scale_str = item.Attribute("scale");
                                if (scale_str == null)
                                {
                                    scale_str = "1_1_1";
                                }
                                var scale = scale_str.Split('_');
                                data.scale = new Vector3(float.Parse(scale[0]), float.Parse(scale[1]), float.Parse(scale[2]));
                                data.sound = item.Attribute("sound");
                                bulletDatas.Add(data.name, data);
                            }
                        }
                        if (root.Tag == "effect")
                        {
                            for (int j = 0; j < root.Children.Count; j++)
                            {
                                var item = root.Children[j] as SecurityElement;
                                EffectData data = new EffectData();
                                data.name = item.Attribute("name");
                                data.resource = item.Attribute("resource");
                                data.animName = item.Attribute("animName");
                                var scale_str = item.Attribute("scale");
                                if (scale_str == null)
                                {
                                    scale_str = "1_1_1";
                                }
                                var scale = scale_str.Split('_');
                                data.scale = new Vector3(float.Parse(scale[0]), float.Parse(scale[1]), float.Parse(scale[2]));
                                data.sound = item.Attribute("sound");
                                effectDatas.Add(data.name, data);
                            }
                        }
                    }
                }
            }
        }

        public BulletData GetBulletData(string name)
        {
            BulletData data = null;
            bulletDatas.TryGetValue(name, out data);
            return data;
        }

        public Dictionary<string, BulletData> GetBulletList()
        {
            return bulletDatas;
        }

        public EffectData GetEffectData(string name)
        {
            EffectData data = null;
            effectDatas.TryGetValue(name, out data);
            return data;
        }

        public Dictionary<string, EffectData> GetEffectList()
        {
            return effectDatas;
        }

        public override void OnDispose()
        {
        }
    }
}
