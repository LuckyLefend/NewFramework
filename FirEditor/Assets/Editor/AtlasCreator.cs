using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;

public enum AtlasType
{
    UI,
    Sprite
}

public class AtlasCreator : BaseEditor
{
    static string atlasName = string.Empty;
    public static string TPBinPath = "D:/TexturePacker/bin/TexturePacker.exe";
    const string TPS_FILE_PATH = "[TPS_FILE_PATH]";     //绝对路径
    const string DATA_FILE_PATH = "[DATA_FILE_PATH]";   //../../Resources/Atlas/Main/Main.tpsheet
    const string TEXTURE_DIR_PATH = "[TEXTURE_DIR_PATH]"; //../../Textures/Main

    [MenuItem("Assets/Generate External Atlas")]
    static void CreateExternalAtlas()
    {
        CreateAtlasInternal(false);
    }

    [MenuItem("Assets/Generate Resources Atlas")]
    static void CreateResourcesAtlas()
    {
        CreateAtlasInternal(true);
    }

    [MenuItem("Assets/Generate Prefab from Atlas")]
    static void CreateAtlasPrefabs()
    {
        var obj = Selection.activeObject;
        var texPath = AssetDatabase.GetAssetPath(obj);

        if (texPath.Contains("/Atlas/") && texPath.EndsWith(".png"))
        {
            var path = Path.GetDirectoryName(texPath) + "/Objects";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                AssetDatabase.Refresh();
            }
            Directory.CreateDirectory(path);

            var sprites = AssetDatabase.LoadAllAssetsAtPath(texPath);
            foreach (var s in sprites)
            {
                if (s.GetType() == typeof(Sprite))
                {
                    var gameObject = CreateTileObject(s.name, texPath, s as Sprite);
                    var prefab = PrefabUtility.CreateEmptyPrefab(path + "/" + s.name + ".prefab");
                    PrefabUtility.ReplacePrefab(gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
                    DestroyImmediate(gameObject);
                }
            }
        }
        AssetDatabase.Refresh();
    }

