using System.Collections.Generic;
using Data;
using DG.Tweening;
using EssentialManagers.Packages.GridManager.Scripts;
using Net.NetMessage;
using UnityEngine;
using Client = Net.Client;

namespace Controllers
{
    public abstract class PieceController : MonoBehaviour
    {
        [Header("Debug")] public CellController CurrentCell;
        public Team Team;
        public PieceData pieceData;
        MeshRenderer _meshRenderer;

        public void Initialize(CellController cell, PieceData pieceData)
        {
            CurrentCell = cell;
            Team = cell.GetCoordinates().y < 2 ? Team.White : Team.Black;
            this.pieceData = pieceData;

            // setMaterial by team
            _meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            _meshRenderer.material = Team == Team.Black ? pieceData.blackMaterial : pieceData.whiteMaterial;
        }

        public void TryMoveSelf(CellController targetCell)
        {
            if (!CanMove(targetCell)) return;

            NetMakeMove nm = new NetMakeMove();
            nm.MoveData = new MoveData
            {
                OriginalCoord = CurrentCell.GetCoordinates(),
                TargetCoord = targetCell.GetCoordinates(),
                TeamEnum = Team
            };

            CurrentCell.SetFree();
            CurrentCell = targetCell;
            targetCell.SetOccupied(this);
            transform.DOMove(targetCell.transform.position, 0.5f).OnComplete(MoveComplete);

            Client.instance.SendToServer(nm);
        }


        public void MakeMove(CellController targetCell)
        {
            CurrentCell.SetFree();
            CurrentCell = targetCell;
            targetCell.SetOccupied(this);
            transform.DOMove(targetCell.transform.position, 0.5f);
        }

        public void HihglightValidCells()
        {
            for (int i = 0; i < GetValidMoves().Count; i++)
            {
                var validMoveCell = GetValidMoves()[i];
                validMoveCell.GetHighlighted();
            }
        }
        
        
        // Helpers
        
        public int GetDirectionSign()
        {
            return Team == Team.Black ? -1 : 1;
        }

        private bool CanMove(CellController cell)
        {
            return GetValidMoves().Contains(cell);
        }
        protected abstract List<CellController> GetValidMoves();
        protected abstract bool TryAddMove(List<CellController> moves, Vector2Int targetCoord, bool isCapture);
        protected abstract void MoveComplete();
    }
}
