using System.Collections.Generic;
using Godot;

public delegate void getExplosionsEventHandler(List<Cell[,]> boards);

public partial class Board : GodotObject
{
    public event getExplosionsEventHandler GetExplosions;
    public int playerToMove;
    public List<Color> Players = new();
    public Color ColorToMove => Players[playerToMove];
    public Cell[,] Cells;
    public int sizeX;
    public int sizeY;
    private Stack<Cell[,]> unmakeInfo = new();
    public void StartGame(int sizeX, int sizeY)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        Players.Clear();
        Players.Add(Color.Red);
        Players.Add(Color.Blue);
        playerToMove = 0;
        Cells = new Cell[sizeX, sizeY];
    }
    public void StartGame() //Standard 1v1
    {
        StartGame(7, 7);
        Cells[2, 1].set(Color.Red, 1);
        Cells[1, 2].set(Color.Red, 1);
        Cells[3, 2].set(Color.Red, 2);
        Cells[2, 3].set(Color.Red, 2);
        Cells[4, 3].set(Color.Blue, 2);
        Cells[3, 4].set(Color.Blue, 2);
        Cells[5, 4].set(Color.Blue, 1);
        Cells[4, 5].set(Color.Blue, 1);
    }

    public bool MakeMove(Vector2I move, bool animate)
    {
        //Prüfen, ob ein gültiger Zug gemacht wird
        if (move.X < 0 || move.Y < 0)
            return false;
        if (move.X > sizeX - 1 || move.Y > sizeY - 1)
            return false;
        if (Cells[move.X, move.Y].color != ColorToMove)
            return false;
        if (CheckWin() != Color.None)
            return false;

        unmakeInfo.Push((Cell[,])Cells.Clone());

        Cells[move.X, move.Y].points++;
        if (Cells[move.X, move.Y].points >= 4)
            Explode(move, animate);

        playerToMove = (playerToMove + 1) % Players.Count;

        return true; //Zug wurde erfolgreich ausgeführt
    }
    public void UnmakeMove()
    {
        if (unmakeInfo.Count == 0)
        {
            GD.PrintErr("Es gibt keinen Zug zum Zurücknehmen");
            return;
        }
        Cells = unmakeInfo.Pop();
        playerToMove--;
        if (playerToMove < 0) playerToMove = Players.Count - 1;
    }
    private void Explode(Vector2I move, bool animate)
    {
        Queue<Vector2I> queue = new();
        List<Cell[,]> boards = new();
        Vector2I cell;
        queue.Enqueue(move);
        while (queue.Count > 0)
        {
            int levelSize = queue.Count;
            for (int i = 0; i < levelSize; i++)
            {
                cell = queue.Dequeue();
                ExplodeCell(cell, queue);
            }
            //Ab hier ist eine Ebene vorbei
            if (animate)
                boards.Add((Cell[,])Cells.Clone());

            if (CheckWin() != Color.None)
                break;
        }
        if (animate) GetExplosions.Invoke(boards);
    }
    private void ExplodeCell(Vector2I cell, Queue<Vector2I> queue)
    {
        int explodePoints = Cells[cell.X, cell.Y].points - 3; //Feature kein Bug
        if (cell.X > 0)
        {
            Cells[cell.X - 1, cell.Y].color = ColorToMove;
            Cells[cell.X - 1, cell.Y].points += explodePoints;
            if (Cells[cell.X - 1, cell.Y].points >= 4)
            {
                if (!queue.Contains(new Vector2I(cell.X - 1, cell.Y)))
                    queue.Enqueue(new Vector2I(cell.X - 1, cell.Y));
            }
        }
        if (cell.X < sizeX - 1)
        {
            Cells[cell.X + 1, cell.Y].color = ColorToMove;
            Cells[cell.X + 1, cell.Y].points += explodePoints;
            if (Cells[cell.X + 1, cell.Y].points >= 4)
            {
                if (!queue.Contains(new Vector2I(cell.X + 1, cell.Y)))
                    queue.Enqueue(new Vector2I(cell.X + 1, cell.Y));
            }
        }
        if (cell.Y > 0)
        {
            Cells[cell.X, cell.Y - 1].color = ColorToMove;
            Cells[cell.X, cell.Y - 1].points += explodePoints;
            if (Cells[cell.X, cell.Y - 1].points >= 4)
            {
                if (!queue.Contains(new Vector2I(cell.X, cell.Y - 1)))
                    queue.Enqueue(new Vector2I(cell.X, cell.Y - 1));
            }
        }
        if (cell.Y < sizeY - 1)
        {
            Cells[cell.X, cell.Y + 1].color = ColorToMove;
            Cells[cell.X, cell.Y + 1].points += explodePoints;
            if (Cells[cell.X, cell.Y + 1].points >= 4)
            {
                if (!queue.Contains(new Vector2I(cell.X, cell.Y + 1)))
                    queue.Enqueue(new Vector2I(cell.X, cell.Y + 1));
            }
        }
        Cells[cell.X, cell.Y].clear();
    }
    public Color CheckWin() //Returns the Color that won, if game is still ongoing it returns Color.None
    {
        
        Color firstColor = Color.None;
        foreach (Cell cell in Cells)
        {
            if (cell.color != Color.None)
            {
                if (firstColor == Color.None)
                    firstColor = cell.color;
                else if (firstColor != cell.color)
                    return Color.None;
            }

        } 
        return firstColor;
    }
}

public enum Color
{
    None = 0,
    Red = 1,
    Blue = 2
}