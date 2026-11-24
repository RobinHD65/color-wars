using Godot;


public partial class CellView : TextureButton
{
	[Signal]
	public delegate void cellClickedEventHandler(Vector2I coordinates);
	public Vector2I coordinates { get; set; }
	private Cell cell { get; set; }
	private Font font = ThemeDB.FallbackFont;
	public override void _Draw()
	{
		switch (cell.color)
		{
			case Color.Blue:
				DrawCircle(Size / 2, Size.X / 3, Colors.RoyalBlue);
				break;
			case Color.Red:
				DrawCircle(Size / 2, Size.X / 3, new Godot.Color("#D95D5D"));
				break;
		}
		if (cell.color != Color.None)
		{
    		string text = cell.points.ToString();
    		int fontSize = 32;
    		Vector2 textSize = font.GetStringSize(text, HorizontalAlignment.Left, -1, fontSize);
    		Vector2 position = new Vector2(
        		(Size.X - textSize.X) / 2,
        		(Size.Y + textSize.Y) / 2 - 12 
    		);
    		DrawString(font, position, text, HorizontalAlignment.Left, -1, fontSize);
		}
	}
	public void OnPressed()
	{
		EmitSignal(SignalName.cellClicked, coordinates);
	}
	public void UpdateCellView(Cell cell)
	{
		this.cell = cell;
		QueueRedraw();
	}
}
