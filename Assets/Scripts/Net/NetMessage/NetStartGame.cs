using Unity.Collections;
using Unity.Networking.Transport;

namespace Net.NetMessage
{
    public class NetStartGame : NetMessage
    {
        public NetStartGame()
        {
            Code = OperationCode.START_GAME;
        }
        
        public NetStartGame(DataStreamReader reader) // <-- Receiving the box
        {
            Code = OperationCode.START_GAME;
            Deserialize(ref reader);
        }

        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
        }

        public override void Deserialize(ref DataStreamReader reader)
        {
            base.Deserialize(ref reader);
        }
        
        public override void ReceivedOnClient()
        {
            NetUtility.C_START_GAME?.Invoke(this);
        }

        public override void ReceivedOnServer(NetworkConnection conn)
        { 
            NetUtility.S_START_GAME?.Invoke(this, conn);
        }
    }
}