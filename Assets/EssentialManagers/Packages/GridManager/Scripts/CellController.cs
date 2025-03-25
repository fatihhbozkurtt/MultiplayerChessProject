using System.Collections.Generic;
using Controllers;
using Data;
using Net.NetMessage;
using UnityEngine;

namespace EssentialManagers.Packages.GridManager.Scripts
{
    public class CellController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        GameObject initialMesh;

        [SerializeField] GameObject highlightMesh;

        [Header("Debug")] public bool isPickable;
        public bool isOccupied;
        [SerializeField] private PieceController currentPiece;
        public PieceController PreviousPiece;

        [SerializeField] Vector2Int coordinates;
        [SerializeField] MeshRenderer meshRenderer;
        public List<CellController> neighbours;
        [SerializeField] private PieceData pieceData;

        private void Start()
        {
            name = coordinates.ToString();
            neighbours = GetNeighbors();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void Initialize(Vector2Int initCoords)
        {
            meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            coordinates = initCoords;
            HandlePieceSpawning();
        }

        // Helper method to instantiate and assign piece
        private void HandlePieceSpawning()
        {
            #region Assign Piece Type

            bool canSpawnPiece = false;

            // Spawn Pawns
            if (coordinates.y == 1 || coordinates.y == 6)
            {
                pieceData = DataExtensions.GetPieceDataByType(PieceType.Pawn);
                canSpawnPiece = true;
            }

            // Spawn Rooks
            if (coordinates == Vector2Int.zero || coordinates == new Vector2Int(7, 0) ||
                coordinates == new Vector2Int(0, 7) || coordinates == new Vector2Int(7, 7))
            {
                pieceData = DataExtensions.GetPieceDataByType(PieceType.Rook);
                canSpawnPiece = true;
            }

            // Spawn Knights
            if (coordinates == new Vector2Int(1, 0) || coordinates == new Vector2Int(6, 0) ||
                coordinates == new Vector2Int(1, 7) || coordinates == new Vector2Int(6, 7))
            {
                pieceData = DataExtensions.GetPieceDataByType(PieceType.Knight);
                canSpawnPiece = true;
            }

            // Spawn Bishops
            if (coordinates == new Vector2Int(2, 0) || coordinates == new Vector2Int(5, 0) ||
                coordinates == new Vector2Int(2, 7) || coordinates == new Vector2Int(5, 7))
            {
                pieceData = DataExtensions.GetPieceDataByType(PieceType.Bishop);
                canSpawnPiece = true;
            }

            // Spawn Queens
            if (coordinates == new Vector2Int(3, 0) || coordinates == new Vector2Int(3, 7))
            {
                pieceData = DataExtensions.GetPieceDataByType(PieceType.Queen);
                canSpawnPiece = true;
            }

            // Spawn Kings
            if (coordinates == new Vector2Int(4, 0) || coordinates == new Vector2Int(4, 7))
            {
                pieceData = DataExtensions.GetPieceDataByType(PieceType.King);
                canSpawnPiece = true;
            }

            if (!canSpawnPiece) return;

            #endregion

            // GameObject clone = PhotonNetwork.Instantiate(pieceData.PieceType.ToString(), transform.position,
            //     Quaternion.identity);
            // currentPiece = clone.GetComponent<PieceController>();

            GameObject pieceClone = Instantiate(pieceData.PiecePrefab, transform.position,
                Quaternion.identity);
            currentPiece = pieceClone.GetComponent<PieceController>();
            currentPiece.Initialize(this, pieceData);
            SetOccupied(currentPiece);
        }


        private void OnMouseDown()
        {
            if (!GameManager.instance.isLevelActive) return;
            GridManager.instance.AssignNewSelectedCell(this);
            // if (currentPiece == null)
            //     return;
            //
            //
            // if (GridManager.instance.GetCurrentTeam() != currentPiece.Team)
            //     return;
              
        }

        public void GetHighlighted()
        {
            highlightMesh.SetActive(true);
            initialMesh.SetActive(false);
        }

        public void RemoveHighlight()
        {
            highlightMesh.SetActive(false);
            initialMesh.SetActive(true);
        }

        #region GETTERS & SETTERS

        public void SetMaterialColor(Color color)
        {
            meshRenderer.material.color = color;
        }

        public void SetOccupied(PieceController pc, bool checkForCapturing = false)
        {
            if (checkForCapturing && currentPiece != null) TriggerCapturing();
            currentPiece = pc;
            isOccupied = true;
        }

        private void TriggerCapturing()
        {
            currentPiece.GetCaptured();
        }
        
        public void SetFree()
        {
            PreviousPiece = currentPiece;
            currentPiece = null;
            isOccupied = false;
        }

        public PieceController GetCurrentPiece()
        {
            return currentPiece;
        }

        public Vector2Int GetCoordinates()
        {
            return coordinates;
        }

        private List<CellController> GetNeighbors()
        {
            List<CellController> gridCells = GridManager.instance.gridPlan;
            List<CellController> neighbors = new();

            // Direction vectors for 8 directions (including diagonals)
            int[] dx = { 1, 1, 0, -1, -1, -1, 0, 1 };
            int[] dz = { 0, 1, 1, 1, 0, -1, -1, -1 };

            for (int i = 0; i < dx.Length; i++)
            {
                Vector2Int neighborCoordinates = coordinates + new Vector2Int(dx[i], dz[i]);
                CellController neighbor = gridCells.Find(cell => cell.coordinates == neighborCoordinates);

                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        #endregion
    }
}