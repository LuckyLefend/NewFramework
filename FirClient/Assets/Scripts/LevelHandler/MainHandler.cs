using UnityEngine;
using System.Collections;
using Common;
using FirClient.Ctrl;
using FirClient.Manager;

public class MainHandler : LevelBase
{
    private MainCtrl mainCtrl;

    public MainHandler()
    {
        mainCtrl = io.GetCtrl<MainCtrl>();
    }

    public override void OnEnterLevel()
    {
        base.OnEnterLevel();
        mainCtrl.OnAwake();
    }

    public override void OnLeaveLevel()
    {
        mainCtrl.OnDestroy();
        base.OnLeaveLevel();
    }
}
