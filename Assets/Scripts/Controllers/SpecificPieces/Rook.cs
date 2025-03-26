using System.Collections.Generic;
using EssentialManagers.Packages.GridManager.Scripts;
using UnityEngine;

namespace Controllers.SpecificPieces
{
    public class Rook : PieceController
    {
        protected override List<CellController> GetValidMoves()
        {
            List<CellController> validMoves = new List<CellController>();
            Vector2Int currentPos = CurrentCell.GetCoordinates();

            // Check all four directions (Up, Down, Left, Right)
            TryAddDirectionalMoves(validMoves, currentPos, Vector2Int.up);
            TryAddDirectionalMoves(validMoves, currentPos, Vector2Int.down);
            TryAddDirectionalMoves(validMoves, currentPos, Vector2Int.left);
            TryAddDirectionalMoves(validMoves, currentPos, Vector2Int.right);

            return validMoves;
        }

        private void TryAddDirectionalMoves(List<CellController> moves, Vector2Int startPos, Vector2Int direction)
        {
            Vector2Int nextPos = startPos + direction;

            while (true)
            {
                if (!TryAddMove(moves, nextPos, isCapture: false)) // Normal movement
                {
                    // If it's occupied, check for capture
                    TryAddMove(moves, nextPos, isCapture: true);
                    break; // Stop moving in this direction after first capture attempt
                }
                nextPos += direction; // Continue in the same direction
            }
        }

        protected override bool TryAddMove(List<CellController> moves, Vector2Int targetCoord, bool isCapture)
        {
            CellController targetCell = GridManager.instance.GetGridCellByCoordinates(targetCoord);
            if (targetCell == null) return false;

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
