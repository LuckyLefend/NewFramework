using UnityEngine;
using System.Collections;
using Mono.Xml;
using System.Security;
using System.IO;

public class XmlHelper {

    public static SecurityElement LoadXml(string xmlPath) {
        SecurityParser sp = new SecurityParser();
        var data = File.ReadAllText(Application.dataPath + xmlPath);
        sp.LoadXml(data.ToString());
        return sp.ToXml();
    }
}
