using UnityEngine;
using System.Collections;
using Common;
using FirClient.Ctrl;
using FirClient.Manager;

public class LoginHandler : LevelBase
{
    private LoginCtrl loginCtrl;

    public LoginHandler()
    {
        loginCtrl = io.GetCtrl<LoginCtrl>();
    }

    public override void OnEnterLevel()
    {
        loginCtrl.OnAwake();
        cfgMgr.Initialize();
        objMgr.Initialize();
        bulletMgr.Initialize();
        effectMgr.Initialize();
    }

    public override void OnLeaveLevel()
    {
        loginCtrl.OnDestroy();
    }
}
