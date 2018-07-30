using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class CreateScriptEditor : MonoBehaviour
{
    class CreateScriptAssetAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            UnityEngine.Object obj = CreateAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(obj);
        }

        internal static UnityEngine.Object CreateAssetFromTemplate(string pahtName, string resourceFile)
        {
            string fullName = Path.GetFullPath(pahtName);
            StreamReader reader = new StreamReader(resourceFile);
            string content = reader.ReadToEnd();
            reader.Close();

            string fileName = Path.GetFileNameWithoutExtension(pahtName);
            content = content.Replace("[NAME]", fileName);
            content = content.Replace("[TIME]", System.DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss dddd"));

            StreamWriter writer = new StreamWriter(fullName, false, System.Text.Encoding.UTF8);
            writer.Write(content);
            writer.Close();

            AssetDatabase.ImportAsset(pahtName);
            AssetDatabase.Refresh();

            return AssetDatabase.LoadAssetAtPath(pahtName, typeof(UnityEngine.Object));
        }
    }

    [MenuItem("Assets/Create/Message Handler", false, 80)]
    public static void CreateNewLua()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
            ScriptableObject.CreateInstance<CreateScriptAssetAction>(),
            GetSelectedPathOrFallback() + "/Ret Handler.cs",
            null, "Assets/Templates/MessageHandler.txt");
    }

    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }
}
