using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MapEditorWindow : EditorWindow
{
    public static string currMapName = "TestMap";
    public static void ShowWindow()
    {
        var window = GetWindow<MapEditorWindow>();
        window.titleContent = new GUIContent("MapEditorWindow");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("Map Name:"), GUILayout.Width(100));
                currMapName = GUILayout.TextField(currMapName);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Create New Map!"))
            {
                CreateNewMapLayout(currMapName);
            }
            GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
    }

    void CreateNewMapLayout(string mapName)
    {
        if (Camera.main != null)
        {
            DestroyImmediate(Camera.main.gameObject);
        }
        var root = GameObject.Find(mapName);
        if (root != null)
        {
            return;
        }
        root = new GameObject(mapName);
        root.AddComponent<MapInfo>();

        this.CreateAnTilemap(root, "Ground", "Ground");
        this.CreateAnTilemap(root, "Environment", "Environment");
        this.CreateAnTilemap(root, "SpawnPoints", "Default");
        this.CreateAnTilemap(root, "Gameplay", "Gameplay");
    }

    /// <summary>
    /// 创建一个Tilemap
    /// </summary>
    /// <param name="root"></param>
    /// <param name="layerName"></param>
    /// <returns></returns>
    Tilemap CreateAnTilemap(GameObject root, string tilemapName, string layerName)
    {
        var gameObj = new GameObject(tilemapName);
        gameObj.layer = LayerMask.NameToLayer(layerName);
        gameObj.transform.SetParent(root.transform);

        var tilemap = gameObj.AddComponent<Tilemap>();
        tilemap.orderInLayer = LayerMask.NameToLayer(layerName);
        tilemap.sortingLayer = SortingLayer.NameToID(layerName);
        tilemap.sortingLayerName = layerName;
        return tilemap;
    }
}
