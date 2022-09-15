using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct V2 {
    public int i;
    public int j;

    public V2(int i, int j) {
        this.i = i;
        this.j = j;
    }

    public V2(int noEat) {
        this.i = this.j = noEat; 
    }
}

public struct Move {
    public GameBoard position;
    public V2 oldPosition;
    public V2 newPosition;
    public V2 eatenPiece;

    public Move(GameBoard position, V2 oldPosition, V2 newPosition, V2 eatenPiece) {
        this.position = position;
        this.oldPosition = oldPosition;
        this.newPosition = newPosition;
        this.eatenPiece = eatenPiece;
    }

    public Move(GameBoard position) : this() {
        this.position = position;
    }
}

public abstract class Piece {
    protected PieceType pieceType;
    protected PieceColor color;
    protected Vector3 localPosition;
    protected PieceManager pieceManager;

    public Piece(PieceColor color, Vector3 localPosition, PieceManager pieceManager) {
        this.color = color;
        this.localPosition = localPosition;
        this.pieceManager = pieceManager;
    }

    public Vector3 getLocalPosition() {
        return localPosition;
    }

    public PieceType getPieceType() {
        return pieceType;
    }

    public PieceColor getPieceColor() {
        return color;
    }

    public PieceManager getPieceManager() {
        return pieceManager;
    }

    public void setLocalPosition(Vector3 position) {
        localPosition = position;
    }

    public void turnOffSelectors() {
        GameObject selector1 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector1");
        GameObject selector2 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector2");
        GameObject selector3 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector3");
        GameObject selector4 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector4");

        selector1.SetActive(false);
        selector2.SetActive(false);
        selector3.SetActive(false);
        selector4.SetActive(false);
    }

    public abstract bool canPieceMove(Vector3 pos, PieceColor col, GameBoard gameBoard);

    public abstract void highlightPossibleMoves(GameBoard gameBoard);

    public abstract bool canYouEat(GameBoard gameBoard);

    public abstract ArrayList getAllValidMoves(GameBoard gameBoard);
}


public class WhiteRegularPiece : Piece {
    public WhiteRegularPiece(PieceColor color, Vector3 localPosition, PieceManager pieceManager) 
        : base(color, localPosition, pieceManager) {
        base.pieceType = PieceType.Normal;
    }

    public WhiteRegularPiece(WhiteRegularPiece piece)
        : base(piece.color, piece.localPosition, piece.pieceManager) {
        base.pieceType = PieceType.Normal;
    }

    public override bool canPieceMove(Vector3 pos, PieceColor col, GameBoard gameBoard) {
        bool pieceMove = true;
        if (!gameBoard.upperLeftMove(pos, col) && !gameBoard.upperLeftEat(pos, col) &&
            !gameBoard.upperRightMove(pos, col) && !gameBoard.upperRightEat(pos, col)) {
            pieceMove = false;
        }
        return pieceMove;
    }

    public override bool canYouEat(GameBoard gameBoard) {
        if (gameBoard.upperLeftEat(localPosition, color) || gameBoard.upperRightEat(localPosition, color))
            return true;
        else
            return false;
    }

    public override void highlightPossibleMoves(GameBoard gameBoard) {
        GameObject selector1 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector1");
        GameObject selector2 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector2");
        GameObject selector3 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector3");
        GameObject selector4 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector4");

        selector1.SetActive(false);
        selector2.SetActive(false);
        selector3.SetActive(false);
        selector4.SetActive(false);

        /*
         
        White Regular Pieces must highlight UpperLeft1 and UpperRight1

         */
        bool CanEat = false;

        if (gameBoard.upperLeftEat(localPosition, color)) {
            // If UpperLeft1 is not empty field check if the upperleft1 piece is opposite color
            // and if UpperLeft2 is empty field jump
            Vector3 position = selector1.transform.localPosition;
            position.x = localPosition.x + 0.5f;
            position.z = localPosition.z + 0.5f;
            selector1.transform.localPosition = position;
            selector1.SetActive(true);
            CanEat = true;
        }
        if (gameBoard.upperRightEat(localPosition, color)) {
            Vector3 position = selector2.transform.localPosition;
            position.x = localPosition.x + 0.5f;
            position.z = localPosition.z - 0.5f;
            selector2.transform.localPosition = position;
            selector2.SetActive(true);
            CanEat = true;
        }


        if (gameBoard.upperLeftMove(localPosition, color) && !CanEat) { // Check if UpperLeft1 is empty field
            Vector3 position = selector1.transform.localPosition;
            position.x = localPosition.x + 0.25f;
            position.z = localPosition.z + 0.25f;
            selector1.transform.localPosition = position;
            selector1.SetActive(true);
        }
        if (gameBoard.upperRightMove(localPosition, color) && !CanEat) {
            Vector3 position = selector2.transform.localPosition;
            position.x = localPosition.x + 0.25f;
            position.z = localPosition.z - 0.25f;
            selector2.transform.localPosition = position;
            selector2.SetActive(true);
        }
    }

