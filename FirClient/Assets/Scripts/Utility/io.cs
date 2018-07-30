using UnityEngine;
using System;
using System.Collections;
using System.Text;

/// <summary>
/// Interface Manager Object 
/// </summary>
public class io {
    /// <summary>
    /// 游戏管理器对象
    /// </summary>
    private static GameObject _manager = null;
    public static GameObject manager {
        get {
            if (_manager == null)
                _manager = GameObject.FindWithTag("GameManager");
            return _manager;
        }
    }

    private static Main _main = null;
    public static Main main
    {
        get {
            if (_main == null)
            {
                _main = manager.GetComponent<Main>();
            }
            return _main;
        }
    }

    /// <summary>
    /// GUI摄像机
    /// </summary>
    public static Canvas uiCanvas {
        get {
            GameObject go = GameObject.FindWithTag("UICanvas");
            if (go != null) return go.GetComponent<Canvas>();
            return null;
        }
    }

    private static Transform _battleObject = null;
    public static Transform battleObject
    {
        get
        {
            if (_battleObject == null)
                _battleObject = manager.transform.Find("BattleObject");
            return _battleObject;
        }
    }

    ///---------------------------------------------------------------------------------
    /// <summary>
    /// 格式化字符串
    /// </summary>
    /// <returns></returns>
    public static string f(string format, params object[] args) {
        StringBuilder sb = new StringBuilder();
        return sb.AppendFormat(format, args).ToString();
    }

    /// <summary>
    /// 字符串连接
    /// </summary>
    public static string c(object argv = null, object arg1 = null, object arg2 = null, object arg3 = null, 
                           object arg4 = null, object arg5 = null, object arg6 = null, object arg7 = null) {
        using (gstring.Block()) {
            if (argv == null) {
                Debugger.LogError("argv cannot null!!");
                return null;
            }
            gstring result = argv.ToString();
            if (arg1 != null) {
                gstring argv1 = arg1.ToString();
                result = result + argv1;
            }
            if (arg2 != null) {
                gstring argv2 = arg2.ToString();
                result = result + argv2;
            }
            if (arg3 != null) {
                gstring argv3 = arg3.ToString();
                result = result + argv3;
            }
            if (arg4 != null) {
                gstring argv4 = arg4.ToString();
                result = result + argv4;
            }
            if (arg5 != null) {
                gstring argv5 = arg5.ToString();
                result = result + argv5;
            }
            if (arg6 != null) {
                gstring argv6 = arg6.ToString();
                result = result + argv6;
            }
            if (arg7 != null) {
                gstring argv7 = arg7.ToString();
                result = result + argv7;
            }
            return result.Intern();
        }
    }

    public static T GetCtrl<T>() where T : class
    {
        return BaseBehaviour.GetCtrl<T>();
    }

    public static T GetManager<T>() where T : class
    {
        return BaseBehaviour.GetManager<T>();
    }
}