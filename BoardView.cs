using Godot;
using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class BoardView : Control
{
	private Board board;
	private Bot bot;
	private CellView[,] cellView;
	private PackedScene cellScene = ResourceLoader.Load<PackedScene>("res://CellView.tscn");
	private GridContainer grid;
	private ColorRect colorIndicator;
	private RichTextLabel searchStats;
	private int sizeX;
	private int sizeY;
	private bool locked;
	private List<Cell[,]> pendingExplosions = new List<Cell[,]>();
	public override void _Ready()
	{
		grid = GetNode<GridContainer>("Cells");
		colorIndicator = GetNode<ColorRect>("colorToMoveIndicator");
		searchStats = GetNode<RichTextLabel>("SearchStats");
		board = new Board();
		bot = new Bot(board);
		board.GetExplosions += GetExplosions;
	}
	public void InstantiateGame()
	{
		InstantiateGame(7, 7);
		board.StartGame();
		UpdateView();
	}
	public void InstantiateGame(int sizeX, int sizeY)
	{
		this.sizeX = sizeX;
		this.sizeY = sizeY;
		board.StartGame(sizeX, sizeY);
		cellView = new CellView[sizeX, sizeY];
		for (int y = 0; y < sizeY; y++)
		{
			for (int x = 0; x < sizeX; x++)
			{
				var cell = cellScene.Instantiate<CellView>();
				cellView[x, y] = cell;
				cell.coordinates = new Vector2I(x, y);
				cell.cellClicked += OnCellClicked;
				grid.AddChild(cell);
			}
		}
	}
	public void UpdateView()
	{
		UpdateCells(board.Cells);
		switch (board.ColorToMove)
		{
			case Color.Blue:
				colorIndicator.Color = Colors.RoyalBlue;
				break;
			case Color.Red:
				colorIndicator.Color = new Godot.Color("#D95D5D");
				break;
		}
		if (board.CheckWin() != Color.None)
		{
			GD.Print(board.CheckWin() + " wins!");
		}
	}
	private void UpdateCells(Cell[,] cells)
	{
		for (int y = 0; y < sizeY; y++)
		{
			for (int x = 0; x < sizeX; x++)
			{
				cellView[x, y].UpdateCellView(cells[x, y]);
			}
		}
	}
	private void GetExplosions(List<Cell[,]> boards) { pendingExplosions = boards; }
	private async Task AnimateExplosions()
	{
		for (int i = 0; i < pendingExplosions.Count; i++)
		{
			UpdateCells(pendingExplosions[i]);
			if (i != pendingExplosions.Count - 1) await Task.Delay(1000);
		}
		pendingExplosions.Clear();
		return;
	}
	private async void OnCellClicked(Vector2I position)
	{
		if (locked == false)
		{
			if (board.MakeMove(position, true) == false)
			{
				GD.PrintErr("Ungültiger Zug");
				return;
			}
			locked = true;
			await AnimateExplosions();
			UpdateView();
			locked = false;
		}
		else
		{
			GD.PrintErr("Eingaben sind gelockt");
		}
	}
	public void MakeBotMove()
	{
		Vector2I move = bot.CalculateMove();
		OnCellClicked(move);
		searchStats.Text = $"Tiefe: {bot.lastDepth}";
	}
	public override void _Process(double delta)
	{

	}
}
