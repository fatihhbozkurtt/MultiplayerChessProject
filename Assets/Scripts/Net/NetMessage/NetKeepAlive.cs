using Unity.Collections;
using Unity.Networking.Transport;

namespace Net.NetMessage
{
    public class NetKeepAlive : NetMessage
    {
        public NetKeepAlive() // <-- Making the box
        {
            Code = OperationCode.KEEP_ALIVE;
        }

        public NetKeepAlive(DataStreamReader reader) // <-- Receiving the box
        {
            Code = OperationCode.KEEP_ALIVE;
            Deserialize(ref reader);
        }

        public override void Serialize(ref DataStreamWriter writer)
        {
            base.Serialize(ref writer);
            writer.WriteByte((byte)Code);
        }

        public override void ReceivedOnClient()
        {
            base.ReceivedOnClient();
            NetUtility.C_KEEP_ALIVE?.Invoke(this);
        }

        public override void ReceivedOnServer(NetworkConnection conn)
        {
            base.ReceivedOnServer(conn);
            NetUtility.S_KEEP_ALIVE?.Invoke(this, conn);
        }
    }
}