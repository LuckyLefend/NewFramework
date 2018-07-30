using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SortingLayerAttribute : PropertyAttribute { }

[ExecuteInEditMode]
public class Tilemap : GameBehaviour
{
    [SerializeField]
    private bool m_isVisible = true;

    [SerializeField, SortingLayer]
    private int m_sortingLayer = 0;

    [HideInInspector]
    public string sortingLayerName = "Default";

    [SerializeField]
    private int m_orderInLayer = 0;

    public bool randomAngle = false;
    public Sprite tileAtlasTexture;

    public int sortingLayer
    {
        get { return m_sortingLayer; }
        set { m_sortingLayer = value; }
    }

    public int orderInLayer
    {
        get { return m_orderInLayer; }
        set { m_orderInLayer = value; }
    }

    public bool IsVisible
    {
        get
        {
            return m_isVisible;
        }
        set
        {
            bool prevValue = m_isVisible;
            m_isVisible = value;
            if (prevValue != m_isVisible)
            {
                RefreshTilemap(value);
            }
        }
    }

    void RefreshTilemap(bool isVisible)
    {
        var renders = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var r in renders)
        {
            r.enabled = isVisible;
        }
    }

    public void RefreshChunksSortingAttributes()
    {
        var renders = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var r in renders)
        {
            r.sortingOrder = orderInLayer;
            r.sortingLayerName = sortingLayerName;
            r.gameObject.layer = LayerMask.NameToLayer(sortingLayerName);
        }
    }
}
