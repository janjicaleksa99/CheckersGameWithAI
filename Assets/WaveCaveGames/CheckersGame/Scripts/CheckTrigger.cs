using UnityEngine;
using WaveCaveGames.CheckersGame;
using WaveCaveGames.Utilities;

namespace WaveCaveGames.CheckersGame{
	
	public class CheckTrigger : MonoBehaviour
	{
		[HideInInspector] public PieceManager piece;
		[HideInInspector] public bool topPlace;

		void OnMouseDown(){
			ClickThis();
		}
		public void ClickThis(){
			GameManager gm = FindObjectOfType<GameManager>();
			float f = 1f / Vector3.Distance(gm.clickedPiece.transform.position, transform.position) * gm.checkSize * Mathf.Sqrt(2f);
			bool dontEatPiece = int.Parse((f * 1000f).ToString("0")) == 1000;
			Vector3 eatPieceVector = Vector3.Lerp(transform.position, gm.clickedPiece.transform.position, f);
			if (!dontEatPiece && gm.FindCheck(eatPieceVector) != null && gm.FindCheck(eatPieceVector).piece != null) {
				if (gm.isBlackTurn) ArrayUtility.DecreaseArrayObject(ref gm.whitePieces, gm.FindCheck(eatPieceVector).piece);
				else ArrayUtility.DecreaseArrayObject(ref gm.blackPieces, gm.FindCheck(eatPieceVector).piece);
				DestroyImmediate(gm.FindCheck(eatPieceVector).piece.gameObject);
			}
			PieceManager targetPiece = gm.clickedPiece;
			gm.FindCheck(targetPiece.transform.position).piece = null;
			gm.pieceMoveMark[0].position = targetPiece.transform.position;
			gm.pieceMoveMark[1].position = transform.position;
			targetPiece.transform.position = transform.position;
			piece = targetPiece;
			if (topPlace) {
				piece.pieceType = PieceType.King;
				piece.GetComponent<MeshFilter>().sharedMesh = gm.kingPieceMesh;
			}
			gm.clickedPiece = null;
			if (dontEatPiece || PlayerPrefs.GetInt("CheckerPieceEating") == 1) {
				gm.ChangePlayerTurn();
			} else {
				gm.CheckIfCanEatPieceAgain(targetPiece);
			}
			gm.CheckIfWin();
		}
	}
}