    public override ArrayList getAllValidMoves(GameBoard gameBoard) {
        ArrayList moves = new ArrayList();
        bool CanEat = false;

        if (gameBoard.upperLeftEat(localPosition, color, true)) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();
            move.removePiece(board[pos.i - 1, pos.j - 1]);

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x += 0.5f;
            newPosition.z += 0.5f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i - 2, pos.j - 2),
                new V2(pos.i - 1, pos.j - 1));
            moves.Add(moveAndData);
            CanEat = true;
        }
        if (gameBoard.upperRightEat(localPosition, color, true)) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();
            move.removePiece(board[pos.i - 1, pos.j + 1]);

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x += 0.5f;
            newPosition.z -= 0.5f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i - 2, pos.j + 2),
                new V2(pos.i - 1, pos.j + 1));
            moves.Add(moveAndData);
            CanEat = true;
        }

        if (gameBoard.upperLeftMove(localPosition, color, true) && !CanEat) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x += 0.25f;
            newPosition.z += 0.25f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i - 1, pos.j - 1),
                new V2(-1));
            moves.Add(moveAndData);
        }
        if (gameBoard.upperRightMove(localPosition, color, true) && !CanEat) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x += 0.25f;
            newPosition.z -= 0.25f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i - 1, pos.j + 1),
                new V2(-1));
            moves.Add(moveAndData);
        }
        return moves;
    }
}

public class BlackRegularPiece : Piece {
    public BlackRegularPiece(PieceColor color, Vector3 localPosition, PieceManager pieceManager)
        : base(color, localPosition, pieceManager) {
        base.pieceType = PieceType.Normal;
    }

    public BlackRegularPiece(BlackRegularPiece piece)
    : base(piece.color, piece.localPosition, piece.pieceManager) {
        base.pieceType = PieceType.Normal;
    }

    public override bool canPieceMove(Vector3 pos, PieceColor col, GameBoard gameBoard) {
        bool pieceMove = true;
        if (!gameBoard.bottomLeftMove(pos, col) && !gameBoard.bottomLeftEat(pos, col) &&
            !gameBoard.bottomRightMove(pos, col) && !gameBoard.bottomRightEat(pos, col)) {
            pieceMove = false;
        }
        return pieceMove;
    }

    public override bool canYouEat(GameBoard gameBoard) {
        if (gameBoard.bottomLeftEat(localPosition, color) || gameBoard.bottomRightEat(localPosition, color))
            return true;
        else
            return false;
    }

    public override void highlightPossibleMoves(GameBoard gameBoard) {
        GameObject selector1 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector1");
        GameObject selector2 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector2");
        GameObject selector3 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector3");
        GameObject selector4 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector4");

        selector1.SetActive(false);
        selector2.SetActive(false);
        selector3.SetActive(false);
        selector4.SetActive(false);

        /*

        Black Regular Pieces must highligh BottomLeft1 and BottomRight1

        */

        bool CanEat = false;

        if (gameBoard.bottomLeftEat(localPosition, color)) {
            // If BottomLeft1 is not empty field check if the upperleft1 piece is opposite color
            // and if BottomLeft2 is empty field jump
            Vector3 position = selector1.transform.localPosition;
            position.x = localPosition.x - 0.5f;
            position.z = localPosition.z + 0.5f;
            selector1.transform.localPosition = position;
            selector1.SetActive(true);
            CanEat = true;
        }
        if (gameBoard.bottomRightEat(localPosition, color)) {
            Vector3 position = selector2.transform.localPosition;
            position.x = localPosition.x - 0.5f;
            position.z = localPosition.z - 0.5f;
            selector2.transform.localPosition = position;
            selector2.SetActive(true);
            CanEat = true;
        }


        if (gameBoard.bottomLeftMove(localPosition, color) && !CanEat) { // Check if UpperLeft1 is empty field
            Vector3 position = selector1.transform.localPosition;
            position.x = localPosition.x - 0.25f;
            position.z = localPosition.z + 0.25f;
            selector1.transform.localPosition = position;
            selector1.SetActive(true);
        }
        if (gameBoard.bottomRightMove(localPosition, color) && !CanEat) {
            Vector3 position = selector2.transform.localPosition;
            position.x = localPosition.x - 0.25f;
            position.z = localPosition.z - 0.25f;
            selector2.transform.localPosition = position;
            selector2.SetActive(true);
        }
    }

