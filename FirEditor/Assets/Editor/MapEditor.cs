using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(MapInfo))]
public class MapEditor : BaseEditor
{

    public static string CurrMapName;

    private MapInfo map;
    private ReorderableList m_tilemapReordList;

    [MenuItem("MapEditor/Create New Map")]
    static void CreateNewMap()
    {
        MapEditorWindow.ShowWindow();
    }

    void OnEnable()
    {
        map = (MapInfo)target;
        map.Refresh();
        map.InitMapInfo();

        m_tilemapReordList = this.CreateLayerList();
        m_tilemapReordList.index = serializedObject.FindProperty("m_selectedIndex").intValue;
    }

    void CopyPasteComponentData(Component src, Component dest)
    {
        ComponentUtility.CopyComponent(src);
        ComponentUtility.PasteComponentValues(dest);
    }

    /// <summary>
    /// 创建层管理
    /// </summary>
    /// <returns></returns>
    ReorderableList CreateLayerList()
    {
        var reordList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_tilemaps"), true, true, true, true);
        reordList.displayAdd = reordList.displayRemove = true;
        reordList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Tile Layers", EditorStyles.boldLabel);
            Texture2D btnTexture = reordList.elementHeight == 0f ? EditorGUIUtility.FindTexture("winbtn_win_max_h") : EditorGUIUtility.FindTexture("winbtn_win_min_h");
            if (GUI.Button(new Rect(rect.width - rect.height, rect.y, rect.height, rect.height), btnTexture, EditorStyles.label))
            {
                reordList.elementHeight = reordList.elementHeight == 0f ? 21f : 0f;
                reordList.draggable = reordList.elementHeight > 0f;
            }
        };
        reordList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            if (reordList.elementHeight == 0)
                return;
            var element = reordList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            Tilemap tilemap = element.objectReferenceValue as Tilemap;
            SerializedObject tilemapSerialized = new SerializedObject(tilemap);

            Object objRefValue = tilemapSerialized.FindProperty("m_GameObject").objectReferenceValue;
            SerializedObject tilemapObjSerialized = new SerializedObject(objRefValue);

            Rect rToggle = new Rect(rect.x, rect.y, 16f, EditorGUIUtility.singleLineHeight);
            Rect rName = new Rect(rect.x + 20f, rect.y, rect.width - 130f - 20f, EditorGUIUtility.singleLineHeight);
            Rect rColliders = new Rect(rect.x + rect.width - 125f, rect.y, 125f, EditorGUIUtility.singleLineHeight);
            Rect rSortingLayer = new Rect(rect.x + rect.width - 125f, rect.y, 80f, EditorGUIUtility.singleLineHeight);
            Rect rSortingOrder = new Rect(rect.x + rect.width - 40f, rect.y, 40f, EditorGUIUtility.singleLineHeight);

