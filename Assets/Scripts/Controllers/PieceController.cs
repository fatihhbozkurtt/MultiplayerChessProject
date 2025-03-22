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
        public PieceAttributes PieceAttributes;
        MeshRenderer _meshRenderer;
        public void Initialize( CellController cell, PieceAttributes pieceAttributes)
        {
            CurrentCell = cell;
            team = cell.GetCoordinates().y < 2 ? Team.White : Team.Black;
            PieceAttributes = pieceAttributes;
            
            // setMaterial by team
            _meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            _meshRenderer.material = team == Team.Black ? 
                pieceAttributes.blackMaterial : pieceAttributes.whiteMaterial;

        }
    }
}