using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using MasterServer.Define;
using Utility;

namespace MasterServer
{
    public class ServerListener : INetEventListener
    {
        private AppServer appServer;

        public ServerListener(AppServer server)
        {
            appServer = server; 
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Log.Info(peer.EndPoint + " OnConnected!!"); 
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (appServer != null)
            {
                var handler = appServer.GetHandler(Protocal.Disconnect);
                handler.OnMessage(peer, null);
            }
            Log.Error("ConnectId:>" + peer.ConnectId + " Disconnected!");
        }

        public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
        {
            Log.Error("OnNetworkError------>>>" + socketErrorCode);
        }

        public void OnNetworkReceive(NetPeer peer, NetDataReader reader, DeliveryMethod deliveryMethod)
        {
            if (appServer != null)
            {
                appServer.DispatchMessage(peer, reader);
            }
        }

        public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.DiscoveryRequest)
            {
                Log.Warn("[SERVER] Received discovery request. Send discovery response");
                if (appServer != null && appServer.mServer != null)
                {
                    appServer.mServer.SendDiscoveryResponse(new byte[] { 1 }, remoteEndPoint);
                }
            }
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey(AppConst.AppName);
            Log.Info("OnConnectionRequest--->>" + request.RemoteEndPoint);
        }
    }
}
