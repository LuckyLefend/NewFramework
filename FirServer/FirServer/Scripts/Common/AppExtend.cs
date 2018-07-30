using System;
using System.Collections.Generic;
using LiteNetLib.Utils;
using UnityEngine;

/// <summary>
/// 扩展方法区
/// </summary>
public static class AppExtend
{
    public static int ToInt(this object o)
    {
        return Convert.ToInt32(o);
    }

    public static float ToFloat(this object o)
    {
        //return (float)Math.Round(Convert.ToSingle(o), 2);
        return Convert.ToSingle(o);
    }

    public static long ToLong(this object o)
    {
        return Convert.ToInt64(o);
    }

    /// <summary>
    /// 搜索子物体组件-GameObject版
    /// </summary>
    public static T ChildGet<T>(this GameObject go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform sub = go.transform.Find(subnode);
            if (sub != null)
                return sub.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 搜索子物体组件-Transform版
    /// </summary>
    public static T ChildGet<T>(this Transform go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform sub = go.Find(subnode);
            if (sub != null)
                return sub.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    public static T Add<T>(this GameObject go) where T : Component
    {
        if (go != null)
        {
            T[] ts = go.GetComponents<T>();
            for (int i = 0; i < ts.Length; i++)
            {
                if (ts[i] != null)
                    GameObject.Destroy(ts[i]);
            }
            return go.gameObject.AddComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    public static GameObject GetChild(this Transform go, string subnode)
    {
        Transform tran = go.Find(subnode);
        if (tran == null)
            return null;
        return tran.gameObject;
    }

    /// <summary>
    /// 取平级对象
    /// </summary>
    public static GameObject GetPeer(this Transform go, string subnode)
    {
        Transform tran = go.parent.Find(subnode);
        if (tran == null)
            return null;
        return tran.gameObject;
    }

    /// <summary>
    /// 清除所有子节点
    /// </summary>
    public static void ClearChild(this Transform go)
    {
        if (go == null)
            return;
        for (int i = go.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(go.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 设置层
    /// </summary>
    /// <param name="root"></param>
    /// <param name="layerName"></param>
    public static void SetLayer(this Transform root, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (root != null)
        {
            Stack<Transform> children = new Stack<Transform>();
            children.Push(root);
            while (children.Count > 0)
            {
                Transform currentTransform = children.Pop();
                currentTransform.gameObject.layer = layer;
                foreach (Transform child in currentTransform)
                {
                    children.Push(child);
                }
            }
        }
    }

    /// <summary>
    /// NetDataWriter
    /// </summary>
    public static void Put(this NetDataWriter dw, Vector2 v)
    {
        dw.Put(v.x);
        dw.Put(v.y);
    }

    public static void Put(this NetDataWriter dw, Vector3 v)
    {
        dw.Put(v.x);
        dw.Put(v.y);
        dw.Put(v.z);
    }

    public static void Put(this NetDataWriter dw, Quaternion v)
    {
        dw.Put(v.x);
        dw.Put(v.y);
        dw.Put(v.z);
        dw.Put(v.w);
    }

    public static Vector2 GetVector2(this NetDataReader reader)
    {
        var x = reader.GetFloat();
        var y = reader.GetFloat();
        return new Vector2(x, y);
    }

    public static Vector3 GetVector3(this NetDataReader reader)
    {
        var x = reader.GetFloat();
        var y = reader.GetFloat();
        var z = reader.GetFloat();
        return new Vector3(x, y, z);
    }

    public static Quaternion GetQuaternion(this NetDataReader reader)
    {
        var x = reader.GetFloat();
        var y = reader.GetFloat();
        var z = reader.GetFloat();
        var w = reader.GetFloat();
        return new Quaternion(x, y, z, w);
    }

    public static bool Between(this sbyte target, sbyte min, sbyte max)
    {
        return target >= min && target <= max;
    }

    public static bool Between(this byte target, byte min, byte max)
    {
        return target >= min && target <= max;
    }

    public static bool Between(this short target, short min, short max)
    {
        return target >= min && target <= max;
    }

    public static bool Between(this ushort target, ushort min, ushort max)
    {
        return target >= min && target <= max;
    }

    public static bool Between(this int target, int min, int max)
    {
        return target >= min && target <= max;
    }

    public static bool Between(this uint target, uint min, uint max)
    {
        return target >= min && target <= max;
    }

    public static bool Between(this long target, long min, long max)
    {
        return target >= min && target <= max;
    }

    public static bool Between(this ulong target, ulong min, ulong max)
    {
        return target >= min && target <= max;
    }

    public static bool Between(this float target, float min, float max)
    {
        return target >= min && target <= max;
    }

    public static bool Between(this double target, double min, double max)
    {
        return target >= min && target <= max;
    }

    public static bool Near(this float target, float other, float distance)
    {
        return target.Between(other - distance, other + distance);
    }

    public static bool Near(this object target, object other, float distance)
    {
        return target == other;
    }
}