using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CreateData", menuName = "Chess/Piece")]
public class PieceSo : ScriptableObject
{
    public List<PieceData> pieceDataList;
}

[System.Serializable]
public class PieceData
{
    public PieceType PieceType;
    public GameObject PiecePrefab;
    public Material blackMaterial;
    public Material whiteMaterial;
}