            tilemap.IsVisible = EditorGUI.Toggle(rToggle, GUIContent.none, tilemap.IsVisible);
            EditorGUI.PropertyField(rName, tilemapObjSerialized.FindProperty("m_Name"), GUIContent.none);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rSortingLayer, tilemapSerialized.FindProperty("m_sortingLayer"), GUIContent.none);
            EditorGUI.PropertyField(rSortingOrder, tilemapSerialized.FindProperty("m_orderInLayer"), GUIContent.none);
            tilemapSerialized.FindProperty("m_orderInLayer").intValue = (tilemapSerialized.FindProperty("m_orderInLayer").intValue << 16) >> 16; // convert from int32 to int16 keeping sign
            if (EditorGUI.EndChangeCheck())
            {
                tilemapSerialized.ApplyModifiedProperties();
                tilemap.RefreshChunksSortingAttributes();
                SceneView.RepaintAll();
            }

            if (GUI.changed)
            {
                tilemapObjSerialized.ApplyModifiedProperties();
            }
        };
        reordList.onReorderCallback = (ReorderableList list) =>
        {
            var targetObj = target as MapInfo;
            int sibilingIdx = 0;
            foreach (Tilemap tilemap in targetObj.Tilemaps)
            {
                tilemap.transform.SetSiblingIndex(sibilingIdx++);
            }
            Repaint();
        };
        reordList.onSelectCallback = (ReorderableList list) =>
        {
            serializedObject.FindProperty("m_selectedIndex").intValue = reordList.index;
            serializedObject.ApplyModifiedProperties();
            GUI.changed = true;
        };
        reordList.onAddCallback = (ReorderableList list) =>
        {
            GameObject obj = new GameObject();
            obj.name = "New Tilemap";
            obj.transform.parent = map.transform;
            obj.transform.localScale = Vector3.one;
            obj.AddComponent<Tilemap>();
        };
        reordList.onRemoveCallback = (ReorderableList list) =>
        {
            var targetObj = target as MapInfo;
            var tilemap = targetObj.SelectedTilemap;
            if (tilemap != null)
            {
                if (tilemap.name == "Terrain")
                {
                }
                else
                {
                    Undo.DestroyObjectImmediate(tilemap.gameObject);
                    //NOTE: Fix argument exception
                    if (m_tilemapReordList.index == targetObj.Tilemaps.Count - 1)
                    {
                        serializedObject.FindProperty("m_selectedIndex").intValue = m_tilemapReordList.index = m_tilemapReordList.index - 1;
                    }
                }
            }
        };
        return reordList;
    }

    /// <summary>
    /// 保存地图
    /// </summary>
    void SaveMap()
    {
        if (EditorUtility.DisplayDialog("Warning", "Are you sure save current map?", "Yes", "No"))
        {
            this.ExportServerMapData();
            var tileEditor = GameObject.Find("TileEditor");
            if (tileEditor != null)
            {
                tileEditor.SetActive(false);
            }
            var prefab = map.gameObject;
            var srcPrefab = PrefabUtility.GetPrefabParent(prefab);
            PrefabUtility.ReplacePrefab(prefab, srcPrefab, ReplacePrefabOptions.ConnectToPrefab);
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }
    }

    /// <summary>
    /// 导出服务器地图数据
    /// </summary>
    void ExportServerMapData()
    {
        var mapEvent = map.name + "_event";
        var tempPath = "Assets/Templates/Maps/";
        string content = string.Empty;

        content += "    <heroSpawnPos>\n";
        for (int i = 0; i < map.heroSpawnPoints.Length; i++)
        {
            var point = map.heroSpawnPoints[i];
            content += "        <item name='" + point.name + "' pos_x='" + point.localPosition.x +
                                "' pos_y='" + point.localPosition.y + "' pos_z='" + point.localPosition.z + "' />\n";
        }
        content += "    </heroSpawnPos>\n";

        content += "    <monsterSpawnPos>\n";
        for (int i = 0; i < map.monsterSpawnPoints.Length; i++)
        {
            var point = map.monsterSpawnPoints[i];
            content += "        <item name='" + point.name + "' pos_x='" + point.localPosition.x +
                                "' pos_y='" + point.localPosition.y + "' pos_z='" + point.localPosition.z + "' />\n";
        }
        content += "    </monsterSpawnPos>\n";

        for(int i = 0; i < map.transform.childCount; i++)
        {
            var child = map.transform.GetChild(i);
            if (child.name == "SpawnPoints")
            {
                continue;
            }
            content += "    <" + child.name + ">\n";
            for (int j = 0; j < child.childCount; j++)
            {
                var point = child.GetChild(j);
                var render = point.GetComponent<SpriteRenderer>();
                content += "        <item name='" + point.name + "' sprite='" + render.sprite.name + "' pos='" + point.localPosition +
                                    "' rotation='" + point.localEulerAngles + "' scale='" + point.localScale + "' />\n";
            }
            content += "    </"+ child.name + ">\n";
        }

        ///save map data
        var assetObj = AssetDatabase.LoadAssetAtPath(tempPath + "MapTemplate.txt", typeof(TextAsset));
        TextAsset template = assetObj as TextAsset;
        string text = template.text.Replace("{#mapname}", map.name);
        text = text.Replace("{#mapEvent}", mapEvent);
        text = text.Replace("{#content}", content);

        string file = dataPath + "/res/Maps/Data/" + map.name + ".xml";
        string dir = Path.GetDirectoryName(file);
        if (File.Exists(dir))
        {
            File.Delete(dir);
        }
        File.WriteAllText(file, text);
        AssetDatabase.Refresh();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        this.DrawTilemapLayer();
        this.DrawBottomUI();
    }

    void DrawTilemapLayer()
    {
        if (map.transform.childCount != map.Tilemaps.Count)
        {
            map.Refresh();
        }
        serializedObject.Update();

        GUI.color = Color.cyan;
        m_tilemapReordList.DoLayoutList();
        GUI.color = Color.white;

        serializedObject.ApplyModifiedProperties();
    }

    void DrawBottomUI()
    {
        GUI.color = Color.red;
        if (GUILayout.Button("Save Current Map!!!"))
        {
            this.SaveMap();
        }
        GUI.color = Color.white;
    }
}
