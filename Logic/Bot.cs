using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Numerics;

public partial class Bot : GodotObject
{
    private Board GameState;
    private Stopwatch stopwatch = new Stopwatch();
    private int calcTime;
    public int lastDepth;
    public Bot(Board gameState)
    {
        GameState = gameState;
    }
    private List<Vector2I> GenerateMoves()
    {
        List<Vector2I> output = new();
        if (GameState.CheckWin() == Color.None)
        {
            for (int x = 0; x < GameState.sizeX; x++)
            {
                for (int y = 0; y < GameState.sizeY; y++)
                {
                    if (GameState.Cells[x, y].color == GameState.ColorToMove)
                    {
                        output.Add(new Vector2I(x, y));
                    }
                }
            }
        }
        return output;
    }
    // Übergebe hier immer die Farbe des Bots, damit die Perspektive gleich bleibt
    public int Evaluate(Color botColor)
    {
        Color winner = GameState.CheckWin();
        if (winner != Color.None)
        {
            // Ein hoher Wert statt int.MaxValue verhindert Overflows bei späteren Rechnungen
            if (winner == botColor) return 1000000;
            else return -1000000;
        }

        int eval = 0;
        foreach (Cell cell in GameState.Cells)
        {
            if (cell.color == Color.None) continue;

            if (cell.color == botColor)
            {
                if (cell.points < 3) eval += 10;
                else eval += 30;
            }
            else
            {
                if (cell.points < 3) eval -= 10;
                else eval -= 30;
            }
        }
        return eval;
    }
    private int miniMax(int tiefe, Color botColor, int alpha, int beta)
    {
        Color winner = GameState.CheckWin();
        if (tiefe == 0 || winner != Color.None)
            return Evaluate(botColor);

        List<Vector2I> possibleMoves = GenerateMoves();
        if (possibleMoves.Count == 0)
            return Evaluate(botColor);

        bool isMaximizing = GameState.ColorToMove == botColor;
        if (isMaximizing) // Bot ist dran, wir wollen den höchsten Score
        {
            int maxScore = int.MinValue;
            foreach (Vector2I move in possibleMoves)
            {
                if (stopwatch.ElapsedMilliseconds >= calcTime) return maxScore;

                if (GameState.MakeMove(move, false))
                {
                    // Prüfen, wer nach diesem simulierten Zug dran ist
                    int score = miniMax(tiefe - 1, botColor, alpha, beta);
                    GameState.UnmakeMove();

                    if (score > maxScore) maxScore = score;
                    alpha = Math.Max(alpha, maxScore);
                    if (alpha >= beta) break;
                }
            }
            return maxScore;
        }
        else // Gegner ist dran, wir müssen vom Schlimmsten ausgehen (niedrigster Score)
        {
            int minScore = int.MaxValue;
            foreach (Vector2I move in possibleMoves)
            {
                if (GameState.MakeMove(move, false))
                {
                    int score = miniMax(tiefe - 1, botColor, alpha, beta);
                    GameState.UnmakeMove();

                    if (score < minScore) minScore = score;
                    beta = Math.Min(beta, minScore);
                    if (alpha >= beta) break;
                }
            }
            return minScore;
        }
    }
    public Vector2I CalculateMove()
    {
        List<Vector2I> possibleMoves = GenerateMoves();
        if (possibleMoves.Count == 0) return new Vector2I(-1, -1); // Fallback

        int maxScore = int.MinValue;
        Vector2I bestMoveTD = possibleMoves[0];
        Vector2I calculatedMove = possibleMoves[0];
        // Die Farbe des Bots für diesen ganzen Rechenbaum merken
        Color botColor = GameState.ColorToMove;
        stopwatch.Reset();
        calcTime = 500;
        stopwatch.Start();
        lastDepth = 0;
        //Zeit
        for (int depth = 0; depth < 20; depth++)
        {
            int alpha = int.MinValue;
            int beta = int.MaxValue;
            foreach (Vector2I move in possibleMoves)
            {
                if (stopwatch.ElapsedMilliseconds >= calcTime) return calculatedMove; ;

                if (GameState.MakeMove(move, false))
                {
                    int score = miniMax(depth, botColor, alpha, beta);
                    GameState.UnmakeMove();

                    if (score > maxScore)
                    {
                        maxScore = score;
                        bestMoveTD = move;
                    }
                    alpha = Math.Max(alpha, maxScore);
                }
            }
            calculatedMove = bestMoveTD;
            lastDepth++;
        }
        return calculatedMove;
    }
}