    public override ArrayList getAllValidMoves(GameBoard gameBoard) {
        ArrayList moves = new ArrayList();
        bool CanEat = false;

        if (gameBoard.bottomLeftEat(localPosition, color, true)) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();
            move.removePiece(board[pos.i + 1, pos.j - 1]);

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x -= 0.5f;
            newPosition.z += 0.5f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i + 2, pos.j - 2),
                new V2(pos.i + 1, pos.j - 1));
            moves.Add(moveAndData);
            CanEat = true;
        }
        if (gameBoard.bottomRightEat(localPosition, color, true)) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();
            move.removePiece(board[pos.i + 1, pos.j + 1]);

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x -= 0.5f;
            newPosition.z -= 0.5f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i + 2, pos.j + 2),
                new V2(pos.i + 1, pos.j + 1));
            moves.Add(moveAndData);
            CanEat = true;
        }

        if (gameBoard.bottomLeftMove(localPosition, color, true) && !CanEat) { // Check if UpperLeft1 is empty field
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x -= 0.25f;
            newPosition.z += 0.25f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i + 1, pos.j - 1),
                new V2(-1));
            moves.Add(moveAndData);
        }
        if (gameBoard.bottomRightMove(localPosition, color, true) && !CanEat) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x -= 0.25f;
            newPosition.z -= 0.25f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i + 1, pos.j + 1),
                new V2(-1));
            moves.Add(moveAndData);
        }
        return moves;
    }
}



public class KingPiece : Piece {
    public KingPiece(PieceColor color, Vector3 localPosition, PieceManager pieceManager)
        : base(color, localPosition, pieceManager) {
        base.pieceType = PieceType.King;
    }

    public KingPiece(KingPiece piece)
        : base(piece.color, piece.localPosition, piece.pieceManager) {
        base.pieceType = PieceType.King;
    }

    public override bool canPieceMove(Vector3 pos, PieceColor col, GameBoard gameBoard) {
        bool pieceMove = true;
        if (!gameBoard.upperLeftMove(pos, col) && !gameBoard.upperLeftEat(pos, col) &&
            !gameBoard.upperRightMove(pos, col) && !gameBoard.upperRightEat(pos, col) &&
            !gameBoard.bottomLeftMove(pos, col) && !gameBoard.bottomLeftEat(pos, col) &&
            !gameBoard.bottomRightMove(pos, col) && !gameBoard.bottomRightEat(pos, col)) {
            pieceMove = false;
        }
        return pieceMove;
    }

    public override bool canYouEat(GameBoard gameBoard) {
        if (gameBoard.upperLeftEat(localPosition, color) || gameBoard.upperRightEat(localPosition, color) ||
            gameBoard.bottomLeftEat(localPosition, color) || gameBoard.bottomRightEat(localPosition, color))
            return true;
        else
            return false;
    }

    public override void highlightPossibleMoves(GameBoard gameBoard) {
        GameObject selector1 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector1");
        GameObject selector2 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector2");
        GameObject selector3 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector3");
        GameObject selector4 = GameObject.Find("/CheckerBoard1/CheckerBoard1/Selector4");

        selector1.SetActive(false);
        selector2.SetActive(false);
        selector3.SetActive(false);
        selector4.SetActive(false);

        bool CanEat = false;

        if (gameBoard.upperLeftEat(localPosition, color)) {
            Vector3 position = selector1.transform.localPosition;
            position.x = localPosition.x + 0.5f;
            position.z = localPosition.z + 0.5f;
            selector1.transform.localPosition = position;
            selector1.SetActive(true);
            CanEat = true;
        }
        if (gameBoard.upperRightEat(localPosition, color)) {
            Vector3 position = selector2.transform.localPosition;
            position.x = localPosition.x + 0.5f;
            position.z = localPosition.z - 0.5f;
            selector2.transform.localPosition = position;
            selector2.SetActive(true);
            CanEat = true;
        }
        if (gameBoard.bottomLeftEat(localPosition, color)) {
            Vector3 position = selector3.transform.localPosition;
            position.x = localPosition.x - 0.5f;
            position.z = localPosition.z + 0.5f;
            selector3.transform.localPosition = position;
            selector3.SetActive(true);
            CanEat = true;
        }
        if (gameBoard.bottomRightEat(localPosition, color)) {
            Vector3 position = selector4.transform.localPosition;
            position.x = localPosition.x - 0.5f;
            position.z = localPosition.z - 0.5f;
            selector4.transform.localPosition = position;
            selector4.SetActive(true);
            CanEat = true;
        }


        if (gameBoard.upperLeftMove(localPosition, color) && !CanEat) {
            Vector3 position = selector1.transform.localPosition;
            position.x = localPosition.x + 0.25f;
            position.z = localPosition.z + 0.25f;
            selector1.transform.localPosition = position;
            selector1.SetActive(true);
        }
        if (gameBoard.upperRightMove(localPosition, color) && !CanEat) {
            Vector3 position = selector2.transform.localPosition;
            position.x = localPosition.x + 0.25f;
            position.z = localPosition.z - 0.25f;
            selector2.transform.localPosition = position;
            selector2.SetActive(true);
        }
        if (gameBoard.bottomLeftMove(localPosition, color) && !CanEat) {
            Vector3 position = selector3.transform.localPosition;
            position.x = localPosition.x - 0.25f;
            position.z = localPosition.z + 0.25f;
            selector3.transform.localPosition = position;
            selector3.SetActive(true);
        }
        if (gameBoard.bottomRightMove(localPosition, color) && !CanEat) {
            Vector3 position = selector4.transform.localPosition;
            position.x = localPosition.x - 0.25f;
            position.z = localPosition.z - 0.25f;
            selector4.transform.localPosition = position;
            selector4.SetActive(true);
        }
    }

