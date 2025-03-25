using Data;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace Net.NetMessage
{
    public class NetMakeMove : NetMessage
    {
        public MoveData MoveData;
        
        public OpCode Code { get;}
        
        public NetMakeMove()
        {
            Code = OpCode.MAKE_MOVE;
        }
        
        public NetMakeMove(DataStreamReader reader) // <-- Receiving the box
        {
            Code = OpCode.MAKE_MOVE;
            Deserialize(ref reader);
        }

        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code); // Writing the enum as a byte
    
            writer.WriteInt(MoveData.OriginalCoord.x);
            writer.WriteInt(MoveData.OriginalCoord.y);
    
            writer.WriteInt(MoveData.TargetCoord.x);
            writer.WriteInt(MoveData.TargetCoord.y);
    
            writer.WriteByte((byte)MoveData.TeamEnum); // Assuming TeamEnum is an enum
        }

        public override void Deserialize(ref DataStreamReader reader)
        { 
            int originalX = reader.ReadInt();
            int originalY = reader.ReadInt();
    
            int targetX = reader.ReadInt();
            int targetY = reader.ReadInt();
    
            Team team = (Team)reader.ReadByte();

            MoveData = new MoveData
            {
                OriginalCoord = new Vector2Int(originalX, originalY),
                TargetCoord = new Vector2Int(targetX, targetY),
                TeamEnum = team
            };
        }
        
        public override void ReceivedOnClient()
        {
            NetUtility.C_MAKE_MOVE?.Invoke(this);
        }

        public override void ReceivedOnServer(NetworkConnection conn)
        { 
            NetUtility.S_MAKE_MOVE?.Invoke(this, conn);
        }
    }
    
}

[System.Serializable]
public class MoveData
{ 
    public Vector2Int OriginalCoord;
    public Vector2Int TargetCoord;
    public Team TeamEnum;
}