using ChessChallenge.API;
using Microsoft.CodeAnalysis.Operations;
using System;

public class MyBot : IChessBot
{
    static int MAX = 1000;
    static int MIN = -1000;
    static int MAX_DEPTH = 5;
    static int n_evaluations = 0;

    public Move Think(Board board, Timer timer)
    {
        n_evaluations = 0;
        Move[] moves = board.GetLegalMoves();
        int maxPuntuation = minimax(1, moves[0], true, MIN, MAX, board);
        Move bestMove = moves[0];
        int index = 0;
        int bestindex = 0;
        foreach (Move m in moves)
        {
            int currentMovePuntuation = minimax(1, m, true, MIN, MAX, board);
            if (currentMovePuntuation >= maxPuntuation)
            {
                bestMove = m;
                maxPuntuation = currentMovePuntuation;
                bestindex = index;
            }
            ++index;
        }
        Console.WriteLine("Number of evaluations: " + n_evaluations);
        //if (maxPuntuation > 10) {
        //    Console.WriteLine("Best move evaluation: " + maxPuntuation);
        //}
        //Console.WriteLine("best index " + bestindex + "/" + index);
        return bestMove;
    }


    static int getValue(Move move, Board board) {
        int val = 0;
        if (move.IsCapture) { val += 10; }
        if (move.IsCastles) { val += 8;  }
        return val;
    }

    static int getPieceValue(PieceType type)
    {
        switch  (type)
        {
            case PieceType.Pawn: return 100;
            case PieceType.Knight: return 300;
            case PieceType.Bishop: return 300;
            case PieceType.Rook: return 500;
            case PieceType.Queen: return 900;
            case PieceType.King: return 1000;
            default: return 0;
        }
    }

    static int countPlayerMaterial(Board board, bool white) {
        PieceList pieceList = board.GetPieceList(PieceType.Pawn, white); 
        int material = 0;
        material += board.GetPieceList(PieceType.Pawn, white).Count * getPieceValue(PieceType.Pawn);
        material += board.GetPieceList(PieceType.Knight, white).Count * getPieceValue(PieceType.Knight);
        material += board.GetPieceList(PieceType.Bishop, white).Count * getPieceValue(PieceType.Bishop);
        material += board.GetPieceList(PieceType.Rook, white).Count * getPieceValue(PieceType.Rook);
        material += board.GetPieceList(PieceType.Queen, white).Count * getPieceValue(PieceType.Queen);
        return material;
    }

    static int countBoardMaterial(Board board) {
        ++n_evaluations;
        int whiteMaterial = countPlayerMaterial(board, true);
        int blackMaterial = countPlayerMaterial(board, false);
        return (whiteMaterial - blackMaterial) * ((board.IsWhiteToMove) ? 1:-1);
    }
    static int minimax(int depth, Move move, Boolean maximizingPlayer, int alpha, int beta, Board board)
    {
        if (depth == MAX_DEPTH)
            return countBoardMaterial(board);

        if (maximizingPlayer)
        {
            board.MakeMove(move);
            Move[] moves = board.GetLegalMoves();
            int best = MIN;
            foreach (Move m in moves)
            {
                int val = minimax(depth + 1, m, !maximizingPlayer, alpha, beta, board);
                best = Math.Max(best, val);
                alpha = Math.Max(alpha, best);
                if (beta <= alpha)
                    break;
            }
            board.UndoMove(move);
            return best;
        }
        else
        {
            board.MakeMove(move);
            Move[] moves = board.GetLegalMoves();
            int best = MAX;
            foreach (Move m in moves)
            {
                int val = minimax(depth + 1, m, !maximizingPlayer, alpha, beta, board);
                best = Math.Min(best, val);
                beta = Math.Min(beta, best);
                if (beta <= alpha)
                    break;
            }
            board.UndoMove(move);
            return best;
        }
    }
}