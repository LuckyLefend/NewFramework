using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Security;
using System.Collections.Generic;
using System;

public enum MapType {
    Normal,
    Grid,
}

namespace FirClient.Manager
{
    public class MapManager : BaseManager
    {
        private IMapScene currMap;

        private Transform content;
        public Transform Content
        {
            get
            {
                if (content == null)
                {
                    GameObject mapView = GameObject.FindWithTag("MapView");
                    content = mapView.transform;
                }
                return content;
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

        /// <summary>
        /// 创建地图
        /// </summary>
        public void CreateMap(string name, Action<GameObject> creatOK)
        {
            currMap = new MapScene();
            currMap.CreateMap(name, creatOK);
        }

        /// <summary>
        /// 载入地图事件
        /// </summary>
        public void CreateMapEvent()
        {
            if (currMap != null)
            {
                currMap.CreateMapEvent();
            }
        }

        /// <summary>
        /// 获取地图信息
        /// </summary>
        public MapInfo GetMapInfo()
        {
            if (currMap != null)
            {
                return currMap.GetMapInfo();
            }
            return null;
        }

        /// <summary>
        /// 获取地图层
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Tilemap GetTileMap(string name)
        {
            //if (currMap != null)
            //{
            //    var map = currMap.GetMapInfo();
            //    if (map != null)
            //    {
            //        var tilemaps = currMap.GetMapInfo();
            //        for (int i = 0; i < tilemaps.Count; i++)
            //        {
            //            if (tilemaps[i].name == name)
            //            {
            //                return tilemaps[i];
            //            }
            //        }
            //    }
            //}
            return null;
        }

        /// <summary>
        /// 试着移动
        /// </summary>
        public bool TryMoveIt(int row, int col, out Vector2 outPos)
        {
            if (currMap != null)
            {
                return currMap.TryMoveIt(row, col, out outPos);
            }
            else
            {
                outPos = Vector2.zero;
                return false;
            }
        }

        /// <summary>
        /// 进入场景事件
        /// </summary>
        public void OnEnterMapEvent()
        {
            if (currMap != null)
            {
                currMap.OnEnterMapEvent();
            }
        }

        /// <summary>
        /// 离开场景事件
        /// </summary>
        public void OnLevelMapEvent()
        {
            if (currMap != null)
            {
                currMap.OnLevelMapEvent();
            }
        }

        /// <summary>
        /// 瓦片场景事件
        /// </summary>
        /// <param name="events"></param>
        public void OnTileDataEvent(List<MapDataEvent> events)
        {
            if (currMap != null)
            {
                currMap.OnTileDataEvent(events);
            }
        }

        public override void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}