using EssentialManagers.Packages.GridManager.Scripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Controllers
{
    public class Player : MonoBehaviourPunCallbacks
    {
        // Start is called before the first frame update
        void Awake()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected master successfully");
            base.OnConnectedToMaster();

            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Joined Lobby successfully");
            base.OnJoinedLobby();

            PhotonNetwork.JoinOrCreateRoom("Room1", new RoomOptions { MaxPlayers = 2, IsOpen = true, IsVisible = true },
                TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined Room successfully");
            base.OnJoinedRoom();
            
            GridManager.instance.CreateGridWithPhoton();
        }
    }
}