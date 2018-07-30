using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// 位图字体导入工具类 - by lijun2
/// </summary>

class BitmapFontInfo {
    public Dictionary<string, string> info;
    public Dictionary<string, string> common;
    public Dictionary<string, string> page;
    public Dictionary<string, string> chars;
    public List<Dictionary<string, string>> chardata;
}

public static class BitmapFontImporter
{
    static BitmapFontInfo currFont;

	[MenuItem("Assets/Generate Bitmap Font")]
	public static void GenerateFont ()
	{
		TextAsset selected = (TextAsset)Selection.activeObject;
        if (selected == null) return;
        string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(selected));
        ParseText(selected);

        var textureFile = currFont.page["file"];
        string fontPath = rootPath + "/" + textureFile;
        Texture2D texture = AssetDatabase.LoadAssetAtPath(fontPath, typeof(Texture2D)) as Texture2D;
        if (!texture)
        {
            throw new UnityException("Texture2d doesn't exist for:> " + fontPath);
        }
		string exportPath = rootPath + "/" + Path.GetFileNameWithoutExtension (selected.name);
        Handle(exportPath, texture);
	}

    static void ParseText(TextAsset import) {
        if (!import)
            throw new UnityException(import.name + "is not a valid font-xml file");

        currFont = new BitmapFontInfo();
        currFont.info = new Dictionary<string, string>();
        currFont.common = new Dictionary<string, string>();
        currFont.page = new Dictionary<string, string>();
        currFont.chars = new Dictionary<string, string>();
        currFont.chardata = new List<Dictionary<string, string>>();

        string[] txt = import.text.Split('\n');
        for (int i = 0; i < txt.Length; i++) {
            string str = txt[i].Trim();
            if (string.IsNullOrEmpty(str)) continue;

            string[] kvStrs = ParseKeyValue(str);
            string key = kvStrs[0];
            switch (key) {
                case "info":
                    ParseLine(kvStrs, ref currFont.info);
                break;
                case "common":
                    ParseLine(kvStrs, ref currFont.common);
                break;
                case "page":
                    ParseLine(kvStrs, ref currFont.page);
                break;
                case "chars":
                    ParseLine(kvStrs, ref currFont.chars);
                break;
                case "char":
                    var info = new Dictionary<string, string>();
                    ParseLine(kvStrs, ref info);
                    currFont.chardata.Add(info);
                break;
            }
        }
    }

    static void Handle(string exportPath, Texture2D texture) {
        CharacterInfo[] charInfos = new CharacterInfo[currFont.chardata.Count];

        float texW = texture.width;
        float texH = texture.height;
        Rect r;
        for (int i = 0; i < currFont.chardata.Count; i++) {
            Dictionary<string, string> charNode = currFont.chardata[i];
            if (charNode != null) {

                CharacterInfo charInfo = new CharacterInfo();
                charInfo.index = int.Parse(charNode["id"]);
                charInfo.width = float.Parse(charNode["xadvance"]);
                charInfo.flipped = false;

                r = new Rect();
                r.x = float.Parse(charNode["x"]) / texW;
                r.y = float.Parse(charNode["y"]) / texH;
                r.width = float.Parse(charNode["width"]) / texW;
                r.height = float.Parse(charNode["height"]) / texH;
                r.y = 1f - r.y - r.height;
                charInfo.uv = r;


                r = new Rect();
                r.x = float.Parse(charNode["xoffset"]);
                r.y = float.Parse(charNode["yoffset"]);
                r.width = float.Parse(charNode["width"]);
                r.height = float.Parse(charNode["height"]);
                r.y = -r.y;
                r.height = -r.height;
                charInfo.vert = r;

                charInfos[i] = charInfo;
            }
        }
        // Create material
        Shader shader = Shader.Find("UI/Default");
        Material material = new Material(shader);
        material.mainTexture = texture;
        AssetDatabase.CreateAsset(material, exportPath + ".mat");

        // Create font
        Font font = new Font();
        font.material = material;
        //font.name = info.Attributes.GetNamedItem("face").InnerText;
        font.name = currFont.info["face"];
        font.characterInfo = charInfos;
        AssetDatabase.CreateAsset(font, exportPath + ".fontsettings");
    }

    /// <summary>
    /// 分析值对
    /// </summary>
    /// <param name="txt"></param>
    /// <returns></returns>
    static string[] ParseKeyValue(string txt) {
        List<string> result = new List<string>();
        if (txt.Contains("\"")) {
            //string[] sArray = Regex.Split(txt, "\" ", RegexOptions.IgnoreCase);
            bool isHave = false;
            int index = 0;
            for (int i = 0; i < txt.Length; i++) {
                if (isHave) {
                    if (txt[i] == ' ') continue;
                    if (txt[i] == '\"') {
                        if (i == txt.Length - 1) {
                            string s = txt.Substring(index, i - index + 1);
                            result.Add(s);
                        }
                        isHave = false;
                        continue;
                    }
                } else {
                    if (txt[i] == '\"') {
                        isHave = true;
                    }
                }
                if (txt[i] == ' ') {
                    string s = txt.Substring(index, i - index);
                    result.Add(s);
                    index = i + 1;
                    continue;
                } 
                if (i == txt.Length - 1) {
                    string s = txt.Substring(index, i - index + 1);
                    result.Add(s);
                }
            } 
        } else {
            return txt.Split(' ');
        }
        return result.ToArray();
    }

    /// <summary>
    /// 分析一行
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="data"></param>
    static void ParseLine(string[] txt, ref Dictionary<string, string> data) {
        for (int i = 1; i < txt.Length; i++) {
            if (string.IsNullOrEmpty(txt[i])) continue;
            string[] kvStrs = txt[i].Split('=');
            string key = kvStrs[0];
            string value = kvStrs[1];
            if (value.Contains("\"")) {
                value = value.Replace("\"", "");
            }
            data.Add(key, value);
        }
    }
	
    /*
	static void Work (TextAsset import, string exportPath, Texture2D texture)
	{
		if (!import)
			throw new UnityException (import.name + "is not a valid font-xml file");
		
		Font font = new Font ();

		XmlDocument xml = new XmlDocument ();
		xml.LoadXml (import.text);
		
		XmlNode info = xml.GetElementsByTagName ("info") [0];
//		XmlNode common = xml.GetElementsByTagName ("common") [0];
		XmlNodeList chars = xml.GetElementsByTagName ("chars") [0].ChildNodes;
		
		CharacterInfo[] charInfos = new CharacterInfo[chars.Count];

		float texW = texture.width;
		float texH = texture.height;
		Rect r;
        for (int i = 0; i < chars.Count; i++) {
            XmlNode charNode = chars[i];
            if (charNode.Attributes != null) {

                CharacterInfo charInfo = new CharacterInfo();
                charInfo.index = (int)ToFloat(charNode, "id");
                charInfo.width = ToFloat(charNode, "xadvance");
                charInfo.flipped = false;

                r = new Rect();
                r.x = ((float)ToFloat(charNode, "x")) / texW;
                r.y = ((float)ToFloat(charNode, "y")) / texH;
                r.width = ((float)ToFloat(charNode, "width")) / texW;
                r.height = ((float)ToFloat(charNode, "height")) / texH;
                r.y = 1f - r.y - r.height;
                charInfo.uv = r;


                r = new Rect();
                r.x = (float)ToFloat(charNode, "xoffset");
                r.y = (float)ToFloat(charNode, "yoffset");
                r.width = (float)ToFloat(charNode, "width");
                r.height = (float)ToFloat(charNode, "height");
                r.y = -r.y;
                r.height = -r.height;
                charInfo.vert = r;

                charInfos[i] = charInfo;
            }
        }
		
		// Create material
		Shader shader = Shader.Find ("UI/Default");
		Material material = new Material (shader);
		material.mainTexture = texture;
		AssetDatabase.CreateAsset (material, exportPath + ".mat");
		
		// Create font
		font.material = material;
		font.name = info.Attributes.GetNamedItem ("face").InnerText;
		font.characterInfo = charInfos;
		AssetDatabase.CreateAsset (font, exportPath + ".fontsettings");
	}
	
	private static float ToFloat (XmlNode node, string name)
	{
		return float.Parse (node.Attributes.GetNamedItem (name).InnerText);
	}

	private static int ToInt (XmlNode node, string name)
	{
		return int.Parse (node.Attributes.GetNamedItem (name).InnerText);
	}
     * */
}