using System;
using System.Collections.Generic;
using MasterServer.Manager;

namespace MasterServer
{
    public class BaseBehaviour
    {
        /// <summary>
        /// 网络管理器
        /// </summary>
        public static NetworkManager NetworkMgr;

        /// <summary>
        /// 配置管理器
        /// </summary>
        public static ConfigManager ConfigMgr;

        /// <summary>
        /// NPC管理器
        /// </summary>
        public static NPCManager NpcMgr;

        /// <summary>
        /// 时间管理器
        /// </summary>
        public static TimerManager TimerMgr;
        
        /// <summary>
        /// 子弹管理器
        /// </summary>
        public static BulletManager BulletMgr;

        /// <summary>
        /// 特效管理器
        /// </summary>
        public static EffectManager EffectMgr;
    }
}
