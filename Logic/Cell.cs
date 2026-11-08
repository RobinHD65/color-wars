using Godot;

public struct Cell
{
    public Color color;
    public int points;

    public void set(Color color, int points)
    {
        this.color = color;
        this.points = points;
    }
    public void clear()
    {
        color = Color.None;
        points = 0;
    }
    public void print()
    {
        GD.Print($"Color: {color}; points: {points}");
    }
}
