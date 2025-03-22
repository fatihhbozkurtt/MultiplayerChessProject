using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CreateData", menuName = "Chess/Piece")]
public class PieceSo : ScriptableObject
{
    public List<PieceAttributes> pieceDataList;
}

[System.Serializable]
public class PieceAttributes
{
    public PieceType PieceType;
    public Material blackMaterial;
    public Material whiteMaterial;
}