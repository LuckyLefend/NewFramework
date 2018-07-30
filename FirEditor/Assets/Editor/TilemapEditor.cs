using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tilemap))]
public class TilemapEditor : BaseEditor
{
    const string TERRAIN_LAYER = "Terrain";

    private class Styles
    {
        static Styles s_instance;
        public static Styles Instance { get { if (s_instance == null) s_instance = new Styles(); return s_instance; } }
        public GUIStyle scrollStyle = new GUIStyle("ScrollView");
        public GUIStyle customBox = new GUIStyle("Box");
    }
    private Tilemap tileMap;
    private Vector2 scrollPos;
    private string currTile;
    private string tipMessage;
    private MessageType tipType;
    private List<Sprite> tiles = new List<Sprite>();

    void OnEnable()
    {
        tipMessage = null;
        tileMap = (Tilemap)target;
        this.LoadMapTile();  
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (tileMap == null || tileMap.tileAtlasTexture == null)
        {
            tiles.Clear();
            currTile = null;
        }
        else
        {
            if (tiles.Count == 0)
            {
                this.LoadMapTile();
            }
        }
        this.DrawLine();
        this.DrawPalette();
        this.DrawBottomUI();
        if (!string.IsNullOrEmpty(tipMessage))
        {
            EditorGUILayout.HelpBox(tipMessage, tipType);
        }
    }

    /// <summary>
    /// 载入地图TILE
    /// </summary>
    /// <param name="path"></param>
    void LoadMapTile()
    {
        if (tileMap.tileAtlasTexture == null) 
        {
            return;
        }
        var path = AssetDatabase.GetAssetPath(tileMap.tileAtlasTexture);
        tiles.Clear();
        if (path.Contains("res/Atlas"))
        {
            MakeTextureReadable(path, true);
            Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
            for (int i = 0; i < objs.Length; i++)
            {
                var sprite = objs[i] as Sprite;
                if (sprite != null)
                {
                    tiles.Add(sprite);
                }
            }
        }
    }

    void DrawBottomUI()
    {
        GUI.color = Color.red;
        if (GUILayout.Button("Clear Map All Node!!!"))
        {
            if (EditorUtility.DisplayDialog("Warning", "Are you sure clear current map?", "Yes", "No"))
            {
                var parent = tileMap.transform;
                for (int i = parent.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(parent.GetChild(i).gameObject);
                }
            }
        }
        GUI.color = Color.white;
        
        ///添加碰撞器
        GUI.color = Color.green;
        if (GUILayout.Button("Add All Node PolygonCollider2D!!!"))
        {
            var parent = tileMap.transform;
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                AddPolygonCollider2D(parent.GetChild(i).gameObject);
            }
        }
        GUI.color = Color.white;

