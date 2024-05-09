namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents the main menu of the game. </summary>
public partial class MainMenu : Control
{
	[Export]
	public EventBus EventBus { get; set; }

	//Menu buttons
	Button newGame;
	Button saveGame;
	Button loadGame;
	Button exitGame;

	/// <summary> Called when the node enters the scene tree for the first time. </summary>
	public override void _Ready()
	{
		newGame = GetNode<Button>("PanelContainer/MarginContainer/MainMenu/NewGameButton");
		saveGame = GetNode<Button>("PanelContainer/MarginContainer/MainMenu/SaveButton");
		loadGame = GetNode<Button>("PanelContainer/MarginContainer/MainMenu/LoadButton");
		exitGame = GetNode<Button>("PanelContainer/MarginContainer/MainMenu/ExitButton");
		//Events
		newGame.Pressed += OnNewGamePressed;
		saveGame.Pressed += OnSaveGamePressed;
		loadGame.Pressed += OnLoadGamePressed;
		exitGame.Pressed += OnExitGamePressed;
	}

	private void OnNewGamePressed()
	{
		EventBus.EmitSignal("NewGame");
	}

	private void OnSaveGamePressed()
	{
		var saveFiles = SaveSystem.GetSaveFilesInfo();
		if (saveFiles.Count > 0) EventBus.EmitSignal("OpenSaveMenu");
		else EventBus.EmitSignal("SaveGame", false);
	}

	private void OnLoadGamePressed()
	{
		EventBus.EmitSignal("OpenLoadMenu");
	}

	private void OnExitGamePressed()
	{
		EventBus.EmitSignal("ExitGame");
	}

	// public override void _Input(InputEvent @event)
	// {
	//     if (@event.IsActionPressed("toggle_menu"))
	//     {
	//         EventBus.EmitSignal("ToggleMenu");
	//     }
	// }
}

