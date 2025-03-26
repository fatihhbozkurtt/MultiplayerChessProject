using System.Collections.Generic;
using EssentialManagers.Packages.GridManager.Scripts;
using UnityEngine;

namespace Controllers.SpecificPieces
{
    public class Knight : PieceController
    {
        protected override List<CellController> GetValidMoves()
        {
            List<CellController> validMoves = new List<CellController>();
            Vector2Int currentPos = CurrentCell.GetCoordinates();

            // All possible L-shaped moves
            Vector2Int[] moveOffsets = {
                new(2, 1), new(2, -1),
                new(-2, 1), new(-2, -1),
                new(1, 2), new(1, -2),
                new(-1, 2), new(-1, -2)
            };

            // Try adding each possible move
            foreach (Vector2Int offset in moveOffsets)
            {
                Vector2Int targetPos = currentPos + offset;
                if (!TryAddMove(validMoves, targetPos, isCapture: false)) 
                {
                    TryAddMove(validMoves, targetPos, isCapture: true);
                }
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