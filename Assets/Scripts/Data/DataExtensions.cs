using System.Linq;
using UnityEngine;

namespace Data
{
    public static class DataExtensions
    {
        private static PieceSo _pieceData;

        // PieceSo nesnesini sadece bir kez yükler ve bellekte tutar
        private static void LoadPieceData()
        {
            if (_pieceData == null)
            {
                _pieceData = Resources.Load<PieceSo>($"PieceDataList");

                if (_pieceData == null)
                {
                    Debug.LogError("PieceDataList not found!");
                }
            }
        }

        public static PieceAttributes GetPieceDataByType(PieceType type)
        {
            LoadPieceData();

            if (_pieceData == null || _pieceData.pieceDataList == null)
            {
                return null;
            }

            PieceAttributes pieceAttributes = _pieceData.pieceDataList.FirstOrDefault(p => p.PieceType == type);

            if (pieceAttributes != null)
            {
                return pieceAttributes;
            }

            Debug.LogError($"Defined {type} could not be found!");
            return null;
        }
    }
}