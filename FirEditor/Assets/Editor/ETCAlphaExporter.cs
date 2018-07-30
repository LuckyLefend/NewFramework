using System.IO;
using UnityEditor;
using UnityEngine;
public class ETCAlphaExporter : BaseEditor {
    static bool MakeTextureAnAtlas(string path, bool force, bool alphaTransparency)
    {
        if (string.IsNullOrEmpty(path)) return false;
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti == null) return false;

        TextureImporterSettings settings = new TextureImporterSettings();
        ti.ReadTextureSettings(settings);

        if (force ||
            settings.readable ||
            settings.maxTextureSize < 4096 ||
            settings.wrapMode != TextureWrapMode.Clamp ||
            settings.npotScale != TextureImporterNPOTScale.ToNearest)
        {
            settings.readable = false;
            settings.maxTextureSize = 4096;
            settings.wrapMode = TextureWrapMode.Clamp;
            settings.npotScale = TextureImporterNPOTScale.None;

            settings.textureFormat = TextureImporterFormat.ARGB32;
            settings.filterMode = FilterMode.Trilinear;

            settings.aniso = 4;
            settings.alphaIsTransparency = alphaTransparency;
            ti.SetTextureSettings(settings);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        }
        return true;
    }

    static public Texture2D ImportTexture(string path, bool forInput, bool force, bool alphaTransparency)
    {
        if (!string.IsNullOrEmpty(path))
        {
            if (forInput) { if (!MakeTextureReadable(path, force)) return null; }
            else if (!MakeTextureAnAtlas(path, force, alphaTransparency)) return null;
            //return AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;

            Texture2D tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            return tex;
        }
        return null;
    }

    public static void CreateMaterial(string materialPath) {
        string pngPath = materialPath.Replace(dataPath, "Assets");
        Texture2D pngSprite = AssetDatabase.LoadAssetAtPath(pngPath, typeof(Texture2D)) as Texture2D;

        string apngPath = pngPath.Replace(".png", "_A.png");
        Texture2D maskSprite = AssetDatabase.LoadAssetAtPath(apngPath, typeof(Texture2D)) as Texture2D;

        materialPath = materialPath.Replace(".png", ".mat");

        string path = Application.dataPath + "/Templates/Materials/SpriteMaterial.mat";
        string name = Path.GetFileNameWithoutExtension(materialPath);
        File.Copy(path, materialPath, true);
        AssetDatabase.Refresh();

        string matPath = materialPath.Replace(dataPath, "Assets");
        Material material = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;

        material.SetTexture("_MainTex", pngSprite);
        material.SetTexture("_AlphaTex", maskSprite);

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 压缩纹理
    /// </summary>
    public static void CompressTexture(string path) {
        string aPath = path.Replace(".png", "_A.png");
        string[] paths = { path, aPath };
        foreach (var newPath in paths) {
            TextureImporter ti = AssetImporter.GetAtPath(newPath) as TextureImporter;
            if (ti == null) return;
            bool isMask = newPath.Contains("_A");

            ti.textureType = TextureImporterType.Default;

            TextureImporterSettings settings = new TextureImporterSettings();
            ti.ReadTextureSettings(settings);

            settings.readable = false;
            settings.mipmapEnabled = false;
            settings.maxTextureSize = 1024;
            settings.wrapMode = TextureWrapMode.Clamp;
            settings.npotScale = TextureImporterNPOTScale.None;

            settings.textureFormat = TextureImporterFormat.ETC_RGB4;
            settings.filterMode = FilterMode.Trilinear;
            settings.spriteMode = (int)(isMask ? SpriteImportMode.None : SpriteImportMode.Multiple);

            settings.aniso = 4;
            settings.alphaIsTransparency = false;
            ti.SetTextureSettings(settings);
            AssetDatabase.ImportAsset(newPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        }
    }
}