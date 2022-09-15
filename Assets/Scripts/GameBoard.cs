using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PlayerType {
    WhitePlayer, BlackPlayer, None
}

public struct Position {
    public int i;
    public int j;

    public Position(int i, int j) {
        this.i = i;
        this.j = j;
    }
}

public class GameBoard : MonoBehaviour
{
    private Piece[,] board = new Piece[8, 8];
    private PlayerType playerOnTurn = PlayerType.WhitePlayer; // White player plays first
    private int blackPieces = 0;
    private int whitePieces = 0;
    private int blackKings = 0;
    private int whiteKings = 0;
    private AIAlgorithms AIAlgorithm;
    private GameObject AISelector;
    private Move aiMove;

    public GameBoard() {

    }

    public GameBoard(GameBoard gameBoard) {
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++) {
                if (gameBoard.board[i, j] != null) {
                    if (gameBoard.board[i, j].getPieceType() == PieceType.Normal) {
                        if (gameBoard.board[i, j].getPieceColor() == PieceColor.Black) {
                            board[i, j] = new BlackRegularPiece((BlackRegularPiece)gameBoard.board[i, j]);
                        }
                        else {
                            board[i, j] = new WhiteRegularPiece((WhiteRegularPiece)gameBoard.board[i, j]);
                        }
                    }
                    else if (gameBoard.board[i, j].getPieceType() == PieceType.King) {
                        board[i, j] = new KingPiece((KingPiece)gameBoard.board[i, j]);
                    }
                }
            }
        playerOnTurn = gameBoard.playerOnTurn;
        blackPieces = gameBoard.blackPieces;
        blackKings = gameBoard.blackKings;
        whitePieces = gameBoard.whitePieces;
        whiteKings = gameBoard.whiteKings;
    }

    public Position calculatePosition(Piece piece) {
        Vector3 position = piece.getLocalPosition();
        float f = position.x * 1000f;
        int i = Mathf.RoundToInt(f);
        f = position.z * 1000f;
        int j = Mathf.RoundToInt(f);

        i = (900 - i) / (25 * 10);
        j = (900 - j) / (25 * 10);

        return new Position(i, j);
    }

    public Position calculatePosition(Vector3 position) {
        float f = position.x * 1000f;
        int i = Mathf.RoundToInt(f);
        f = position.z * 1000f;
        int j = Mathf.RoundToInt(f);

        i = (900 - i) / (25 * 10);
        j = (900 - j) / (25 * 10);

        return new Position(i, j);
    }

    public PlayerType haveAWinner() {
        if (whitePieces == 0 && whiteKings == 0)
            return PlayerType.BlackPlayer;
        else if (blackPieces == 0 && blackKings == 0)
            return PlayerType.WhitePlayer;

        PlayerType winner = PlayerType.None;
        int whitePiecesWithNoMove = 0;
        int blackPiecesWithNoMove = 0;
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++) {
                if (board[i, j] != null) {
                    if (((int)board[i, j].getPieceColor()) == ((int)playerOnTurn)) {
                        Vector3 pos = board[i, j].getLocalPosition();
                        PieceColor col = board[i, j].getPieceColor();
                        if (!board[i, j].canPieceMove(pos, col, this)) {
                            if (playerOnTurn == PlayerType.WhitePlayer)
                                whitePiecesWithNoMove++;
                            else
                                blackPiecesWithNoMove++;
                        }
                    }
                }
            }
        if (playerOnTurn == PlayerType.WhitePlayer) {
            if (whitePiecesWithNoMove == (whitePieces + whiteKings))
                winner = PlayerType.BlackPlayer;
        }
        else {
            if (blackPiecesWithNoMove == (blackPieces + blackKings))
                winner = PlayerType.WhitePlayer;
        }
        return winner;
    }

    public void nextTurn() {
        if (playerOnTurn == PlayerType.WhitePlayer)
            playerOnTurn = PlayerType.BlackPlayer;
        else
            playerOnTurn = PlayerType.WhitePlayer;
    }

    public PlayerType getplayerOnTurn() {
        return playerOnTurn;
    }

    public void addPiece(Piece piece) {
        Position pos = calculatePosition(piece);
        int i = pos.i;
        int j = pos.j;

        board[i, j] = piece;

        if (piece.getPieceColor() == PieceColor.White && piece.getPieceType() == PieceType.Normal)
            whitePieces++;
        else if (piece.getPieceColor() == PieceColor.White && piece.getPieceType() == PieceType.King)
            whiteKings++;
        else if (piece.getPieceColor() == PieceColor.Black && piece.getPieceType() == PieceType.Normal)
            blackPieces++;
        else
            blackKings++;
    }

    public void removePiece(Piece piece) {
        Position pos = calculatePosition(piece);
        int i = pos.i;
        int j = pos.j;

        board[i, j] = null;

        if (piece.getPieceColor() == PieceColor.White && piece.getPieceType() == PieceType.Normal)
            whitePieces--;
        else if (piece.getPieceColor() == PieceColor.White && piece.getPieceType() == PieceType.King)
            whiteKings--;
        else if (piece.getPieceColor() == PieceColor.Black && piece.getPieceType() == PieceType.Normal)
            blackPieces--;
        else
            blackKings--;
    }

    public void movePiece(Vector3 oldPosition, Piece piece) {
        Position pos = calculatePosition(oldPosition);
        int i = pos.i;
        int j = pos.j;

        board[i, j] = null;

        // get new position of the piece
        pos = calculatePosition(piece);
        i = pos.i;
        j = pos.j;

        board[i, j] = piece;
    }

    public bool upperLeftMove(Vector3 position, PieceColor pieceColor, bool aiTest = false) {
        if (((int)playerOnTurn) == ((int)pieceColor) || aiTest) {
            Position pos = calculatePosition(position);
            int i = pos.i;
            int j = pos.j;

            if (i - 1 < 0 || j - 1 < 0)
                return false;
            if (board[i - 1, j - 1] != null)
                return false;
            return true;
        }
        return false;
    }

    public bool upperRightMove(Vector3 position, PieceColor pieceColor, bool aiTest = false) {
        if (((int)playerOnTurn) == ((int)pieceColor) || aiTest) {
            Position pos = calculatePosition(position);
            int i = pos.i;
            int j = pos.j;

            if (i - 1 < 0 || j + 1 >= 8)
                return false;
            if (board[i - 1, j + 1] != null)
                return false;
            return true;
        }
        return false;
    }

    public bool bottomLeftMove(Vector3 position, PieceColor pieceColor, bool aiTest = false) {
        if (((int)playerOnTurn) == ((int)pieceColor) || aiTest) {
            Position pos = calculatePosition(position);
            int i = pos.i;
            int j = pos.j;

            if (i + 1 >= 8 || j - 1 < 0)
                return false;
            if (board[i + 1, j - 1] != null)
                return false;
            return true;
        }
        return false;
    }

    public bool bottomRightMove(Vector3 position, PieceColor pieceColor, bool aiTest = false) {
        if (((int)playerOnTurn) == ((int)pieceColor) || aiTest) {
            Position pos = calculatePosition(position);
            int i = pos.i;
            int j = pos.j;

            if (i + 1 >= 8 || j + 1 >= 8)
                return false;
            if (board[i + 1, j + 1] != null)
                return false;
            return true;
        }
        return false;
    }

    public bool upperLeftEat(Vector3 position, PieceColor pieceColor, bool aiTest = false) {
        if (((int)playerOnTurn) == ((int)pieceColor) || aiTest) {
            Position pos = calculatePosition(position);
            int i = pos.i;
            int j = pos.j;

            if (i - 1 < 0 || j - 1 < 0)
                return false;

            if (board[i - 1, j - 1] == null)
                return false;

            if (i - 2 < 0 || j - 2 < 0)
                return false;

            if (board[i - 2, j - 2] != null) // Check if UpperLeft2 is not empty field
                return false;
            else {
                if (((int)board[i - 1, j - 1].getPieceColor()) == ((int)playerOnTurn))
                    return false; // White Piece == White Player
            }
            SelectorManager.pieceToBeEatenUL = board[i - 1, j - 1].getPieceManager();
            return true;
        }
        return false;
    }

    public bool upperRightEat(Vector3 position, PieceColor pieceColor, bool aiTest = false) {
        if (((int)playerOnTurn) == ((int)pieceColor) || aiTest) {
            Position pos = calculatePosition(position);
            int i = pos.i;
            int j = pos.j;

            if (i - 1 < 0 || j + 1 >= 8)
                return false;

            if (board[i - 1, j + 1] == null)
                return false;

            if (i - 2 < 0 || j + 2 >= 8)
                return false;

            if (board[i - 2, j + 2] != null) // Check if UpperLeft2 is not empty field
                return false;
            else {
                if (((int)board[i - 1, j + 1].getPieceColor()) == ((int)playerOnTurn))
                    return false; // White Piece == White Player
            }
            SelectorManager.pieceToBeEatenUR = board[i - 1, j + 1].getPieceManager();
            return true;
        }
        return false;
    }

    public bool bottomLeftEat(Vector3 position, PieceColor pieceColor, bool aiTest = false) {
        if (((int)playerOnTurn) == ((int)pieceColor) || aiTest) {
            Position pos = calculatePosition(position);
            int i = pos.i;
            int j = pos.j;

            if (i + 1 >= 8 || j - 1 < 0)
                return false;

            if (board[i + 1, j - 1] == null)
                return false;

            if (i + 2 >= 8 || j - 2 < 0)
                return false;

            if (board[i + 2, j - 2] != null) // Check if UpperLeft2 is not empty field
                return false;
            else {
                if (((int)board[i + 1, j - 1].getPieceColor()) == ((int)playerOnTurn))
                    return false; // White Piece == White Player
            }
            SelectorManager.pieceToBeEatenBL = board[i + 1, j - 1].getPieceManager();
            return true;
        }
        return false;
    }

    public bool bottomRightEat(Vector3 position, PieceColor pieceColor, bool aiTest = false) {
        if (((int)playerOnTurn) == ((int)pieceColor) || aiTest) {
            Position pos = calculatePosition(position);
            int i = pos.i;
            int j = pos.j;

            if (i + 1 >= 8 || j + 1 >= 8)
                return false;

            if (board[i + 1, j + 1] == null)
                return false;

            if (i + 2 >= 8 || j + 2 >= 8)
                return false;

            if (board[i + 2, j + 2] != null) // Check if UpperLeft2 is not empty field
                return false;
            else {
                if (((int)board[i + 1, j + 1].getPieceColor()) == ((int)playerOnTurn))
                    return false; // White Piece == White Player
            }
            SelectorManager.pieceToBeEatenBR = board[i + 1, j + 1].getPieceManager();
            return true;
        }
        return false;
    }

    public bool becomeKing(Piece piece) {
        Vector3 position = piece.getLocalPosition();
        float f = position.x * 1000f;
        int i = Mathf.RoundToInt(f);

        i = (900 - i) / (25 * 10);

        bool king = false;
        if (piece.getPieceColor() == PieceColor.White) {
            if (i == 0)
                king = true;
        }
        else {
            if (i == 7)
                king = true;
        }
        return king;
    }

    public void transformPieceToKing(Piece piece) {
        if (piece.getPieceColor() == PieceColor.White) {
            whitePieces--;
            whiteKings++;
        }
        else {
            blackPieces--;
            blackKings++;
        }
    }

    public float evaluateBoard() {
        float cumulativeDistance = 0f;
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++) {
                if (board[i, j] != null) {
                    Vector3 pos = board[i, j].getLocalPosition();
                    PieceColor col = board[i, j].getPieceColor();
                    if (board[i, j].canPieceMove(pos, col, this)) {
                        cumulativeDistance += i;
                    }
                }
            }
        return blackPieces - whitePieces + (blackKings * 2f - whiteKings * 2f) - cumulativeDistance;
    }

    public ArrayList getAllPieces(PieceColor color) {
        ArrayList pieces = new ArrayList();
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++) {
                if (board[i, j] != null) {
                    if (board[i, j].getPieceColor() == color)
                        pieces.Add(board[i, j]);
                }
            }
        return pieces;
    }

    public Piece[,] getBoard() {
        return board;
    }

    public void AIMove() {
        aiMove = AIAlgorithm.AIAlgorithm(this, AIAlgorithms.depthOfTree, MinMaxPlayers.MaxPlayer).move;


        /*---------------------------------------------------------------------------------*/
        TMP_Dropdown dropdown = GameObject.Find("/AIMoves/Dropdown").GetComponent<TMPro.TMP_Dropdown>();
        dropdown.ClearOptions();
        int i = 1;
        foreach (AIVals possibleMove in AIAlgorithms.possibleAIMoves) {
            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
            newOption.text = "Potez " + i++ + " procena: " + possibleMove.evaluation;
            dropdown.options.Add(newOption);
        }
        GameObject.Find("/ForPreviewingAIMove").GetComponent<PreviewAIMove>().OnValueChanged(0);
        /*---------------------------------------------------------------------------------*/
    }

    public void continueAIMove() {
        AISelector.SetActive(false);
        AIAlgorithms.possibleAIMoves.Clear();
        GameObject.Find("/ForPreviewingAIMove").GetComponent<PreviewAIMove>().turnSpotlightOff();

        GameBoard newBoard = aiMove.position;

        int iOld = aiMove.oldPosition.i;
        int jOld = aiMove.oldPosition.j;
        Animator animator = board[iOld, jOld].getPieceManager()
            .GetComponentInParent<Animator>();
        int iNew = aiMove.newPosition.i;
        int jNew = aiMove.newPosition.j;

        int iEaten = aiMove.eatenPiece.i;
        int jEaten = aiMove.eatenPiece.j;

        int iDiff = iNew - iOld;
        int jDiff = jNew - jOld;

        if (iEaten != -1 && jEaten != -1) {
            board[iEaten, jEaten].getPieceManager().gameObject.SetActive(false);
            removePiece(board[iEaten, jEaten]);
        }


        Vector3 newPosition = board[iOld, jOld].getPieceManager().transform.localPosition;
        if (iDiff == -1 && jDiff == -1) { // UpperLeftMove1
            animator.runtimeAnimatorController = AnimatorController.UpperLeftMove1;
            newPosition.x += 0.25f;
            newPosition.z += 0.25f;
        }
        else if (iDiff == -2 && jDiff == -2) { // UpperLeftMove2
            animator.runtimeAnimatorController = AnimatorController.UpperLeftMove2;
            PieceManager.alreadyAte = true;
            newPosition.x += 0.5f;
            newPosition.z += 0.5f;
        }
        else if (iDiff == -1 && jDiff == 1) { // UpperRightMove1
            animator.runtimeAnimatorController = AnimatorController.UpperRightMove1;
            newPosition.x += 0.25f;
            newPosition.z -= 0.25f;
        }
        else if (iDiff == -2 && jDiff == 2) { // UpperRightMove2
            animator.runtimeAnimatorController = AnimatorController.UpperRightMove2;
            PieceManager.alreadyAte = true;
            newPosition.x += 0.5f;
            newPosition.z -= 0.5f;
        }
        else if (iDiff == 1 && jDiff == -1) { // BottomLeftMove1
            animator.runtimeAnimatorController = AnimatorController.BottomLeftMove1;
            newPosition.x -= 0.25f;
            newPosition.z += 0.25f;
        }
        else if (iDiff == 2 && jDiff == -2) { // BottomLeftMove2
            animator.runtimeAnimatorController = AnimatorController.BottomLeftMove2;
            PieceManager.alreadyAte = true;
            newPosition.x -= 0.5f;
            newPosition.z += 0.5f;
        }
        else if (iDiff == 1 && jDiff == 1) { // BottomRightMove1
            animator.runtimeAnimatorController = AnimatorController.BottomRightMove1;
            newPosition.x -= 0.25f;
            newPosition.z -= 0.25f;
        }
        else if (iDiff == 2 && jDiff == 2) { // BottomRightMove2
            animator.runtimeAnimatorController = AnimatorController.BottomRightMove2;
            PieceManager.alreadyAte = true;
            newPosition.x -= 0.5f;
            newPosition.z -= 0.5f;
        }

        AnimatorController.currentAnimatiorController = animator.runtimeAnimatorController;

        SelectorManager.pieceOnTheMove = board[iOld, jOld].getPieceManager();
        board[iOld, jOld].getPieceManager().setNewPosition(newPosition);
        board[iOld, jOld].setLocalPosition(newPosition);
        board[iNew, jNew] = board[iOld, jOld];
        board[iOld, jOld] = null;

        this.blackKings = newBoard.blackKings;
        this.blackPieces = newBoard.blackPieces;
        this.whiteKings = newBoard.whiteKings;
        this.whitePieces = newBoard.whitePieces;
    }

    public Piece getPiece(int i, int j) {
        if (i < 0 || i >= 8 || j < 0 || j >= 8)
            return null;
        return board[i, j];
    }

    public int getBlackPieces() {
        return blackPieces;
    }

    public int getWhitePieces() {
        return whitePieces;
    }

    public int getBlackKings() {
        return blackKings;
    }

    public int getWhiteKings() {
        return whiteKings;
    }

    private void Awake() {
        AIAlgorithm = AIAlgorithms.currentAIAlgorithm;
        AISelector = GameObject.Find("/CheckerBoard1/CheckerBoard1/AISelector");
    }

    public void RestartGame() {
        board = new Piece[8, 8];
        playerOnTurn = PlayerType.WhitePlayer;
        whiteKings = whitePieces = 0;
        blackKings = blackPieces = 0;
        AIAlgorithms.currentAIAlgorithm = null;
        AIAlgorithms.depthOfTree = -1;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
