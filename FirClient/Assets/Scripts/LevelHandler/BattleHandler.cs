using System.Collections;
using System.Collections.Generic;
using FirClient.Ctrl;
using UnityEngine;

public class BattleHandler : LevelBase
{
    private BattleCtrl battleCtrl;

    public BattleHandler()
    {
        battleCtrl = io.GetCtrl<BattleCtrl>();
    }

    public override void OnEnterLevel()
    {
        base.OnEnterLevel();

        var mapName = "Map001";
        mapMgr.CreateMap(mapName, delegate(GameObject gameObj)
        {
            mapMgr.CreateMapEvent();
            if (gameObj != null)
            {
                gameObj.name = mapName;
                gameObj.transform.SetParent(io.battleObject);
                gameObj.transform.localPosition = Vector3.zero;
                gameObj.transform.localScale = Vector3.one;
            }
            var mainCtrl = io.GetCtrl<MainCtrl>();
            if (mainCtrl != null)
            {
                battleCtrl.OnAwake();
            }
            atlasMgr.InitAtlas("Tanks", AppConst.TankAtlasPath);
            atlasMgr.InitAtlas("Animals", AppConst.AnimalAtlasPath);

            networkMgr.SendData(Protocal.ReqUserInfo);
            networkMgr.SendData(Protocal.ReqNpcInfo);

            timerMgr.Initialize();
            battleMgr.Initialize();
        });
    }

    public override void OnLeaveLevel()
    {
        battleCtrl.OnDestroy();
        base.OnLeaveLevel();
    }
}
