using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GBData {
    private Piece[,] board = new Piece[8, 8];
    private PlayerType playerOnTurn = PlayerType.WhitePlayer; // White player plays first
    private int blackPieces = 0;
    private int whitePieces = 0;
    private int blackKings = 0;
    private int whiteKings = 0;

    public GBData(GameBoard gameBoard) {
        Piece[,] copyBoard = gameBoard.getBoard();
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++) {
                if (copyBoard[i, j] != null) {
                    if (copyBoard[i, j].getPieceType() == PieceType.Normal) {
                        if (copyBoard[i, j].getPieceColor() == PieceColor.Black) {
                            board[i, j] = new BlackRegularPiece((BlackRegularPiece)copyBoard[i, j]);
                        }
                        else {
                            board[i, j] = new WhiteRegularPiece((WhiteRegularPiece)copyBoard[i, j]);
                        }
                    }
                    else if (copyBoard[i, j].getPieceType() == PieceType.King) {
                        board[i, j] = new KingPiece((KingPiece)copyBoard[i, j]);
                    }
                }
            }
        playerOnTurn = gameBoard.getplayerOnTurn();
        blackPieces = gameBoard.getBlackPieces();
        blackKings = gameBoard.getBlackKings();
        whitePieces = gameBoard.getWhitePieces();
        whiteKings = gameBoard.getWhiteKings();
    }
}
