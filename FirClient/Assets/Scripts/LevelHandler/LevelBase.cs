using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FirClient.Manager;

public class LevelBase : BaseBehaviour
{
    public virtual void OnEnterLevel()
    {
        mapMgr.OnEnterMapEvent();
    }

    public virtual void OnLeaveLevel()
    {
        mapMgr.OnLevelMapEvent();
    }
}
