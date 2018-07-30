using System.Collections;
using System.Collections.Generic;
using FirClient.Manager;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace FirClient.Network
{
    public class ClientListener : INetEventListener
    {
        private NetworkManager networkMgr;
        public ClientListener(NetworkManager manager)
        {
            networkMgr = manager;
        }

        public void OnPeerConnected(NetPeer peer)
        {
            if (networkMgr != null)
            {
                networkMgr.OnConnected(peer);
            }
            Debugger.LogWarning("[CLIENT] OnPeerConnected: " + peer.ConnectId);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debugger.LogError("[CLIENT] disconnected: " + disconnectInfo.Reason);
        }

        public void OnNetworkReceive(NetPeer peer, NetDataReader reader, DeliveryMethod deliveryMethod)
        {
            if (networkMgr != null)
            {
                networkMgr.OnReceived(peer, reader);
            }
        }

        public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
        {
            Debugger.LogError("[CLIENT] error! " + socketErrorCode);
        }

        public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.DiscoveryResponse)
            {
                Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
                if (networkMgr.mClient != null && networkMgr.mClient.PeersCount == 0)
                {
                    networkMgr.mClient.Connect(remoteEndPoint, AppConst.AppName);
                }
            }
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            throw new System.NotImplementedException();
        }
    }

}
