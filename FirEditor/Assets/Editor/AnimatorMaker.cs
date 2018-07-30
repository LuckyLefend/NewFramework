using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using Anthill.Animation;

public enum FrameAnimType
{
    UI,
    Sprite,
}

public class AnimatorMaker : BaseEditor
{
    [MenuItem("Assets/Generate Sprite Animation")]
    static void CreateSpriteAnimation()
    {
        var path = GetSelectObjectPath();
        if (!path.EndsWith(".png"))
        {
            return;
        }
        List<Sprite> list = new List<Sprite>();
        Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].GetType() == typeof(Sprite))
            {
                list.Add(objs[i] as Sprite);
            }
        }
        string name = Path.GetFileNameWithoutExtension(path);
        string dir = Path.GetDirectoryName(path);
        dir = Path.GetFileNameWithoutExtension(dir);

        string outPath = "Assets/res/Resources/Prefabs/" + dir;
        BuildPrefab(FrameAnimType.Sprite, name, outPath, list.ToArray());
        AssetDatabase.Refresh();
    }

    static void BuildPrefab(FrameAnimType type, string name, string path, Sprite[] sprites)
    {
        switch (type)
        {
            case FrameAnimType.UI:
                CreateUIAnimationPrefab(name, path, sprites);
            break;
            case FrameAnimType.Sprite:
                CreateSpriteAnimationPrefab(name, path, sprites);
            break;
        }
    }

    static void CreateSpriteAnimationPrefab(string name, string path, Sprite[] sprites)
    {
        var go = new GameObject(name);
        var actor = go.AddComponent<AntActor>();
        actor.initialAnimation = "Move";
        actor.animations = new AntActor.AntAnimation[1];
        actor.animations[0].name = "Move";
        actor.animations[0].frames = sprites;

        var materialPath = GetSelectObjectPath();
        materialPath = materialPath.Replace(".png", ".mat");
        var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material == null)
        {
            material = Resources.Load<Material>("Atlas/SpriteMaterial");
        }

        var spriteRender = go.GetComponent<SpriteRenderer>();
        spriteRender.sprite = sprites[0];
        spriteRender.material = material;

        string prefab = path + "/" + name + ".prefab";
        if (File.Exists(prefab))
        {
            File.Delete(prefab);
        }
        PrefabUtility.CreatePrefab(prefab, go);
        GameObject.DestroyImmediate(go);
    }

    static void CreateUIAnimationPrefab(string name, string path, Sprite[] sprites)
    {
        var go = new GameObject(name);
        go.AddComponent<UIFrameAnimation>().sprites = sprites;

        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);

        var node = new GameObject("Animation");
        node.transform.SetParent(go.transform);
        node.AddComponent<RectTransform>();
        node.transform.localEulerAngles = new Vector3(0, 0, 270);

        Image image = node.AddComponent<Image>();
        image.sprite = sprites[0];
        image.SetNativeSize();

        string prefab = path + "/" + name + ".prefab";
        if (File.Exists(prefab))
        {
            File.Delete(prefab);
        }
        PrefabUtility.CreatePrefab(prefab, go);

        GameObject.DestroyImmediate(node);
        GameObject.DestroyImmediate(go);
    }
}
