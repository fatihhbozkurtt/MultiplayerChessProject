using System;
using Net.NetMessage;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace Net
{
    public static class NetUtility
    {
        // Net Messages
        public static Action<NetMessage.NetMessage> C_KEEP_ALIVE;
        public static Action<NetMessage.NetMessage> C_WELCOME;
        public static Action<NetMessage.NetMessage> C_START_GAME;
        public static Action<NetMessage.NetMessage> C_MAKE_MOVE;
        public static Action<NetMessage.NetMessage> C_GET_CAPTURED;
        public static Action<NetMessage.NetMessage> C_REMATCH;
        public static Action<NetMessage.NetMessage, NetworkConnection> S_KEEP_ALIVE;
        public static Action<NetMessage.NetMessage, NetworkConnection> S_WELCOME;
        public static Action<NetMessage.NetMessage, NetworkConnection> S_START_GAME;
        public static Action<NetMessage.NetMessage, NetworkConnection> S_MAKE_MOVE;
        public static Action<NetMessage.NetMessage, NetworkConnection> S_GET_CAPTURED;
        public static Action<NetMessage.NetMessage, NetworkConnection> S_REMATCH;


        public static void OnData(DataStreamReader stream, NetworkConnection conn, Server server = null)
        {
            NetMessage.NetMessage msg = null;
            var opCode = (OperationCode)stream.ReadByte();

            switch (opCode)
            {
                case OperationCode.KEEP_ALIVE: msg = new NetKeepAlive(stream); break;
                case OperationCode.WELCOME: msg = new NetWelcome(stream); break;
                case OperationCode.START_GAME: msg = new NetStartGame(stream); break;
                case OperationCode.MAKE_MOVE: msg = new NetMakeMove(stream); break;
                case OperationCode.GET_CAPTURED: msg = new NetGetCaptured(stream); break;
                // case OpCode.REMATCH: msg = new NetRematch(stream); break;
                default:
                    Debug.LogError("Message received had no OpCode");
                    throw new ArgumentOutOfRangeException();
            }

            if (server != null)
                msg.ReceivedOnServer(conn);
            else
                msg.ReceivedOnClient();
        }
    }
}

public enum OperationCode
{
    KEEP_ALIVE,
    WELCOME,
    START_GAME, 
    MAKE_MOVE,
    GET_CAPTURED,
    REMATCH,
}