    public override ArrayList getAllValidMoves(GameBoard gameBoard) {
        ArrayList moves = new ArrayList();
        bool CanEat = false;

        if (gameBoard.upperLeftEat(localPosition, color, true)) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();
            move.removePiece(board[pos.i - 1, pos.j - 1]);

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x += 0.5f;
            newPosition.z += 0.5f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i - 2, pos.j - 2),
                new V2(pos.i - 1, pos.j - 1));
            moves.Add(moveAndData);
            CanEat = true;
        }
        if (gameBoard.upperRightEat(localPosition, color, true)) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();
            move.removePiece(board[pos.i - 1, pos.j + 1]);

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x += 0.5f;
            newPosition.z -= 0.5f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i - 2, pos.j + 2),
                new V2(pos.i - 1, pos.j + 1));
            moves.Add(moveAndData);
            CanEat = true;
        }
        if (gameBoard.bottomLeftEat(localPosition, color, true)) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();
            move.removePiece(board[pos.i + 1, pos.j - 1]);

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x -= 0.5f;
            newPosition.z += 0.5f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i + 2, pos.j - 2),
                new V2(pos.i + 1, pos.j - 1));
            moves.Add(moveAndData);
            CanEat = true;
        }
        if (gameBoard.bottomRightEat(localPosition, color, true)) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();
            move.removePiece(board[pos.i + 1, pos.j + 1]);

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x -= 0.5f;
            newPosition.z -= 0.5f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i + 2, pos.j + 2),
                new V2(pos.i + 1, pos.j + 1));
            moves.Add(moveAndData);
            CanEat = true;
        }


        if (gameBoard.upperLeftMove(localPosition, color, true) && !CanEat) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x += 0.25f;
            newPosition.z += 0.25f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i - 1, pos.j - 1),
                new V2(-1));
            moves.Add(moveAndData);
        }
        if (gameBoard.upperRightMove(localPosition, color, true) && !CanEat) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x += 0.25f;
            newPosition.z -= 0.25f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i - 1, pos.j + 1),
                new V2(-1));
            moves.Add(moveAndData);
        }
        if (gameBoard.bottomLeftMove(localPosition, color, true) && !CanEat) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x -= 0.25f;
            newPosition.z += 0.25f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i + 1, pos.j - 1),
                new V2(-1));
            moves.Add(moveAndData);
        }
        if (gameBoard.bottomRightMove(localPosition, color, true) && !CanEat) {
            GameBoard move = new GameBoard(gameBoard);

            Position pos = move.calculatePosition(this);
            Piece[,] board = move.getBoard();

            Vector3 oldPosition = board[pos.i, pos.j].getLocalPosition();
            Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z);
            newPosition.x -= 0.25f;
            newPosition.z -= 0.25f;
            board[pos.i, pos.j].setLocalPosition(newPosition);
            move.movePiece(oldPosition, board[pos.i, pos.j]);

            Move moveAndData = new Move(move, new V2(pos.i, pos.j), new V2(pos.i + 1, pos.j + 1),
                new V2(-1));
            moves.Add(moveAndData);
        }
        return moves;
    }
}
