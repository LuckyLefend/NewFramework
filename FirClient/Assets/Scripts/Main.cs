using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;
using FirClient.Manager;
using LiteNetLib;

public class Main : GameBehaviour
{
    private UIManager uiMgr;
    private LevelManager levelMgr;
    private NetworkManager networkMgr;

    /// <summary>
    /// 初始化游戏管理器
    /// </summary>
    protected override void OnAwake()
    {
        base.OnAwake();
        this.Initialize();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void Initialize()
    {
        BaseBehaviour.Initialize();
        uiMgr = io.GetManager<UIManager>();
        levelMgr = io.GetManager<LevelManager>();
        networkMgr = io.GetManager<NetworkManager>();

        DontDestroyOnLoad(gameObject);  //防止销毁自己
        Debugger.enableLog = AppConst.LogMode;
        Debug.logger.logEnabled = AppConst.LogMode;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = AppConst.GameFrameRate;
        QualitySettings.vSyncCount = 2;

        Screen.SetResolution(1136, 640, false);

        DOTween.Init(true, true, LogBehaviour.Default);

        uiMgr.Initialize();
        networkMgr.Initialize();
        levelMgr.Initialize();
        levelMgr.LoadLevel(LevelType.Login);
    }

    /// <summary>
    /// 固定帧更新
    /// </summary>
    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        BaseBehaviour.OnFixedUpdate(Time.fixedDeltaTime);
    }

    /// <summary>
    /// 每一帧更新
    /// </summary>
    protected override void OnUpdate()
    {
        base.OnUpdate();
        BaseBehaviour.OnUpdate(Time.deltaTime); 
    }

    /// <summary>
    /// 析构函数
    /// </summary>
    protected override void OnDestroyMe()
    {
        if (networkMgr != null)
        {
            networkMgr.OnDispose();
        }
        base.OnDestroyMe();
        Debugger.Log("~GameManager was destroyed");
    }
}