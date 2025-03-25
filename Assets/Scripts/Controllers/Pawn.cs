using System.Collections.Generic;
using EssentialManagers.Packages.GridManager.Scripts;
using UnityEngine;

namespace Controllers
{
    public class Pawn : PieceController
    {
        private bool _hasMoved;

        public override List<CellController> GetValidMoves()
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

        public override bool CanMove(CellController cell)
        {
             return GetValidMoves().Contains(cell);
        }

        private bool TryAddMove(List<CellController> moves, Vector2Int targetCoord, bool isCapture)
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

        public void MoveTo(CellController newCell)
        {
            // if (newCell == null) return;
            //
            // // Update old and new cell references
            // CurrentCell.pieceOnCell = null;
            // newCell.pieceOnCell = this;
            // transform.position = newCell.transform.position;
            // CurrentCell = newCell;

            // Mark as moved after first move
            _hasMoved = true;
        }
    }
}