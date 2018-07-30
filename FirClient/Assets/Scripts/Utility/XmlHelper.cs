using UnityEngine;
using System.Collections;
using Mono.Xml;
using System.Security;
using FirClient.Manager;

public class XmlHelper : BaseBehaviour
{
    public static SecurityElement LoadXml(string xmlPath)
    {
        SecurityParser sp = new SecurityParser();
        var data = resMgr.LoadAsset<TextAsset>(xmlPath);
        sp.LoadXml(data.ToString());
        return sp.ToXml();
    }
}
