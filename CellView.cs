using Godot;
using System.Threading.Tasks;
using System;



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
				DrawCircle(Size / 2, 20, Colors.RoyalBlue);
				break;
			case Color.Red:
				DrawCircle(Size / 2, 20, new Godot.Color("#D95D5D"));
				break;
		}
		if (cell.color != Color.None)
		{
			string text = cell.points.ToString();
			Vector2 textSize = font.GetStringSize(text);
			Vector2 position = Size / 2 - textSize / 2;
			position.Y += textSize.Y * 3 / 4;
			DrawString(font, position, text);
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