    static GameObject CreateTileObject(string idName, string atlasPath, Sprite sprite)
    {
        var gameObj = new GameObject(idName);
        gameObj.transform.localScale = Vector3.one;
        gameObj.transform.localPosition = Vector3.zero;

        var materialPath = atlasPath.Replace(".png", ".mat");
        var mobject = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));
        var spriteRender = gameObj.AddComponent<SpriteRenderer>();
        spriteRender.sprite = sprite;
        spriteRender.material = mobject as Material;
        return spriteRender.gameObject;
    }

    /// <summary>
    /// 创建图集
    /// </summary>
    /// <param name="inResource"></param>
    static void CreateAtlasInternal(bool inResource)
    {
        if (string.IsNullOrEmpty(TPBinPath) || !File.Exists(TPBinPath))
        {
            Debug.LogError("没有找到TexturePacker安装路径，请配置后再执行此命令！");
            return;
        }
        string dir = GetSelectedPathOrFallback();
        if (dir != null && dir.Contains("/Textures/") && Directory.Exists(dir))
        {
            var dirName = dir.Replace("Assets/Textures/", string.Empty);
            var type = dirName.Contains("UI") ? AtlasType.UI : AtlasType.Sprite;
            CreateAtlas(dirName, Path.GetFileName(dir), inResource, type);
        }
    }

    /// <summary>
    /// 创建一个TP图集
    /// TexturePacker /path/to/your/spritesheet.tps --force-publish
    /// </summary>
    /// <param name="name"></param>
    static void CreateAtlas(string dir, string name, bool inResource, AtlasType type)
    {
        var path = inResource ? "res/Resources/Atlas" : "res/Atlas";
        string texture_dir = dataPath + "/Textures/" + dir;
        string tps_file_dir = dataPath + "/" + path + "/" + dir + "/" + name + ".tps";
        string sheet_data_dir = dataPath + "/" + path + "/" + dir + "/" + name + ".tpsheet";

        string dirName = Path.GetDirectoryName(tps_file_dir);
        if (Directory.Exists(dirName) == false)
        {
            Directory.CreateDirectory(dirName);
        }
        if (File.Exists(tps_file_dir))
        {
            ExecCommand(TPBinPath, tps_file_dir + " --force-publish", dirName);
        }
        else
        {
            string templatePath = dataPath + "/Templates/Atlas/AtlasTemplate.txt";
            string content = File.ReadAllText(templatePath);
            content = content.Replace(TEXTURE_DIR_PATH, texture_dir)
                             .Replace(DATA_FILE_PATH, sheet_data_dir)
                             .Replace(TPS_FILE_PATH, tps_file_dir);
            File.WriteAllText(tps_file_dir, content);
            AssetDatabase.Refresh();

            ExecCommand(TPBinPath, tps_file_dir + " --force-publish", dirName);
        }
        //CreateAtlasAndAlpha(name, dirName, texture_dir);
        //ETCAlphaExporter.CreateMaterial(tps_file_dir);
        //ETCAlphaExporter.CompressTexture(tps_file_dir);

        CreateMaterial(tps_file_dir, type);
        CompressTexture(tps_file_dir);
        Debug.Log("Atlas " + name + " Create OK!");
    }

    /// <summary>
    /// 创建材质
    /// </summary>
    /// <param name="tpsPath"></param>
    static void CreateMaterial(string tpsPath, AtlasType type)
    {
        string typeName = type.ToString();
        var materialPath = tpsPath.Replace(".tps", ".mat");
        string path = dataPath + "/Templates/Materials/" + typeName + "Material.mat";
        string name = Path.GetFileNameWithoutExtension(materialPath);
        File.Copy(path, materialPath, true);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 压缩纹理
    /// </summary>
    /// <param name="tpsPath"></param>
    static void CompressTexture(string tpsPath)
    {
        string aPath = tpsPath.Replace(dataPath, "Assets").Replace(".tps", ".png");
        TextureImporter ti = AssetImporter.GetAtPath(aPath) as TextureImporter;
        if (ti == null)
        {
            return;
        }
        ti.textureType = TextureImporterType.Default;

        TextureImporterSettings settings = new TextureImporterSettings();
        ti.ReadTextureSettings(settings);

        settings.readable = false;
        settings.mipmapEnabled = false;
        settings.maxTextureSize = 1024;
        settings.wrapMode = TextureWrapMode.Clamp;
        settings.npotScale = TextureImporterNPOTScale.None;

        settings.textureFormat = TextureImporterFormat.ETC2_RGBA8;
        settings.filterMode = FilterMode.Trilinear;
        settings.spriteMode = (int)SpriteImportMode.Multiple;

        settings.aniso = 4;
        settings.alphaIsTransparency = false;
        ti.SetTextureSettings(settings);
        AssetDatabase.ImportAsset(aPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 创建图集跟透明通道
    /// </summary>
    /// <param name="atlasName"></param>
    /// <param name="packedPath"></param>
    /// <param name="texturePath"></param>
    static void CreateAtlasAndAlpha(string atlasName, string packedPath, string texturePath)
    {
        var tpexe = AtlasCreator.TPBinPath;
        string main = "--format unity-texture2d " +
                    "--max-width 1024 " +
                    "--max-height 1024 " +
                    "--multipack " +
                    "--trim-mode None " +
                    "--disable-rotation " +
                    "--size-constraints POT " +
                    "--opt RGB888 " +
                    "--sheet " + packedPath + "/" + atlasName + ".png " +
                    "--data " + packedPath + "/" + atlasName + ".tpsheet " + texturePath;
        ExecCommand(tpexe, main, packedPath);

        string mask = "--format unity-texture2d " +
                    "--max-width 1024 " +
                    "--max-height 1024 " +
                    "--multipack " +
                    "--trim-mode None " +
                    "--disable-rotation " +
                    "--size-constraints POT " +
                    "--opt ALPHA " +
                    "--sheet " + packedPath + "/" + atlasName + "_A.png " +
                    "--data " + packedPath + "/" + atlasName + "_A.tpsheet " + texturePath;
        ExecCommand(tpexe, mask, packedPath);
        AssetDatabase.Refresh();
    }
}
