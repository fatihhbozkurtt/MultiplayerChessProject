using System.Collections.Generic;
using EssentialManagers.Packages.GridManager.Scripts;
using UnityEngine;

namespace Controllers.SpecificPieces
{
    public class Pawn : PieceController
    {
        private bool _hasMoved;

        protected override List<CellController> GetValidMoves()
        {
            List<CellController> validMoves = new List<CellController>();
            Vector2Int currentPos = CurrentCell.GetCoordinates();
            int dirSign = GetDirectionSign();

            // Forward movement (1 square)
            if (TryAddMove(validMoves, new Vector2Int(currentPos.x, currentPos.y + dirSign), isCapture: false) &&
                !_hasMoved)
            {
                // If the first move is possible, allow double move
                TryAddMove(validMoves, new Vector2Int(currentPos.x, currentPos.y + 2 * dirSign), isCapture: false);
            }

            // Diagonal captures
            TryAddMove(validMoves, new Vector2Int(currentPos.x - 1, currentPos.y + dirSign), isCapture: true);
            TryAddMove(validMoves, new Vector2Int(currentPos.x + 1, currentPos.y + dirSign), isCapture: true);

            return validMoves;
        }

        protected override bool TryAddMove(List<CellController> moves, Vector2Int targetCoord, bool isCapture)
        {
            CellController targetCell = GridManager.instance.GetGridCellByCoordinates(targetCoord);
            if (targetCell == null) return false;

            if (!isCapture && targetCell.isOccupied == false) // Normal move
            {
                moves.Add(targetCell);
                return true;
            }
            else if (isCapture && targetCell.GetCurrentPiece() != null &&
                     targetCell.GetCurrentPiece().Team != Team) // Capture move
            {
                moves.Add(targetCell);
                return true;
            }

            return false;
        }

        protected override void MoveComplete()
        {
             _hasMoved = true;
        }
    }
}