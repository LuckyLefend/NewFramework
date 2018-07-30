using System.Collections;
using System.Collections.Generic;
using FirClient.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace FirClient.View
{
    public class Minimap : BaseBehaviour
    {
        private static readonly object _lock = new object();

        private BattlePanel battleView;
        private Dictionary<NPCView, RectTransform> allMiniMaps;

        public Minimap(BattlePanel mainView)
        {
            this.battleView = mainView;
            this.allMiniMaps = new Dictionary<NPCView, RectTransform>();
        }

        public void AddMinimap(NPCView npcView)
        {
            lock (_lock)
            {
                var miniMap = CreateMiniMapItem(npcView.gameObject.name);
                miniMap.sizeDelta = new Vector2(10, 10);
                var minimg = miniMap.GetComponent<Image>();
                if (npcView.IsOwner)
                {
                }
                else
                {
                    minimg.color = Color.red;
                }
                allMiniMaps.Add(npcView, miniMap);
            }
        }

        public void RemoveMinimap(NPCView npcView)
        {
            lock (_lock)
            {
                if (allMiniMaps.ContainsKey(npcView))
                {
                    var rect = allMiniMaps[npcView];
                    if (rect != null)
                    {
                        GameObject.Destroy(rect.gameObject);
                    }
                    allMiniMaps.Remove(npcView);
                }
            }
        }

        /// <summary>
        /// 创建小地图项
        /// </summary>
        /// <param name="name"></param>
        RectTransform CreateMiniMapItem(string name)
        {
            //var prefab = mainView.prefabMapitem;
            //var mapObj = GameObject.Instantiate(prefab) as GameObject;
            //mapObj.name = name;
            //mapObj.transform.SetParent(mainView.objMinimap.transform);
            //mapObj.transform.localScale = Vector3.one;
            //mapObj.SetActive(true);

            //var rect = mapObj.transform as RectTransform;
            //rect.anchoredPosition = Vector2.zero;
            //return rect;
            return null;
        }

        /// <summary>
        /// 获取小地图位置
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        Vector2 GetMinimapPos(Vector3 pos)
        {
            //var mapInfo = mapMgr.GetMapInfo();
            //var minmap = mainView.objMinimap.transform as RectTransform;
            //var grid_x = pos.x * minmap.sizeDelta.x / mapInfo.mapWidth;
            //var grid_y = pos.z * minmap.sizeDelta.y / mapInfo.mapHeight;

            //return new Vector2(grid_x, grid_y);
            return Vector2.zero;
        }

        /// <summary>
        /// 更新主角位置
        /// </summary>
        public void UpdateMiniMapPosition()
        {
            if (allMiniMaps != null)
            {
                foreach (var de in allMiniMaps)
                {
                    var pos = de.Key.transform.localPosition;
                    //de.Value.anchoredPosition = GetMinimapPos(pos);
                    //if (de.Key.isMainCharacter)
                    //{
                    //    mainView.txtMypos.text = io.c("X:", pos.x.ToInt(), " Y:", pos.z.ToInt());
                    //}
                }
            }
        }
    }
}