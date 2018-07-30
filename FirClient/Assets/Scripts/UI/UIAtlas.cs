using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using FirClient.Manager;

public class UIAtlas : BaseBehaviour
{
    private string atlasName;
    private string atlasPath;
    private Material atlasMaterial;
    private Dictionary<string, Sprite> dicSprites = new Dictionary<string, Sprite>();

    public string Name
    {
        get { return atlasName; }
        set { atlasName = value; }
    }

    public string Path
    {
        get { return atlasPath; }
        set { atlasPath = value; }
    }

    public Material AtlasMaterial
    {
        get { return atlasMaterial; }
    }

    public Dictionary<string, Sprite> Sprites
    {
        get { return dicSprites; }
    }

    /// <summary>
    /// 初始化图集
    /// </summary>
    /// <param name="path"></param>
    public UIAtlas(string path)
    {
        this.atlasPath = path;
        Sprite[] sprites = resMgr.LoadAssets<Sprite>(path);
        for (int i = 0; i < sprites.Length; i++)
        {
            var sprite = sprites[i];
            if (sprite != null)
            {
                dicSprites.Add(sprite.name, sprite);
            }
        }
        atlasMaterial = resMgr.LoadAsset<Material>(path);
    }

    /// <summary>
    /// 获取纹理
    /// </summary>
    public Sprite GetSprite(string name)
    {
        if (dicSprites.ContainsKey(name))
        {
            return dicSprites[name];
        }
        return null;
    }
}
