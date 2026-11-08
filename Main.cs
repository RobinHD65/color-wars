using Godot;

public partial class Main : Node
{
	private BoardView boardView;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		boardView = GetNode<BoardView>("BoardView");
		boardView.InstantiateGame();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
