using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum MapEventType
{
    None = 0,               //无操作
    EnterNextMap = 1,       //进入地图
    SpawnMonster = 2,       //出生怪物
}

[Serializable]
public class MapDataEvent
{
    public MapEventType eventType;

    //[TextAreaAttribute]
    public string eventData;
}

public class MapInfo : GameBehaviour
{
    public float mapWidth;
    public float mapHeight;
    public Transform[] heroSpawnPoints;
    public Transform[] monsterSpawnPoints;
    public MapDataEvent[] enterMapEvent;
    public MapDataEvent[] leaveMapEvent;

    [SerializeField]
    [HideInInspector]
    private List<Tilemap> m_tilemaps;

    [SerializeField]
    [HideInInspector]
    private int m_selectedIndex = -1;

    public List<Tilemap> Tilemaps { get { return m_tilemaps; } }

    public Tilemap SelectedTilemap
    {
        get { return m_selectedIndex >= 0 && m_selectedIndex < m_tilemaps.Count ? m_tilemaps[m_selectedIndex] : null; }
        set
        {
            m_selectedIndex = m_tilemaps != null ? m_tilemaps.IndexOf(value) : -1;
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        this.InitMapInfo();
    }

    public void InitMapInfo()
    {
        this.InitSpwanPoints();
    }

    void InitSpwanPoints()
    {
        var heroSpawnParent = transform.Find("SpawnPoints/Hero");
        if (heroSpawnParent != null)
        {
            heroSpawnPoints = new Transform[heroSpawnParent.childCount];
            for (int i = 0; i < heroSpawnParent.childCount; i++)
            {
                heroSpawnPoints[i] = heroSpawnParent.GetChild(i);
            }
        }
        var monsterSpawnParent = transform.Find("SpawnPoints/Monster");
        if (monsterSpawnParent != null)
        {
            monsterSpawnPoints = new Transform[monsterSpawnParent.childCount];
            for (int i = 0; i < monsterSpawnParent.childCount; i++)
            {
                monsterSpawnPoints[i] = monsterSpawnParent.GetChild(i);
            }
        }
    }

    public void Refresh()
    {
        m_tilemaps = new List<Tilemap>(GetComponentsInChildren<Tilemap>(true));
        if (m_tilemaps.Count > 0 && m_selectedIndex < 0)
            m_selectedIndex = 0;
        m_selectedIndex = Mathf.Clamp(m_selectedIndex, -1, m_tilemaps.Count);
    }
}
