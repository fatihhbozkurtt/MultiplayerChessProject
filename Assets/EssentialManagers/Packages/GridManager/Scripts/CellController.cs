using System.Collections.Generic;
using System.Data;
using Controllers;
using Data;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace EssentialManagers.Packages.GridManager.Scripts
{
    public class CellController : MonoBehaviour
    {
        [Header("Debug")] public bool isPickable;
        public bool isOccupied;

        [FormerlySerializedAs("spawnedOccupierObj")] [SerializeField]
        private PieceController currentPiece;

        [SerializeField] Vector2Int coordinates;
        [SerializeField] MeshRenderer meshRenderer;

        public List<CellController> neighbours;

        [SerializeField] private PieceAttributes pieceAttribute;

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
                pieceAttribute = DataExtensions.GetPieceDataByType(PieceType.Pawn);
                canSpawnPiece = true;
            }

            // Spawn Rooks
            if (coordinates == Vector2Int.zero || coordinates == new Vector2Int(7, 0) ||
                coordinates == new Vector2Int(0, 7) || coordinates == new Vector2Int(7, 7))
            {
                pieceAttribute = DataExtensions.GetPieceDataByType(PieceType.Rook);
                canSpawnPiece = true;
            }

            // Spawn Knights
            if (coordinates == new Vector2Int(1, 0) || coordinates == new Vector2Int(6, 0) ||
                coordinates == new Vector2Int(1, 7) || coordinates == new Vector2Int(6, 7))
            {
                pieceAttribute = DataExtensions.GetPieceDataByType(PieceType.Knight);
                canSpawnPiece = true;
            }

            // Spawn Bishops
            if (coordinates == new Vector2Int(2, 0) || coordinates == new Vector2Int(5, 0) ||
                coordinates == new Vector2Int(2, 7) || coordinates == new Vector2Int(5, 7))
            {
                pieceAttribute = DataExtensions.GetPieceDataByType(PieceType.Bishop);
                canSpawnPiece = true;
            }

            // Spawn Queens
            if (coordinates == new Vector2Int(3, 0) || coordinates == new Vector2Int(3, 7))
            {
                pieceAttribute = DataExtensions.GetPieceDataByType(PieceType.Queen);
                canSpawnPiece = true;
            }

            // Spawn Kings
            if (coordinates == new Vector2Int(4, 0) || coordinates == new Vector2Int(4, 7))
            {
                pieceAttribute = DataExtensions.GetPieceDataByType(PieceType.King);
                canSpawnPiece = true;
            }

            if (!canSpawnPiece) return;

            #endregion

            GameObject clone = PhotonNetwork.Instantiate(pieceAttribute.PieceType.ToString(), transform.position,
                Quaternion.identity);
            currentPiece = clone.GetComponent<PieceController>();
            currentPiece.Initialize(this, pieceAttribute);
            SetOccupied(currentPiece);
        }


        private void OnMouseDown()
        {
            if (!GameManager.instance.isLevelActive) return;
            if (!isOccupied) return;

            if (currentPiece != null)
                currentPiece.gameObject.SetActive(false);
        }

        #region GETTERS & SETTERS

        public void SetMaterialColor(Color color)
        {
            meshRenderer.material.color = color;
        }

        public void SetOccupied(PieceController pc)
        {
            currentPiece = pc;
            isOccupied = true;
        }

        public void SetFree()
        {
            currentPiece = null;
            isOccupied = false;
        }

        public PieceController GetOccupierObject()
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