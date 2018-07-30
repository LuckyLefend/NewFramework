using LiteNetLib.Utils;

namespace FirClient.Network
{
    public class PacketData
    {
        public Protocal protocal;
        public NetDataWriter writer;

        public PacketData() { }

        public PacketData(Protocal protocal, NetDataWriter writer)
        {
            this.protocal = protocal;
            this.writer = writer;
        }

        public void Reset()
        {
            protocal = Protocal.Default;
            writer = null;
        }
    }
}

