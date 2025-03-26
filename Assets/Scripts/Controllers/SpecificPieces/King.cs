using System.Collections.Generic;
using EssentialManagers.Packages.GridManager.Scripts;
using UnityEngine;

namespace Controllers.SpecificPieces
{
    public class King : PieceController
    {
        protected override List<CellController> GetValidMoves()
        {
            List<CellController> validMoves = new List<CellController>();
            Vector2Int currentPos = CurrentCell.GetCoordinates();

            // All possible king moves (one square in all directions)
            Vector2Int[] directions = {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, 
                new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1)
            };

            foreach (var direction in directions)
            {
                TryAddMove(validMoves, currentPos + direction, isCapture: true);
            }

            return validMoves;
        }

        protected override bool TryAddMove(List<CellController> moves, Vector2Int targetCoord, bool isCapture)
        {
            CellController targetCell = GridManager.instance.GetGridCellByCoordinates(targetCoord);
            if (targetCell == null) return false; // Out of bounds

            if (!isCapture && !targetCell.isOccupied) // Normal move
            {
                moves.Add(targetCell);
                return true;
            }
            else if (isCapture && targetCell.isOccupied && targetCell.GetCurrentPiece().Team != Team) // Capture move
            {
                moves.Add(targetCell);
                return true;
            }

            return false;
        }

        protected override void OnMoveComplete()
        {
            ;
        }
    }
}