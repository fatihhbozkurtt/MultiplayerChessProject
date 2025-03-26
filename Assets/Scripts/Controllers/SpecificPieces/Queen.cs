using System.Collections.Generic;
using EssentialManagers.Packages.GridManager.Scripts;
using UnityEngine;

namespace Controllers.SpecificPieces
{
    public class Queen : PieceController
    {
        protected override List<CellController> GetValidMoves()
        {
            List<CellController> validMoves = new List<CellController>();
            Vector2Int currentPos = CurrentCell.GetCoordinates();

            // Horizontal & Vertical moves (Rook-like)
            TryAddDirectionalMoves(validMoves, currentPos, Vector2Int.up);
            TryAddDirectionalMoves(validMoves, currentPos, Vector2Int.down);
            TryAddDirectionalMoves(validMoves, currentPos, Vector2Int.left);
            TryAddDirectionalMoves(validMoves, currentPos, Vector2Int.right);

            // Diagonal moves (Bishop-like)
            TryAddDirectionalMoves(validMoves, currentPos, new Vector2Int(1, 1));
            TryAddDirectionalMoves(validMoves, currentPos, new Vector2Int(-1, 1));
            TryAddDirectionalMoves(validMoves, currentPos, new Vector2Int(1, -1));
            TryAddDirectionalMoves(validMoves, currentPos, new Vector2Int(-1, -1));

            return validMoves;
        }

        private void TryAddDirectionalMoves(List<CellController> moves, Vector2Int startPos, Vector2Int direction)
        {
            Vector2Int nextPos = startPos + direction;

            while (true)
            {
                if (!TryAddMove(moves, nextPos, isCapture: false)) // Normal move
                {
                    if (TryAddMove(moves, nextPos, isCapture: true)) // Capture move
                    {
                        break; // Stop after capturing
                    }
                    break; // Stop moving in this direction
                }

                nextPos += direction; // Move further in the same direction
            }
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
