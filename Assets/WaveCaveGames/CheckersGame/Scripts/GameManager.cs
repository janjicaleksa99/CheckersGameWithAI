using UnityEngine;
using UnityEngine.UI;
using WaveCaveGames.CheckersGame;
using WaveCaveGames.Utilities;

namespace WaveCaveGames.CheckersGame{
	
	public class GameManager : MonoBehaviour
	{
		public float checkSize;
		public CheckTrigger checkTrigger;
		public Transform checkTriggerParent;
		public Transform pieceParent;
		public GameObject whitePiecePrefab;
		public GameObject blackPiecePrefab;
		public Mesh kingPieceMesh;
		public Transform pieceSelectMark;
		public Transform[] pieceMoveMark;
		[Header("UI Elements")]
		public GameObject winGameWindow;
		public Text winnerText;
		[HideInInspector] public CheckTrigger[] checkTriggers;
		[HideInInspector] public PieceManager clickedPiece;
		[HideInInspector] public bool isBlackTurn;
		[HideInInspector] public PieceManager[] whitePieces;
		[HideInInspector] public PieceManager[] blackPieces;
		public const float sqrt2 = 1.414214f;

		void Start(){
			checkTriggers = new CheckTrigger[0];
			whitePieces = new PieceManager[0];
			blackPieces = new PieceManager[0];
			float startPos = -checkSize * 4.5f;
			for (int i = 0; i < 10; i++) {
				float posX = startPos + checkSize * (float)i;
				for (int j = 0; j < 5; j++) {
					float posZ = startPos + checkSize * (float)j * 2f + ((i / 2 * 2 == i) ? checkSize : 0f);
					GameObject triggerObj = Instantiate(checkTrigger.gameObject, new Vector3(posX, checkTriggerParent.position.y, posZ), Quaternion.identity, checkTriggerParent);
					triggerObj.SetActive(true);
					ArrayUtility.IncreaseArray(ref checkTriggers, triggerObj.GetComponent<CheckTrigger>());
				}
			}
			//set top place
			for (int i = 0; i < 5; i++) {
				checkTriggers[i].topPlace = true;
				checkTriggers[i + 45].topPlace = true;
			}
			//create pieces
			GameObject pieceObj = null;
			for (int i = 0; i < 20; i++) {
				pieceObj = Instantiate(blackPiecePrefab, checkTriggers[i].transform.position, Quaternion.identity, pieceParent);
				checkTriggers[i].piece = pieceObj.GetComponent<PieceManager>();
				ArrayUtility.IncreaseArray(ref blackPieces, pieceObj.GetComponent<PieceManager>());
			}
			for (int i = 30; i < 50; i++) {
				pieceObj = Instantiate(whitePiecePrefab, checkTriggers[i].transform.position, Quaternion.identity, pieceParent);
				checkTriggers[i].piece = pieceObj.GetComponent<PieceManager>();
				ArrayUtility.IncreaseArray(ref whitePieces, pieceObj.GetComponent<PieceManager>());
			}
			//disable black piece trigger
			for (int i = 0; i < blackPieces.Length; i++) {
				if (blackPieces[i] != null) blackPieces[i].GetComponent<Collider>().enabled = false;
			}
		}
		public CheckTrigger FindCheck(Vector3 v){
			for (int i = 0; i < checkTriggers.Length; i++) {
				if ((v - checkTriggers[i].transform.position).sqrMagnitude < 0.001f) return checkTriggers[i];
			}
			return null;
		}
		public void FindCheckAndLightUp(Vector3 v, bool eatPiece){
			var check = FindCheck(v);
			if (check != null) {
				bool blocked = false;
				bool eatPieceBlocked = false;
				bool canGetToTarget = false;
				if (clickedPiece.pieceType == PieceType.King) {
					int distance = int.Parse((Vector3.Distance(clickedPiece.transform.position, v) / checkSize / sqrt2).ToString("0"));
					for (int i = 1; i < distance; i++) {
						if (blocked) eatPieceBlocked = true;
						CheckTrigger ct = FindCheck(Vector3.Lerp(clickedPiece.transform.position, v, (float)i / (float)distance));
						if (!blocked && ct != null && ct.piece != null) blocked = true;
					}
				}
				if (eatPiece) {
					Vector3 eatPieceVector = Vector3.Lerp(v, clickedPiece.transform.position, 1f / Vector3.Distance(clickedPiece.transform.position, v) * checkSize * sqrt2);
					if (!eatPieceBlocked && FindCheck(eatPieceVector).piece != null && FindCheck(eatPieceVector).piece.color == ((isBlackTurn) ? PieceColor.White : PieceColor.Black) && check.piece == null) {
						canGetToTarget = true;
					}
				} else {
					canGetToTarget = !blocked && check.piece == null;
				}
				if (canGetToTarget) {
					check.GetComponent<Collider>().enabled = true;
					check.GetComponent<Renderer>().enabled = true;
				}
			}
		}
		public CheckTrigger FindAndReturnCheckIfCanGetToTarget(PieceManager p, Vector3 v, bool eatPiece){
			var check = FindCheck(v);
			if (check != null) {
				bool blocked = false;
				bool eatPieceBlocked = false;
				bool canGetToTarget = false;
				if (p.pieceType == PieceType.King) {
					int distance = int.Parse((Vector3.Distance(p.transform.position, v) / checkSize / sqrt2).ToString("0"));
					for (int i = 1; i < distance; i++) {
						if (blocked) eatPieceBlocked = true;
						CheckTrigger ct = FindCheck(Vector3.Lerp(p.transform.position, v, (float)i / (float)distance));
						if (!blocked && ct != null && ct.piece != null) blocked = true;
					}
				}
				if (eatPiece) {
					Vector3 eatPieceVector = Vector3.Lerp(v, p.transform.position, 1f / Vector3.Distance(p.transform.position, v) * checkSize * sqrt2);
					if (!eatPieceBlocked && FindCheck(eatPieceVector).piece != null && FindCheck(eatPieceVector).piece.color == ((isBlackTurn) ? PieceColor.White : PieceColor.Black) && check.piece == null) {
						canGetToTarget = true;
					}
				} else {
					canGetToTarget = !blocked && check.piece == null;
				}
				if (canGetToTarget) return check;
				else return null;
			}
			return null;
		}
		public void ClickPiece(PieceManager p){
			for (int i = 0; i < checkTriggers.Length; i++) {
				checkTriggers[i].GetComponent<Collider>().enabled = false;
				checkTriggers[i].GetComponent<Renderer>().enabled = false;
			}
			clickedPiece = p;
			pieceSelectMark.GetComponent<Renderer>().enabled = true;
			pieceSelectMark.position = p.transform.position;
			switch (p.pieceType) {
			case PieceType.Normal:
				Vector3 vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize), p.transform.position.y, p.transform.position.z + checkSize);
				if (FindCheck(vector) != null)
					FindCheckAndLightUp(vector, false);
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize), p.transform.position.y, p.transform.position.z - checkSize);
				if (FindCheck(vector) != null)
					FindCheckAndLightUp(vector, false);
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize) * 2f, p.transform.position.y, p.transform.position.z + checkSize * 2f);
				if (FindCheck(vector) != null)
					FindCheckAndLightUp(vector, true);
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize) * 2f, p.transform.position.y, p.transform.position.z - checkSize * 2f);
				if (FindCheck(vector) != null)
					FindCheckAndLightUp(vector, true);
				break;
			case PieceType.King:
				for (int i = 1; i < 10; i++) {
					Vector3 dir1Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					FindCheckAndLightUp(dir1Vector, false);
					if (i != 1) FindCheckAndLightUp(dir1Vector, true);
					dir1Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					FindCheckAndLightUp(dir1Vector, false);
					if (i != 1) FindCheckAndLightUp(dir1Vector, true);
					dir1Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					FindCheckAndLightUp(dir1Vector, false);
					if (i != 1) FindCheckAndLightUp(dir1Vector, true);
					dir1Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					FindCheckAndLightUp(dir1Vector, false);
					if (i != 1) FindCheckAndLightUp(dir1Vector, true);
				}
				break;
			}
		}
		public void ClickPieceAgain(PieceManager p){
			for (int i = 0; i < checkTriggers.Length; i++) {
				checkTriggers[i].GetComponent<Collider>().enabled = false;
				checkTriggers[i].GetComponent<Renderer>().enabled = false;
			}
			clickedPiece = p;
			pieceSelectMark.GetComponent<Renderer>().enabled = true;
			pieceSelectMark.position = p.transform.position;
			switch (p.pieceType) {
			case PieceType.Normal:
				Vector3 vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize) * 2f, p.transform.position.y, p.transform.position.z + checkSize * 2f);
				if (FindCheck(vector) != null)
					FindCheckAndLightUp(vector, true);
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize) * 2f, p.transform.position.y, p.transform.position.z - checkSize * 2f);
				if (FindCheck(vector) != null)
					FindCheckAndLightUp(vector, true);
				break;
			case PieceType.King:
				for (int i = 2; i < 10; i++) {
					Vector3 dir1Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					FindCheckAndLightUp(dir1Vector, true);
					dir1Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					FindCheckAndLightUp(dir1Vector, true);
					dir1Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					FindCheckAndLightUp(dir1Vector, true);
					dir1Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					FindCheckAndLightUp(dir1Vector, true);
				}
				break;
			}
		}
		public CheckTrigger[] PossibleTargets(PieceManager p){
			CheckTrigger[] ct = new CheckTrigger[0];
			switch (p.pieceType) {
			case PieceType.Normal:
				Vector3 vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize), p.transform.position.y, p.transform.position.z + checkSize);
				if (FindCheck(vector) != null)
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, vector, false));
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize), p.transform.position.y, p.transform.position.z - checkSize);
				if (FindCheck(vector) != null)
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, vector, false));
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize) * 2f, p.transform.position.y, p.transform.position.z + checkSize * 2f);
				if (FindCheck(vector) != null)
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, vector, true));
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize) * 2f, p.transform.position.y, p.transform.position.z - checkSize * 2f);
				if (FindCheck(vector) != null)
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, vector, true));
				break;
			case PieceType.King:
				for (int i = 1; i < 10; i++) {
					Vector3 dir1Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, false));
					if (i != 1) ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, true));
					dir1Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, false));
					if (i != 1) ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, true));
					dir1Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, false));
					if (i != 1) ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, true));
					dir1Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, false));
					if (i != 1) ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, true));
				}
				break;
			}
			return ct;
		}
		public CheckTrigger[] PossibleEatingTargets(PieceManager p){
			CheckTrigger[] ct = new CheckTrigger[0];
			switch (p.pieceType) {
			case PieceType.Normal:
				Vector3 vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize) * 2f, p.transform.position.y, p.transform.position.z + checkSize * 2f);
				if (FindCheck(vector) != null)
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, vector, true));
				vector = new Vector3(p.transform.position.x + ((p.color == PieceColor.Black) ? checkSize : -checkSize) * 2f, p.transform.position.y, p.transform.position.z - checkSize * 2f);
				if (FindCheck(vector) != null)
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, vector, true));
				break;
			case PieceType.King:
				for (int i = 2; i < 10; i++) {
					Vector3 dir1Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, true));
					dir1Vector = new Vector3(p.transform.position.x + checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, true));
					dir1Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z + checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, true));
					dir1Vector = new Vector3(p.transform.position.x - checkSize * (float)i, p.transform.position.y, p.transform.position.z - checkSize * (float)i);
					ArrayUtility.IncreaseArrayAvoidNull(ref ct, FindAndReturnCheckIfCanGetToTarget(p, dir1Vector, true));
				}
				break;
			}
			return ct;
		}
		public void CheckIfCanEatPieceAgain(PieceManager p){
			//disable mark and trigger
			pieceSelectMark.GetComponent<Renderer>().enabled = false;
			for (int i = 0; i < checkTriggers.Length; i++) {
				checkTriggers[i].GetComponent<Collider>().enabled = false;
				checkTriggers[i].GetComponent<Renderer>().enabled = false;
			}
			//disable all pieces trigger
			for (int i = 0; i < whitePieces.Length; i++) {
				if (whitePieces[i] != null) whitePieces[i].GetComponent<Collider>().enabled = false;
			}
			for (int i = 0; i < blackPieces.Length; i++) {
				if (blackPieces[i] != null) blackPieces[i].GetComponent<Collider>().enabled = false;
			}
			ClickPieceAgain(p);
			CheckTrigger[] activeTriggers = FindActiveTriggers();
			if (activeTriggers.Length == 0) ChangePlayerTurn();
		}
		public void ChangePlayerTurn(){
			pieceSelectMark.GetComponent<Renderer>().enabled = false;
			for (int i = 0; i < checkTriggers.Length; i++) {
				checkTriggers[i].GetComponent<Collider>().enabled = false;
				checkTriggers[i].GetComponent<Renderer>().enabled = false;
			}
			isBlackTurn = !isBlackTurn;
			for (int i = 0; i < whitePieces.Length; i++) {
				if (whitePieces[i] != null) whitePieces[i].GetComponent<Collider>().enabled = !isBlackTurn;
			}
			for (int i = 0; i < blackPieces.Length; i++) {
				if (blackPieces[i] != null) blackPieces[i].GetComponent<Collider>().enabled = isBlackTurn;
			}
		}
		public void CheckIfWin(){
			if (whitePieces.Length == 0) {
				winGameWindow.SetActive(true);
				winnerText.text = "Black wins!";
			}
			if (blackPieces.Length == 0) {
				winGameWindow.SetActive(true);
				winnerText.text = "White wins!";
			}
		}
		public void BackToMenu(){
			UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
		}
		public CheckTrigger[] FindActiveTriggers(){
			CheckTrigger[] ct = new CheckTrigger[0];
			for (int i = 0; i < checkTriggers.Length; i++) {
				if (checkTriggers[i].GetComponent<Collider>().enabled == true)
					ArrayUtility.IncreaseArray(ref ct, checkTriggers[i]);
			}
			return ct;
		}
	}
}