        GUI.color = Color.green;
        if (GUILayout.Button("Clear All Node PolygonCollider2D!!!"))
        {
            var parent = tileMap.transform;
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                ClearPolygonCollider2D(parent.GetChild(i).gameObject);
            }
        }
        GUI.color = Color.white;

        if (GUILayout.Button("Update GameObject Layers!!!"))
        {
            var parent = tileMap.transform;
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                parent.GetChild(i).gameObject.layer = tileMap.gameObject.layer;
            }
        }
    }

    void DrawLine()
    {
        //var style = new GUIStyle(GUI.skin.box);
        //style.stretchWidth = true;
        //style.fixedHeight = 1;
        //GUILayout.Box("", style);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    void DrawPalette()
    {

        int elementsInThisRow = 0;
        Color activeColor = new Color(1f, 1f, 1f, 1f);
        Color disableColor = new Color(1f, 1f, 1f, 0.4f);
        EditorGUILayout.LabelField("Tile Palette: " + currTile, EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, Styles.Instance.scrollStyle);
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < tiles.Count; i++)
            {
                var texture = CreateTextureFromSprite(tiles[i]);
                GUI.color = currTile == texture.name ? activeColor : disableColor;
                if (GUILayout.Button(texture, GUILayout.MaxWidth(50), GUILayout.MaxHeight(50)))
                {
                    currTile = texture.name;
                    Tools.current = Tool.None;
                    EditorWindow.FocusWindowIfItsOpen<SceneView>();
                    this.OnSelected();
                }
                GUI.color = Color.white;

                elementsInThisRow++;
                if (elementsInThisRow > Screen.width / 70)
                {
                    elementsInThisRow = 0;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 创建TILE
    /// </summary>
    /// <param name="sprite"></param>
    /// <returns></returns>
    Texture CreateTextureFromSprite(Sprite sprite)
    {
        var x = (int)sprite.textureRect.x;
        var y = (int)sprite.textureRect.y;
        var width = (int)sprite.textureRect.width;
        var height = (int)sprite.textureRect.height;

        Color[] pixels = sprite.texture.GetPixels(x, y, width, height);
        Texture2D newTex = new Texture2D(width, height);
        newTex.name = sprite.name;
        newTex.SetPixels(pixels);
        newTex.mipMapBias = -10;
        newTex.Apply(false);
        return newTex as Texture2D;
    }

    void OnSelected()
    {
        if (currTile != null)
        {
            MessageBox("Current tile: " + currTile, MessageType.Info);
        }
    }

    void MessageBox(string message, MessageType type)
    {
        tipType = type;
        tipMessage = message;
    }

    void OnSceneGUI()
    {
        bool pressToolBar = IsDoToolbar();
        if (pressToolBar)
        {
            currTile = null;
        }
        if (pressToolBar
            || DragAndDrop.objectReferences.Length > 0 // hide brush when user is dragging a prefab into the scene
            || EditorWindow.mouseOverWindow != SceneView.currentDrawingSceneView // hide brush when it's not over the scene view
            || (Tools.current != Tool.None))
        {
            SceneView.RepaintAll();
            return;
        }
        int id = GUIUtility.GetControlID(FocusType.Passive);
        Event e = Event.current;
        if (e.isMouse)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                this.OnDown(e);
                GUIUtility.hotControl = id;
                e.Use();
                break;
                case EventType.MouseUp:
                this.OnUp(e);
                GUIUtility.hotControl = 0;
                break;
                case EventType.MouseDrag:
                if (GUIUtility.hotControl == id) { }
                break;
            }
        }
    }

    void OnDown(Event e)
    {
        var targetObj = target as Tilemap;
        if (targetObj == null)
        {
            MessageBox("Select tilemap layer first!", MessageType.Error);
        }
        else
        {
            if (targetObj.name == TERRAIN_LAYER)
            {
                MessageBox("Terrain Layer cannot paint tile!", MessageType.Warning);
            }
            else
            {
                Plane chunkPlane = new Plane(targetObj.transform.forward, targetObj.transform.position);
                Vector2 mousePos = e.mousePosition;
                mousePos.y = Screen.height - mousePos.y;
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                float dist;
                if (chunkPlane.Raycast(ray, out dist))
                {
                    var outPos = targetObj.transform.InverseTransformPoint(ray.GetPoint(dist));
                    var spriteRender = this.CreateTile();
                    if (spriteRender == null)
                    {
                        MessageBox("Select tile first!", MessageType.Warning);
                    }
                    else
                    {
                        spriteRender.transform.position = outPos;
                        spriteRender.sortingOrder = tileMap.orderInLayer;
                        spriteRender.sortingLayerName = tileMap.sortingLayerName;

                        float z = tileMap.randomAngle ? UnityEngine.Random.Range(0, 360) : 0;
                        spriteRender.transform.localEulerAngles = new Vector3(0, 0, z);

                        //spriteRender.gameObject.AddComponent<TweenAnimation>();
                        //spriteRender.gameObject.AddComponent<CullingRegister>();

                        //AddPolygonCollider2D(spriteRender.gameObject);
                    }
                }
            }
        }
    }

    void OnUp(Event e)
    {
        //Debug.LogError("up event!!!");
    }

    void AddPolygonCollider2D(GameObject gameObj)
    {
        if (gameObj == null)
        {
            return;
        }
        var collider = gameObj.GetComponent<PolygonCollider2D>();
        if (collider != null)
        {
            return;
        }
        gameObj.AddComponent<PolygonCollider2D>();
        ReduceVertexPolygonCollider2D.RemoveShapes(gameObj);
    }

    void ClearPolygonCollider2D(GameObject gameObj)
    {
        if (gameObj == null)
        {
            return;
        }
        var colliders = gameObj.GetComponentsInChildren<PolygonCollider2D>();
        foreach (var c in colliders)
        {
            DestroyImmediate(c);
        }
    }

    /// <summary>
    /// 创建TILE
    /// </summary>
    /// <returns></returns>
    SpriteRenderer CreateTile()
    {
        if (currTile == null)
        {
            return null;
        }
        var targetObj = target as Tilemap;
        var parent = targetObj.transform;

        var currMaxId = GetLastId(parent);
        currMaxId++;

        var texPath = AssetDatabase.GetAssetPath(tileMap.tileAtlasTexture);
        var index = texPath.LastIndexOf('/');
        var texDir = texPath.Substring(0, index) + "/Objects";

        if (!Directory.Exists(texDir))
        {
            return null;
        }
        var prefabDir = texDir + "/" + currTile + ".prefab";
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabDir);
        var gameObj = Instantiate<GameObject>(asset);
        gameObj.name = currMaxId.ToString();
        gameObj.transform.SetParent(parent);
        gameObj.transform.localScale = Vector3.one;
        gameObj.transform.localPosition = Vector3.zero;
        return gameObj.GetComponent<SpriteRenderer>();
    }

    int GetLastId(Transform parent)
    {
        int max = 0;
        for (int i = 0; i < parent.childCount; i++)
        {
            var node = parent.GetChild(i);
            if (IsNumber(node.name))
            {
                var v = int.Parse(node.name);
                if (v > max)
                    max = v;
            }
        }
        return max;
    }

    /// <summary>
    /// 是不是工具栏
    /// </summary>
    /// <returns></returns>
    bool IsDoToolbar()
    {
        if (Tools.current != Tool.None)
        {
            string message = "Press any toolbar button to start painting...";

            Rect rWarningArea = new Rect(1, 1, 370f, 22f);
            GUILayout.BeginArea(rWarningArea);
            EditorGUI.HelpBox(new Rect(0, 0, rWarningArea.size.x, rWarningArea.size.y), message, MessageType.Warning);
            GUILayout.EndArea();
            return true;
        }
        return false;
    }
}
