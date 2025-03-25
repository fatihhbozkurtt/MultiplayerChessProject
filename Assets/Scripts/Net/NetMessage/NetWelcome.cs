using Data;
using Unity.Collections;
using Unity.Networking.Transport;

namespace Net.NetMessage
{
    public class NetWelcome : NetMessage
    {
        public int AssignedIntTeam { get; set; }
        public Team Team { get; set; }

        public NetWelcome() // <-- Making the box
        {
            Code = OperationCode.WELCOME;
        }

        public NetWelcome(DataStreamReader reader) // <-- Receiving the box
        {
            Code = OperationCode.WELCOME;
            Deserialize(ref reader);
        }

        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(AssignedIntTeam);
            writer.WriteByte((byte)Team);
        }

        public override void Deserialize(ref DataStreamReader reader)
        {
            base.Deserialize(ref reader);

            AssignedIntTeam = reader.ReadInt();
            Team = (Team)reader.ReadByte();
        }
        
        public override void ReceivedOnClient()
        {
            base.ReceivedOnClient();
            NetUtility.C_WELCOME?.Invoke(this);
        }

        public override void ReceivedOnServer(NetworkConnection conn)
        {
            base.ReceivedOnServer(conn);
            NetUtility.S_WELCOME?.Invoke(this, conn);
        }
    }
}