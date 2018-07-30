using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface IMapScene {
    void CreateMap(string name, Action<GameObject> creatOK);
    void CreateMapEvent();
    MapInfo GetMapInfo();
    bool TryMoveIt(int row, int col, out Vector2 outPos);

    void OnEnterMapEvent();
    void OnLevelMapEvent();
    void OnTileDataEvent(List<MapDataEvent> events);
}
