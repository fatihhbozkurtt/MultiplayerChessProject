using System.Collections.Generic;
using System.Linq;
using Data;
using Net;
using Net.NetMessage;
using Photon.Pun;
using Unity.Networking.Transport;
using UnityEngine;

namespace EssentialManagers.Packages.GridManager.Scripts
{
    public class GridManager : MonoSingleton<GridManager>
    {
        [Header("Config")] public int gridWidth = 10; // Width of the grid
        public int gridHeight = 10; // Height of the grid
        public float cellSpacing = 1f; // Spacing between cells

        [Header("References")] public GameObject cellPrefab; // Prefab for the cell

        [Header("Debug")] public List<CellController> gridPlan;

        [Header("Multiplayer Logic")] private int _playerCount;
        private Team _currentTeamEnum;


        protected void Start()
        {
            CreateGrid();
        }

        protected override void Awake()
        {
            base.Awake();

            RegisterEvents();
        }
        
        private void CreateGrid()
        {
            for (var x = 0; x < gridWidth; x++)
            {
                for (var y = 0; y < gridHeight; y++)
                {
                    Vector2Int coordinates = new Vector2Int(x, y);

                    GameObject cell = Instantiate(cellPrefab,
                        new Vector3(x * cellSpacing, 0, y * cellSpacing), Quaternion.identity);

                    CellController cellController = cell.GetComponent<CellController>();
                    cellController.Initialize(coordinates);
                    cell.transform.parent = transform;
                    gridPlan.Add(cellController);

                    // Assign color based on (x + y) % 2
                    bool isBlack = (x + y) % 2 == 0;
                    Color tileColor = isBlack ? Color.black : Color.white;
                    cellController.SetMaterialColor(tileColor);
                }
            }
        }

        #region Events Region

        private void RegisterEvents()
        {
            NetUtility.S_WELCOME += OnWelcomeServer;

            NetUtility.C_WELCOME += OnWelcomeClient;
            NetUtility.C_START_GAME += OnStartGameClient;
        }


        private void UnregisterEvents()
        {
        }

        // Server
        private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
        {
            // Client has connected, assign a team and return the message back to him
            NetWelcome nw = msg as NetWelcome;

            // Assign team
            nw.AssignedIntTeam = ++_playerCount;

            // Get all enum values
            Team[] teamValues = (Team[])System.Enum.GetValues(typeof(Team));
            nw.Team = teamValues[nw.AssignedIntTeam]; // Assign the team at the specified index

            Debug.Log($" 1) My assigned team: {nw.Team}, int value: {_playerCount}");

            // Return back to the client
            Server.instance.SendToClient(cnn, nw);


            if (_playerCount == 2)
            {
                Server.instance.Broadcast(new NetStartGame());
            }
        }

        // Client
        int _tempIntTeam;

        private void OnWelcomeClient(NetMessage msg)
        {
            // Receive the connection message
            NetWelcome nw = msg as NetWelcome;

            // Assign team 
            if (nw != null)
            {
                _tempIntTeam = nw.AssignedIntTeam;
                _currentTeamEnum = nw.Team;

                Debug.Log($" 2) My assigned team: {nw.Team}, int value: {nw.AssignedIntTeam}");
            }
            else
            {
                Debug.Log(" 3) My assigned team null");
            }
        }

        private void OnStartGameClient(NetMessage msg)
        {
            GameManager.instance.StartGame();

            CameraManager.instance.SetCam(_currentTeamEnum == Team.White
                ? CameraManager.CamType.WhitePlayer
                : CameraManager.CamType.BlackPlayer);
 
        }

        #endregion

        #region Helper Methods

        public Team GetCurrentTeam() => _currentTeamEnum;
        
        public CellController GetClosestGridCell(Vector3 from)
        {
            if (gridPlan == null || gridPlan.Count == 0)
            {
                Debug.LogWarning("GridPlan list is empty or null!");
                return null;
            }

            CellController closestCellController = null;
            float closestDistance = Mathf.Infinity;

            for (int i = 0; i < gridPlan.Count; i++)
            {
                CellController cellController = gridPlan[i];
                float distance = Vector3.Distance(cellController.transform.position, from);

                if (distance < closestDistance)
                {
                    closestCellController = cellController;
                    closestDistance = distance;
                }
            }

            return closestCellController;
        }

        public CellController GetGridCellByCoordinates(Vector2Int coordinates)
        {
            if (gridPlan == null || gridPlan.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < gridPlan.Count; i++)
            {
                CellController cellController = gridPlan[i];
                if (cellController.GetCoordinates() == coordinates)
                {
                    return cellController;
                }
            }


            return null;
        }

        #endregion
    }
}