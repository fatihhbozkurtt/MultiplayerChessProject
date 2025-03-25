using System.Collections.Generic;
using EssentialManagers.Packages.GridManager.Scripts;
using UnityEngine;

namespace Controllers.SpecificPieces
{
    public class King : PieceController
    {
        

        protected override List<CellController> GetValidMoves()
        {
            throw new System.NotImplementedException();
        }

        protected override bool TryAddMove(List<CellController> moves, Vector2Int targetCoord, bool isCapture)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnMoveComplete()
        {
            throw new System.NotImplementedException();
        }
    }
}