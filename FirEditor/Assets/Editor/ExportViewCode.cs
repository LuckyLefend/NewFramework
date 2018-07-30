using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ExportViewCode {
    static string source = @"//Generate By @ExportViewCode
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class [NAME] : UIPanel 
{
[VARS]

	// Use this for initialization
    void Awake() 
    {
        base.Awake();
[BODY]
	}
	
	void OnDestroy () 
    {
        base.OnDestroy();
[DESTROY]
	}
}
";

    public static string AppDataPath {
        get {
            return Application.dataPath;
        }
    }

    [MenuItem("Game/Export View Code")]
    static void StartExprotView() {
        string prefabPath = AppDataPath + "/res/Resources/Prefabs/UI/";
        string[] files = Directory.GetFiles(prefabPath, "*Panel.prefab", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++) {
            string file = files[i].Replace(AppDataPath, "Assets").Replace('\\', '/');

            Debug.Log("Search file:>" + file);
            UnityEngine.Object game =  AssetDatabase.LoadAssetAtPath(file, typeof(GameObject));
            if (game != null) {
                Transform trans = (game as GameObject).transform;
                List<string> paths = new List<string>();
                RecursiveSearch(trans, ref paths);

                var uiName = Path.GetFileNameWithoutExtension(file);
                GenerateViewCode(uiName, paths);
            }
        }
        AssetDatabase.Refresh();
    }

    static void RecursiveSearch(Transform trans, ref List<string> paths) {
        if (trans.name.StartsWith("#")) {
            Transform newTrans = trans;
            string path = "/" + newTrans.name;
            while (newTrans.parent != null) {
                newTrans = newTrans.parent;
                path = "/" + newTrans.name + path;
            }
            paths.Add(path);
        } 
        for (int i = 0; i < trans.childCount; i++) {
            RecursiveSearch(trans.GetChild(i), ref paths);
        }
    }

    /// <summary>
    /// 产生代码
    /// </summary>
    /// <param name="paths"></param>
    static void GenerateViewCode(string uiName, List<string> paths) {
        string vars = string.Empty;
        string body = string.Empty;
        string destroy = string.Empty;
        string code = source.Replace("[NAME]", uiName);

        for (int i = 0; i < paths.Count; i++) {
            string endChar = string.Empty;
            if (i < paths.Count - 1) {
                endChar = "\n";
            }
            string newPath = paths[i].Replace("/" + uiName + "/", "");
            string name = Path.GetFileName(newPath);
            string[] nameStrs = name.Split('_');

            string ctrlName = nameStrs[0].Remove(0, 1) + nameStrs[1].Substring(0, 1).ToUpper() + nameStrs[1].Substring(1);
            body += "        " + ctrlName + " = transform.FindChild(\"" + newPath + "\")";

            string componentStr = string.Empty;
            switch (nameStrs[0]) {
                case "#img":
                    vars += "    public Image " + ctrlName + ";";
                    componentStr = ".GetComponent<Image>();"; 
                break;
                case "#txt":
                    vars += "    public Text " + ctrlName + ";";
                    componentStr = ".GetComponent<Text>();"; 
                break;
                case "#obj": case "#prefab":
                    vars += "    public GameObject " + ctrlName + ";";
                    componentStr = ".gameObject;"; 
                break;
                case "#btn":
                    vars += "    public Button " + ctrlName + ";";
                    componentStr = ".GetComponent<Button>();"; 
                break;
                case "#slider":
                    vars += "    public Slider " + ctrlName + ";";
                    componentStr = ".GetComponent<Slider>();"; 
                break;
            }
            vars += endChar;
            body += componentStr + endChar;
            destroy += "        " + ctrlName + " = null;" + endChar; ;
        }
        string path = AppDataPath + "/Scripts/View/" + uiName + ".cs";
        code = code.Replace("[VARS]", vars).Replace("[BODY]", body).Replace("[DESTROY]", destroy);
        File.WriteAllText(path, code);
    }
}
