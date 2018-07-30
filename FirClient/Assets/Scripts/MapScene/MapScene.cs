using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using FirClient.Manager;
using System;

public class MapScene : BaseManager, IMapScene
{
    private MapInfo mapInfo;

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
    /// <param name="name"></param>
    /// <param name="createOK"></param>
    public void CreateMap(string name, Action<GameObject> createOK)
    {
        mapMgr.StartCoroutine(OnCreateMap(name, createOK));
    }

    IEnumerator OnCreateMap(string name, Action<GameObject> createOK)
    {
        var ao = resMgr.LoadAssetAsync<GameObject>("Prefabs/Maps/" + name);
        yield return ao;

        while (!ao.isDone)
        {
            yield return 0;
        }
        var prefab = ao.asset as GameObject;
        var gameObj = GameObject.Instantiate<GameObject>(prefab);
        yield return new WaitForSeconds(0.1f);

        if (createOK != null)
        {
            createOK(gameObj);
        }
    }

    public void CreateMapEvent()
    {
    }

    /// <summary>
    /// 获取地图信息
    /// </summary>
    /// <returns></returns>
    public MapInfo GetMapInfo()
    {
        return mapInfo;
    }

    public bool TryMoveIt(int row, int col, out Vector2 outPos)
    {
        throw new System.NotImplementedException();
    }

    public void OnEnterMapEvent()
    {
        throw new System.NotImplementedException();
    }

    public void OnLevelMapEvent()
    {
        throw new System.NotImplementedException();
    }

    public void OnTileDataEvent(System.Collections.Generic.List<MapDataEvent> events)
    {
        throw new System.NotImplementedException();
    }

    public override void OnDispose()
    {
        throw new NotImplementedException();
    }
}
