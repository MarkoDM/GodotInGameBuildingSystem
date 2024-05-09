namespace Godot.GodotInGameBuildingSystem;

/// <summary>
/// Represents main event bus that handles various signals related to the building system and game menus.
/// </summary>
public partial class EventBus : Node
{
	// Building menu signals

	/// <summary> Signal emitted when a slot is clicked in the building menu. </summary>
	[Signal]
	public delegate void SlotClickedEventHandler(int index, int button);

	/// <summary> Signal emitted when the level is changed. </summary>
	[Signal]
	public delegate void LevelChangedEventHandler(int level);

	/// <summary> Signal emitted when the build mode is changed. </summary>
	[Signal]
	public delegate void BuildModeChangedEventHandler(bool enabled);

	/// <summary> Signal emitted when the demolition mode is changed. </summary>
	[Signal]
	public delegate void DemolitionModeChangedEventHandler(bool enabled);

	// Main menu signals

	/// <summary> Signal emitted when a new game is started. </summary>
	[Signal]
	public delegate void NewGameEventHandler();

	/// <summary> Signal emitted when the game is saved. </summary>
	[Signal]
	public delegate void SaveGameEventHandler(bool overwrite);

	/// <summary> Signal emitted when a game is loaded. </summary>
	[Signal]
	public delegate void LoadGameEventHandler(string filename);

	/// <summary> Signal emitted when the game is exited. </summary>
	[Signal]
	public delegate void ExitGameEventHandler();

	/// <summary> Signal emitted when the game menu is toggled. </summary>
	[Signal]
	public delegate void ToggleMenuEventHandler();

	/// <summary> Signal emitted when the load menu is opened. </summary>
	[Signal]
	public delegate void OpenLoadMenuEventHandler();

	/// <summary> Signal emitted when the save menu is opened. </summary>
	[Signal]
	public delegate void OpenSaveMenuEventHandler();

	// Mouse node signals

	/// <summary> Signal emitted when the mouse enters a tile body. </summary>
	[Signal]
	public delegate void MouseTileBodyEnteredEventHandler();

	/// <summary> Signal emitted when the mouse exits a tile body. </summary>
	[Signal]
	public delegate void MouseTileBodyExitedEventHandler();
}

