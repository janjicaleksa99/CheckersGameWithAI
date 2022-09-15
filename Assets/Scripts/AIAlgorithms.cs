using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinMaxPlayers {
    MinPlayer, MaxPlayer
}

public struct AIVals {
    public float evaluation;
    public Move move;

    public AIVals(float evaluation, Move move) {
        this.evaluation = evaluation;
        this.move = move;
    }

    public AIVals(float evaluation) {
        this.evaluation = evaluation;
        this.move = new Move();
    }

    public AIVals(Move move) : this() {
        this.move = move;
    }
}

public abstract class AIAlgorithms {
    public static AIAlgorithms currentAIAlgorithm = null;
    public static int depthOfTree = -1;
    public static ArrayList possibleAIMoves = new ArrayList();

    public abstract AIVals AIAlgorithm(GameBoard position, int depth, MinMaxPlayers minimaxPlayer);

    public ArrayList getAllMoves(GameBoard position, PieceColor color) {
        ArrayList moves = new ArrayList();

        ArrayList pieces = position.getAllPieces(color);
        foreach (Piece piece in pieces) {
            if (PieceManager.pieceThatJumpsAgain != null) { // check if there is a piece that must jump again
                if (PieceManager.pieceThatJumpsAgain.getLocalPosition() != piece.getLocalPosition())
                    // check if this piece is the piece that has to jump
                    continue;
            }
            ArrayList validMoves = piece.getAllValidMoves(position);
            foreach (Move move in validMoves)
                moves.Add(move);
        }
        return moves;
    }
}

public class Minimax : AIAlgorithms
{

    public override AIVals AIAlgorithm(GameBoard position, int depth, MinMaxPlayers minimaxPlayer) {
        if (depth == 0 || position.haveAWinner() != PlayerType.None)
            return new AIVals(position.evaluateBoard(), new Move(position));

        if (minimaxPlayer == MinMaxPlayers.MaxPlayer) { // Max player
            float maxEval = float.MinValue;
            Move bestMove = new Move();
            foreach (Move move in getAllMoves(position, PieceColor.Black)) {
                float evaluation = AIAlgorithm(move.position, depth - 1, MinMaxPlayers.MinPlayer).evaluation;
                if (depth == depthOfTree) {
                    possibleAIMoves.Add(new AIVals(evaluation, move));
                }
                if (evaluation > maxEval) {
                    maxEval = evaluation;
                    bestMove = move;
                }
            }
            return new AIVals(maxEval, bestMove);
        }
        else { // Min player
            float minEval = float.MaxValue;
            Move bestMove = new Move();
            foreach (Move move in getAllMoves(position, PieceColor.White)) {
                float evaluation = AIAlgorithm(move.position, depth - 1, MinMaxPlayers.MaxPlayer).evaluation;
                if (evaluation < minEval) {
                    minEval = evaluation;
                    bestMove = move;
                }
            }
            return new AIVals(minEval, bestMove);
        }
    }
}

public class MinimaxAlphaBeta : AIAlgorithms {

    public AIVals minimaxAlphaBeta(GameBoard position, int depth, MinMaxPlayers minimaxPlayer, float alpha, float beta) {
        if (depth == 0 || position.haveAWinner() != PlayerType.None)
            return new AIVals(position.evaluateBoard(), new Move(position));

        if (minimaxPlayer == MinMaxPlayers.MaxPlayer) { // Max player
            float maxEval = float.MinValue;
            Move bestMove = new Move();
            foreach (Move move in getAllMoves(position, PieceColor.Black)) {
                float evaluation = minimaxAlphaBeta(move.position, depth - 1, MinMaxPlayers.MinPlayer, alpha, beta).evaluation;
                if (depth == depthOfTree) {
                    possibleAIMoves.Add(new AIVals(evaluation, move));
                }
                if (evaluation > maxEval) {
                    maxEval = evaluation;
                    bestMove = move;
                }
                if (evaluation > alpha) {
                    alpha = evaluation;
                }
                if (alpha >= beta)
                    break;
            }
            return new AIVals(maxEval, bestMove);
        }
        else { // Min player
            float minEval = float.MaxValue;
            Move bestMove = new Move();
            foreach (Move move in getAllMoves(position, PieceColor.White)) {
                float evaluation = minimaxAlphaBeta(move.position, depth - 1, MinMaxPlayers.MaxPlayer, alpha, beta).evaluation;
                if (evaluation < minEval) {
                    minEval = evaluation;
                    bestMove = move;
                }
                if (evaluation < beta) {
                    beta = evaluation;
                }
                if (alpha >= beta)
                    break;
            }
            return new AIVals(minEval, bestMove);
        }
    }

    public override AIVals AIAlgorithm(GameBoard position, int depth, MinMaxPlayers minimaxPlayer) {
        return minimaxAlphaBeta(position, depth, minimaxPlayer, float.MinValue, float.MaxValue);
    }
}


public class NegaMax : AIAlgorithms {
    public override AIVals AIAlgorithm(GameBoard position, int depth, MinMaxPlayers negamaxPlayer) {
        if (depth == 0 || position.haveAWinner() != PlayerType.None)
            return new AIVals(position.evaluateBoard() * (negamaxPlayer == MinMaxPlayers.MinPlayer ? -1 : 1), new Move(position));

        float maxEval = float.MinValue;
        Move bestMove = new Move();
        MinMaxPlayers nextPlayer;
        if (negamaxPlayer == MinMaxPlayers.MaxPlayer)
            nextPlayer = MinMaxPlayers.MinPlayer;
        else
            nextPlayer = MinMaxPlayers.MaxPlayer;
        foreach (Move move in getAllMoves(position, negamaxPlayer == MinMaxPlayers.MaxPlayer ? PieceColor.Black : PieceColor.White)) {
            float evaluation = -AIAlgorithm(move.position, depth - 1, nextPlayer).evaluation;
            if (depth == depthOfTree) {
                possibleAIMoves.Add(new AIVals(evaluation, move));
            }
            if (evaluation > maxEval) {
                maxEval = evaluation;
                bestMove = move;
            }
        }
        return new AIVals(maxEval, bestMove);
    }
}