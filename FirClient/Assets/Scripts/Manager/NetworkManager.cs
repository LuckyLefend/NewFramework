using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Threading;
using FirClient.Network;
using FirClient.View;
using FirClient.Message;

namespace FirClient.Manager
{
    public partial class NetworkManager : BaseManager
    {
        private static readonly object msgLock = new object();

        private static readonly NetDataWriter writer = new NetDataWriter();
        private static readonly IMessageHandler mDefaultHandler = new DefaultHandler();
        private static readonly Queue<PacketData> mPacketPool = new Queue<PacketData>();
        private static readonly List<NetworkObject> mNetworkObjects = new List<NetworkObject>();
        private static readonly Dictionary<Protocal, IMessageHandler> mHandlers = new Dictionary<Protocal, IMessageHandler>();

        public NetManager mClient { get; private set; }
        private Action connectOK;

        public override void Initialize()
        {
            InitHandler();
            
            var listener = new ClientListener(this);
            mClient = new NetManager(listener);
            //client.SimulateLatency = true;
            mClient.UpdateTime = 15;
            if (!mClient.Start())
            {
                Console.WriteLine("Client start failed");
            }
            isOnUpdate = true;
            isFixedUpdate = true;

            StartNetworkObjectUpdate();
        }

        /// <summary>
        /// 注册消息处理器
        /// </summary>
        void InitHandler()
        {
            mHandlers.Add(Protocal.Disconnect, new DisconnectHandler());
            mHandlers.Add(Protocal.ReqUserInfo, new RetUserInfoHandler());
            mHandlers.Add(Protocal.ReqMapInfo, new RetMapInfoHandler());
            mHandlers.Add(Protocal.ReqNpcInfo, new RetNpcsHandler());
            mHandlers.Add(Protocal.ReqOpenFire, new RetOpenFireHandler());
            mHandlers.Add(Protocal.ReqHitTarget, new RetHitTargetHandler());

            mHandlers.Add(Protocal.RetNewPlayer, new RetNewPlayerHandler());
            mHandlers.Add(Protocal.SerializeFields, new RetSerializeFieldHandler());
        }

        /// <summary>
        /// 启动网络对象更新
        /// </summary>
        private void StartNetworkObjectUpdate()
        {
            Task.Queue(() =>
            {
                while (true)
                {
                    lock (mNetworkObjects)
                    {
                        for (int i = 0; i < mNetworkObjects.Count; i++)
                        {
                            mNetworkObjects[i].HeartBeat(0);
                        }
                    }
                    Thread.Sleep(10);
                }
            });
        }

        public override void OnFixedUpdate(float deltaTime)
        {
            this.OnInterpolateUpdate(deltaTime);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (mClient != null)
            {
                this.OnSocketUpdate();
                this.OnProcessPack();
            }
        }

        /// <summary>
        /// Socket更新
        /// </summary>
        private void OnSocketUpdate()
        {
            mClient.PollEvents();

            var peer = mClient.GetFirstPeer();
            if (peer == null || peer.ConnectionState != ConnectionState.Connected)
            {
                mClient.SendDiscoveryRequest(new byte[] { 1 }, 5000);
            }
        }
        
        /// <summary>
        /// 处理缓存池
        /// </summary>
        private void OnProcessPack()
        {
            var peer = mClient.GetFirstPeer();
            if (peer != null && peer.ConnectionState == ConnectionState.Connected)
            {
                lock (msgLock)
                {
                    while (mPacketPool.Count > 0)
                    {
                        SendPacketData(mPacketPool.Dequeue());
                    }
                }
            }
        }

        /// <summary>
        /// 插值更新
        /// </summary>
        private void OnInterpolateUpdate(float deltaTime)
        {
            foreach (var de in npcMgr.Npcs)
            {
                var npcView = de.Value as NPCView;
                if (npcView != null)
                {
                    npcView.InterpolateUpdate();
                }
            }
        }

        /// <summary>
        /// Connect to the server.
        /// </summary>
        public void Connect(Action connectOK)
        {
            this.connectOK = connectOK;
            var addr = AppConst.SocketAddress;
            var port = AppConst.SocketPort;
            mClient.Connect(addr, port, AppConst.AppName);
            Debugger.LogWarning("Connect Server Address:{0} Port:{1}", addr, port);
        }

        public void OnConnected(NetPeer peer)
        {
            if (connectOK != null)
            {
                connectOK.Invoke();
            }
            Debugger.LogWarning("Server Connected!!");
        }

        /// <summary>
        /// 消息接收
        /// </summary>
        public void OnReceived(NetPeer peer, NetDataReader reader)
        {
            IMessageHandler handler = null;
            Protocal key = (Protocal)reader.GetUShort();
            if (mHandlers.ContainsKey(key))
            {
                handler = mHandlers[key];
            }
            else
            {
                handler = mDefaultHandler;
            }
            handler.OnMessage(peer, reader);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="protocal"></param>
        /// <param name="buffer"></param>
        public void SendData(Protocal protocal, NetDataWriter buffer = null) 
        {
            if (mClient != null)
            {
                lock(msgLock) 
                {
                    var packet = objMgr.Get<PacketData>();
                    packet.protocal = protocal;
                    packet.writer = buffer;
                    mPacketPool.Enqueue(packet);
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        private void SendPacketData(PacketData data)
        {
            if (data != null)
            {
                writer.Reset();
                writer.Put((ushort)data.protocal);
                if (data.writer != null)
                {
                    writer.Put(data.writer.Data);
                }
                var peer = mClient.GetFirstPeer(); 
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
                objMgr.Release<PacketData>(data);
            }
        }

        /// <summary>
        /// 添加网络对象
        /// </summary>
        public void AddNetworkObject(NetworkObject view)
        {
            lock (mNetworkObjects)
            {
                mNetworkObjects.Add(view);
            }
        }

        /// <summary>
        /// 移除网络对象
        /// </summary>
        public void RemoveNetworkObject(NetworkObject view)
        {
            lock (mNetworkObjects)
            {
                mNetworkObjects.Remove(view);
            }
        }

        public override void OnDispose()
        {
            if (mClient != null)
            {
                mClient.Stop();
                mClient = null;
            }
            Debugger.Log("~NetworkManager was destroy");
        }
    }
}