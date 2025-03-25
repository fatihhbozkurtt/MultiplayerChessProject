using Controllers;
using Data;
using EssentialManagers.Packages.GridManager.Scripts;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace Net.NetMessage
{
    public class NetGetCaptured : NetMessage
    {
        public CaptureData CaptureData;
        public OperationCode Code { get;}
        
        public NetGetCaptured()
        {
            Code = OperationCode.GET_CAPTURED;
        }
        
        public NetGetCaptured(DataStreamReader reader) // <-- Receiving the box
        {
            Code = OperationCode.GET_CAPTURED;
            Deserialize(ref reader);
        }

        public override void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            
            writer.WriteInt(CaptureData.CapturedCellCoordinates.x);
            writer.WriteInt(CaptureData.CapturedCellCoordinates.y);
            
            writer.WriteByte((byte)CaptureData.TeamEnum); 
        }

        public override void Deserialize(ref DataStreamReader reader)
        {
            int x = reader.ReadInt();
            int y = reader.ReadInt();
            
            Team team = (Team)reader.ReadByte();

            CaptureData = new CaptureData()
            {
                CapturedCellCoordinates = new Vector2Int(x, y),
                TeamEnum = team
            };
        }

        public override void ReceivedOnClient()
        {
            NetUtility.C_GET_CAPTURED?.Invoke(this);
        }

        public override void ReceivedOnServer(NetworkConnection conn)
        {
            NetUtility.S_GET_CAPTURED?.Invoke(this, conn);

        }
    }
}

[System.Serializable]
public class CaptureData
{
    public Vector2Int CapturedCellCoordinates;
    public Team TeamEnum;
}