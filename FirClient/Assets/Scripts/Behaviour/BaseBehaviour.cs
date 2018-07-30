using System;
using System.Collections;
using System.Collections.Generic;
using FirClient.Ctrl;
using FirClient.Manager;
using UnityEngine;

public abstract class BaseBehaviour
{
    protected static Dictionary<string, BaseCtrl> Ctrls = new Dictionary<string, BaseCtrl>();
    protected static Dictionary<string, BaseManager> Managers = new Dictionary<string, BaseManager>();

    private static ObjectManager _objMgr;
    protected static ObjectManager objMgr
    {
        get
        {
            if (_objMgr == null)
            {
                _objMgr = GetManager<ObjectManager>();
            }
            return _objMgr;
        }
    }

    private static SoundManager _soundMgr;
    protected static SoundManager soundMgr
    {
        get
        {
            if (_soundMgr == null)
            {
                _soundMgr = GetManager<SoundManager>();
            }
            return _soundMgr;
        }
    }

    private static UIManager _uiMgr;
    protected static UIManager uiMgr
    {
        get
        {
            if (_uiMgr == null)
            {
                _uiMgr = GetManager<UIManager>();
            }
            return _uiMgr;
        }
    }

    private static NPCManager _npcMgr;
    protected static NPCManager npcMgr
    {
        get
        {
            if (_npcMgr == null)
            {
                _npcMgr = GetManager<NPCManager>();
            }
            return _npcMgr;
        }
    }

    private static HandlerManager _handlerMgr;
    protected static HandlerManager handlerMgr
    {
        get
        {
            if (_handlerMgr == null)
            {
                _handlerMgr = GetManager<HandlerManager>();
            }
            return _handlerMgr;
        }
    }

    private static ConfigManager _cfgMgr;
    protected static ConfigManager cfgMgr
    {
        get
        {
            if (_cfgMgr == null)
            {
                _cfgMgr = GetManager<ConfigManager>();
            }
            return _cfgMgr;
        }
    }

    private static AtlasManager _atlasMgr;
    protected static AtlasManager atlasMgr
    {
        get
        {
            if (_atlasMgr == null)
            {
                _atlasMgr = GetManager<AtlasManager>();
            }
            return _atlasMgr;
        }
    }

    private static BulletManager _bulletMgr;
    protected static BulletManager bulletMgr
    {
        get
        {
            if (_bulletMgr == null)
            {
                _bulletMgr = GetManager<BulletManager>();
            }
            return _bulletMgr;
        }
    }

    private static EffectManager _effectMgr;
    protected static EffectManager effectMgr
    {
        get
        {
            if (_effectMgr == null)
            {
                _effectMgr = GetManager<EffectManager>();
            }
            return _effectMgr;
        }
    }

    private static TimerManager _timerMgr;
    protected static TimerManager timerMgr
    {
        get
        {
            if (_timerMgr == null)
            {
                _timerMgr = GetManager<TimerManager>();
            }
            return _timerMgr;
        }
    }

    private static ResourceManager _resMgr;
    protected static ResourceManager resMgr
    {
        get
        {
            if (_resMgr == null)
            {
                _resMgr = GetManager<ResourceManager>();
            }
            return _resMgr;
        }
    }

    private static PanelManager _panelMgr;
    protected static PanelManager panelMgr
    {
        get
        {
            if (_panelMgr == null)
            {
                _panelMgr = GetManager<PanelManager>();
            }
            return _panelMgr;
        }
    }

    private static MapManager _mapMgr;
    protected static MapManager mapMgr
    {
        get
        {
            if (_mapMgr == null)
            {
                _mapMgr = GetManager<MapManager>();
            }
            return _mapMgr;
        }
    }

    private static LevelManager _levelMgr;
    protected static LevelManager levelMgr
    {
        get
        {
            if (_levelMgr == null)
            {
                _levelMgr = GetManager<LevelManager>();
            }
            return _levelMgr;
        }
    }

