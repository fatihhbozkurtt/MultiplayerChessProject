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

        public static PieceData GetPieceDataByType(PieceType type)
        {
            LoadPieceData();

            if (_pieceData == null || _pieceData.pieceDataList == null)
            {
                return null;
            }

            PieceData pieceData = _pieceData.pieceDataList.FirstOrDefault(p => p.PieceType == type);

            if (pieceData != null)
            {
                return pieceData;
            }

            Debug.LogError($"Defined {type} could not be found!");
            return null;
        }

        public static Team GetEnumByString(string teamString)
        {
            if (System.Enum.TryParse(teamString, true, out Team result))
            {
                return result;
            }
    
            Debug.LogError($"Invalid team string: {teamString}");
            return default; // Default to Team.Black if parsing fails
        }

    }
}