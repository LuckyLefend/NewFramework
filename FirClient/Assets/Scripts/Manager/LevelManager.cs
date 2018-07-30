using UnityEngine;
using System.Collections;
using Common;
using FirClient.Ctrl;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum LevelType { 
    Init = 0,
    Login = 1,
    Loader = 2,
    Main = 3,
    Battle = 4,
}

namespace FirClient.Manager
{
    public class LevelManager : BaseManager
    {
        private LevelType newLevel;
        private LoaderCtrl loaderCtrl;
        private Dictionary<LevelType, LevelBase> levels = new Dictionary<LevelType, LevelBase>();

        // Use this for initialization
        public override void Initialize()
        {
            levels.Add(LevelType.Login, new LoginHandler());
            levels.Add(LevelType.Loader, new LoaderHandler());
            levels.Add(LevelType.Main, new MainHandler());
            levels.Add(LevelType.Battle, new BattleHandler());
        }

        public override void OnFixedUpdate(float deltaTime)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public LevelBase GetLevelHander(LevelType type)
        {
            LevelBase level = null;
            levels.TryGetValue(type, out level);
            return level;
        }

        /// <summary>
        /// 载入关卡
        /// </summary>
        /// <param name="name"></param>
        public void LoadLevel(LevelType level)
        {
            newLevel = level;
            if (loaderCtrl == null)
            {
                loaderCtrl = io.GetCtrl<LoaderCtrl>();
            }
            loaderCtrl.InitLoader(delegate()
            {
                var scene = SceneManager.GetActiveScene();
                LevelType currLevel = (LevelType)scene.buildIndex;
                StartCoroutine(OnLoadLevel(currLevel, true));
            });
        }

        IEnumerator OnLoadLevel(LevelType level, bool isloader = false)
        {
            int levelid = isloader ? (int)LevelType.Loader : (int)level;
            AsyncOperation op = SceneManager.LoadSceneAsync(levelid);
            yield return op;
            yield return new WaitForSeconds(0.1f);

            if (isloader)
            {
                loaderCtrl.OnLeaveLevel(level, delegate()
                {
                    StartCoroutine(OnLoadLevel(newLevel));
                });
            }
            else
            {
                loaderCtrl.OnEnterLevel(newLevel, delegate()
                {
                    loaderCtrl.CloseLoader();
                });
            }
        }

        public override void OnDispose()
        {
            throw new System.NotImplementedException();
        }
    }
}