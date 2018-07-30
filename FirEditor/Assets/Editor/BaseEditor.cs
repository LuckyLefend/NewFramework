using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class BaseEditor : Editor {
    public static string dataPath { get { return Application.dataPath; } }

    /// <summary>
    /// 获取选择目录
    /// </summary>
    /// <returns></returns>
    public static string GetSelectedPathOrFallback() {
        if (Selection.activeInstanceID != 0) {
            Selection.activeInstanceID = 0;
            return null;
        }
        string path = "Assets";

        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

    public static string GetSelectObjectPath() {
        var obj = Selection.activeObject;
        return AssetDatabase.GetAssetPath(obj);
    }

    /// <summary>
    /// 执行命令行
    /// </summary>
    /// <param name="exe"></param>
    /// <param name="args"></param>
    /// <param name="workDir"></param>
    public static void ExecCommand(string exe, string args, string workDir) {
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = exe;
        info.Arguments = args;
        info.WindowStyle = ProcessWindowStyle.Hidden;
        info.UseShellExecute = true;
        info.WorkingDirectory = workDir;
        info.ErrorDialog = true;
        UnityEngine.Debug.Log(info.FileName + " " + info.Arguments);

        Process pro = Process.Start(info);
        pro.WaitForExit();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="force"></param>
    /// <returns></returns>
    public static bool MakeTextureReadable(string path, bool force) {
        if (string.IsNullOrEmpty(path)) return false;
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti == null || ti.isReadable) return false;

        TextureImporterSettings settings = new TextureImporterSettings();
        ti.textureType = TextureImporterType.Default;
        ti.mipmapEnabled = false;
        ti.ReadTextureSettings(settings);

        if (force || !settings.readable || settings.npotScale != TextureImporterNPOTScale.None || settings.alphaIsTransparency) {
            settings.mipmapEnabled = false;
            settings.readable = true;
            settings.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            settings.npotScale = TextureImporterNPOTScale.None;
            settings.alphaIsTransparency = false;
            ti.SetTextureSettings(settings);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        }
        return true;
    }

    public static bool IsNumber(string strNumber) {
        Regex regex = new Regex("[^0-9]");
        return !regex.IsMatch(strNumber);
    }
}