    private static NetworkManager _networkMgr;
    protected static NetworkManager networkMgr
    {
        get
        {
            if (_networkMgr == null)
            {
                _networkMgr = GetManager<NetworkManager>();
            }
            return _networkMgr;
        }
    }

    private static BattleManager _battleMgr;
    public static BattleManager battleMgr
    {
        get
        {
            if (_battleMgr == null)
            {
                _battleMgr = GetManager<BattleManager>();
            }
            return _battleMgr;
        }
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    public BaseBehaviour() { }

    public T Instantiate<T>(T original) where T : UnityEngine.Object
    {
        return GameObject.Instantiate<T>(original);
    }

    public static void Destroy(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            GameObject.Destroy(obj);
        }
    }

    public static void Destroy(UnityEngine.Object obj, float t)
    {
        if (obj != null)
        {
            GameObject.Destroy(obj, t);
        }
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return io.main.StartCoroutine(routine);
    }

    public static void Initialize()
    {
        InitManager();
        InitController();
    }

    /// <summary>
    /// 初始化管理器
    /// </summary>
    static void InitManager()
    {
        AddManager<ConfigManager>();
        AddManager<NPCManager>();
        AddManager<MapManager>();
        AddManager<PanelManager>();
        AddManager<SoundManager>();
        AddManager<TimerManager>();
        AddManager<NetworkManager>();
        AddManager<ResourceManager>();
        AddManager<LevelManager>();
        AddManager<UIManager>();
        AddManager<ObjectManager>();
        AddManager<AtlasManager>();
        AddManager<HandlerManager>();
        AddManager<AnimManager>();
        AddManager<EffectManager>();
        AddManager<BulletManager>();
        AddManager<BattleManager>();
    }

    static T AddManager<T>() where T : BaseManager, new()
    {
        var type = typeof(T);
        var obj = new T();
        Managers.Add(type.Name, obj);
        return obj;
    }

    public static T GetManager<T>() where T : class
    {
        var type = typeof(T);
        if (!Managers.ContainsKey(type.Name))
        {
            return null;
        }
        return Managers[type.Name] as T;
    }

    /// <summary>
    /// 初始化所有控制器
    /// </summary>
    static void InitController()
    {
        AddCtrl(new MainCtrl());
        AddCtrl(new LoaderCtrl());
        AddCtrl(new LoginCtrl());
        AddCtrl(new MessageCtrl());
        AddCtrl(new BattleCtrl());
    }

    /// <summary>
    /// 添加控制器
    /// </summary>
    /// <param name="o"></param>
    static void AddCtrl(BaseCtrl o)
    {
        var type = o.GetType();
        if (!Ctrls.ContainsKey(type.Name))
        {
            Ctrls.Add(type.Name, o);
        }
    }

    /// <summary>
    /// 获取控制器
    /// </summary>
    public static T GetCtrl<T>() where T : class
    {
        var type = typeof(T);
        if (!Ctrls.ContainsKey(type.Name))
        {
            return null;
        }
        return Ctrls[type.Name] as T;
    }

    public static void OnFixedUpdate(float deltaTime)
    {
        foreach (var mgr in Managers)
        {
            if (mgr.Value != null && mgr.Value.isFixedUpdate)
            {
                mgr.Value.OnFixedUpdate(deltaTime);
            }
        }
    }

    /// <summary>
    /// 控制器更新
    /// </summary>
    /// <param name="deltaTime"></param>
    public static void OnUpdate(float deltaTime)
    {
        foreach (var ctrl in Ctrls)
        {
            if (ctrl.Value != null && ctrl.Value.isFrameUpdate)
            {
                ctrl.Value.OnUpdate(deltaTime);
            }
        }
        foreach (var mgr in Managers)
        {
            if (mgr.Value != null && mgr.Value.isOnUpdate)
            {
                mgr.Value.OnUpdate(deltaTime);
            }
        }
    }
}
