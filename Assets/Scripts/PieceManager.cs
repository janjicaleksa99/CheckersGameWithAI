using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public enum PieceType {
	Normal, King
}
public enum PieceColor {
	White, Black
}


public class PieceManager : MonoBehaviour, IPointerDownHandler {
	private Piece piece;
	private GameBoard gameBoard;
	public PieceType pieceType;
	public PieceColor color;
    private GameObject crown;
    public static bool alreadyAte = false;
    public static Piece pieceThatJumpsAgain = null;
    public static bool HumanOpMode = false;
    private Vector3 newPosition;
    private static GameObject gameOverDialog;
    private static bool setGameOverDialog = false;
    private static GameObject AIMoves;
    private static bool setAIMovesCanvas = false;

    public static void resetBools() {
        setAIMovesCanvas = false;
        setGameOverDialog = false;
        HumanOpMode = false;
    }

    private void Awake() {
        if (!setGameOverDialog) {
            gameOverDialog = GameObject.Find("/GameOverDialog");
            gameOverDialog.SetActive(false);
            setGameOverDialog = true;
        }
        switch (color) {
            case PieceColor.White:
                piece = new WhiteRegularPiece(color, transform.localPosition, this);
                break;
            case PieceColor.Black:
                piece = new BlackRegularPiece(color, transform.localPosition, this);
                break;
        }
		gameBoard = GameObject.Find("/CheckerBoard1/CheckerBoard1").GetComponent<GameBoard>();
		gameBoard.addPiece(piece);
        crown = gameObject.transform.GetChild(0).gameObject;

        if (!setAIMovesCanvas) {
            AIMoves = GameObject.Find("/AIMoves");
            AIMoves.SetActive(false);
            setAIMovesCanvas = true;
        }
    }

    public void setNewPosition(Vector3 position) {
        newPosition = position;
    }

    public void EndOfMove() {
        transform.localPosition = newPosition;

        if (!(piece.canYouEat(gameBoard) && alreadyAte)) {
            gameBoard.nextTurn();
            pieceThatJumpsAgain = null;
        } 
        else
            pieceThatJumpsAgain = piece;
        alreadyAte = false;

        // Check if the piece has come to the end and needs to become a KING
        if (pieceType == PieceType.Normal) {
            if (gameBoard.becomeKing(piece)) {
                gameBoard.removePiece(piece);
                piece = new KingPiece(color, transform.localPosition, this);
                gameBoard.addPiece(piece);
                gameBoard.transformPieceToKing(piece);
                this.pieceType = PieceType.King;
                crown.SetActive(true);
            }
        }

        // Check if you have a winner
        PlayerType winner = gameBoard.haveAWinner();
        if (winner != PlayerType.None) {
            Debug.Log("End of Game!");
            GameObject g = GameObject.Find("/GameOverDialog");
            gameOverDialog.SetActive(true);
            GameObject winnerText = GameObject.Find("/GameOverDialog/Winner");
            if (winner == PlayerType.WhitePlayer)
                winnerText.GetComponent<TMPro.TextMeshProUGUI>().text= "Beli igrac je pobedio";
            else if (winner == PlayerType.BlackPlayer)
                winnerText.GetComponent<TMPro.TextMeshProUGUI>().text = "Crni igrac je pobedio";
            gameBoard.RestartGame();
            setGameOverDialog = false;
        }
        else {
            // AI MOVE
            if (gameBoard.getplayerOnTurn() == PlayerType.BlackPlayer && !HumanOpMode) {
                AIMoves.SetActive(true);
                gameBoard.AIMove();
            }
        }
    }

    public Piece getPiece() {
		return piece;
    }

    public GameBoard getGameBoard() {
        return gameBoard;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (AnimatorController.currentAnimatiorController != null) {
            return; // Animation is in progress so no piece can move at this time
        }

        if (pieceThatJumpsAgain != null) { // check if there is a piece that must jump again
            if (pieceThatJumpsAgain != piece) // check if this piece is the piece that has to jump
                return;
        }

        if (gameBoard.getplayerOnTurn() == PlayerType.WhitePlayer || HumanOpMode) {
            piece.highlightPossibleMoves(gameBoard);
            SelectorManager.pieceOnTheMove = this; // We set this static field so when the Selector is clicked on
        }                                          // it can know which piece is trying to move on the selector field
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
