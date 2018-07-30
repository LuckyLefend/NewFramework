using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(Canvas))]
//[RequireComponent(typeof(GraphicRaycaster))]
public class UIPanel : GameBehaviour
{
    protected static int m_refCount = 100;
    protected static Dictionary<UIPanel, int> depth = new Dictionary<UIPanel, int>();

    protected int depthid = 0;

    // Use this for initialization
    protected void Awake()
    {
        this.Init();
    }

    void Init()
    {
        depthid = m_refCount++;
        depth.Add(this, depthid);
    }

    protected void Start()
    {
        var canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = depthid;
        }
    }

    protected void OnDestroy()
    {
        depth.Remove(this);
    }
}
