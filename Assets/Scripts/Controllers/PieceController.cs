using Data;
using EssentialManagers.Packages.GridManager.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controllers
{
    public class PieceController : MonoBehaviour
    {
        [Header("Debug")] public CellController CurrentCell;
          public Team team;
        [FormerlySerializedAs("PieceAttributes")] public PieceData pieceData;
        MeshRenderer _meshRenderer;
        public void Initialize( CellController cell, PieceData pieceData)
        {
            CurrentCell = cell;
            team = cell.GetCoordinates().y < 2 ? Team.White : Team.Black;
            this.pieceData = pieceData;
            
            // setMaterial by team
            _meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            _meshRenderer.material = team == Team.Black ? 
                pieceData.blackMaterial : pieceData.whiteMaterial;

        }
    }
}