using System;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using MasterServer.Define;
using MasterServer.Manager;
using MasterServer.Message;
using Utility;

namespace MasterServer
{
    public class AppServer
    {
        private static readonly IMessageHandler mDefaultHandler = new DefaultMessageHandler();
        private static readonly Dictionary<string, IManager> mManagers = new Dictionary<string, IManager>();
        private static readonly Dictionary<Protocal, IMessageHandler> mHandlers = new Dictionary<Protocal, IMessageHandler>();

        public bool IsRunning { get; private set; }
        public NetManager mServer { get; private set; }
        private ServerListener mListener = null;

        protected void Initialize(int port)
        {
            IsRunning = false;

            InitHandler();
            InitManager();
            StartServer(port);
        }

        /// <summary>
        /// 初始化服务器
        /// </summary>
        public void StartServer(int port)
        {
            mListener = new ServerListener(this);
            mServer = new NetManager(mListener, 10);
            mServer.Start(port);
            mServer.UpdateTime = 15;
            mServer.DiscoveryEnabled = true;
            IsRunning = true;
            Log.Warn("MasterServer Started!!");
        }

        /// <summary>
        /// 停止服务器
        /// </summary>
        public void StopServer()
        {
            if (mServer != null)
            {
                mServer.Stop();
                mServer = null;
            }
            IsRunning = false;
            Log.Warn("MasterServer Stoped!!");
        }

        /// <summary>
        /// 初始化管理器
        /// </summary>
        private void InitManager()
        {
            mManagers.Add(ManagerNames.NETWORK, new NetworkManager());
            mManagers.Add(ManagerNames.CONFIG, new ConfigManager());
            mManagers.Add(ManagerNames.NPC, new NPCManager());
            mManagers.Add(ManagerNames.TIMER, new TimerManager());
            mManagers.Add(ManagerNames.BULLET, new BulletManager());
            mManagers.Add(ManagerNames.EFFECT, new EffectManager());

            ///初始化所有管理器
            foreach (var de in mManagers)
            {
                if (de.Value != null)
                {
                    de.Value.Initialize();
                }
            }
        }

        /// <summary>
        /// 初始化消息处理器映射
        /// </summary>
        private void InitHandler()
        {
            mHandlers.Add(Protocal.Disconnect, new DisconnectHandler());
            mHandlers.Add(Protocal.ReqUserInfo, new ReqUserInfoHandler());
            mHandlers.Add(Protocal.ReqMapInfo, new ReqMapInfoHandler());
            mHandlers.Add(Protocal.ReqNpcInfo, new ReqNpcsHandler());
            mHandlers.Add(Protocal.ReqOpenFire, new ReqOpenFireHandler());
            mHandlers.Add(Protocal.ReqHitTarget, new ReqHitTargetHandler());
            mHandlers.Add(Protocal.SerializeFields, new ReqSerializeFieldHandler());
        }

        /// <summary>
        /// 获取处理器
        /// </summary>
        public IMessageHandler GetHandler(Protocal protocal)
        {
            return mHandlers[protocal];
        }

        public void OnUpdate()
        {
            if (mServer != null)
            {
                mServer.PollEvents();
            }
            ///更新管理器
            foreach (var de in mManagers)
            {
                if (de.Value != null)
                {
                    de.Value.OnFrameUpdate();
                }
            }
        }

        /// <summary>
        /// 处理数据
        /// </summary>
        public void DispatchMessage(NetPeer peer, NetDataReader reader)
        {
            var messageid = reader.GetUShort();
            Protocal key = (Protocal)messageid;
            if (mHandlers.ContainsKey(key))
            {
                mHandlers[key].OnMessage(peer, reader);
            }
            else
            {
                mDefaultHandler.OnMessage(peer, reader);
            }
        }
    }
}
