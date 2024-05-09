namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents the save interface in the UI. </summary>
public partial class SaveInterface : Control
{
	[Export]
	public EventBus EventBus { get; set; }
	private Button newButton;
	private Button overwriteButton;
	private Button closeButton;

	/// <summary> Called when the node enters the scene tree for the first time. </summary>
	public override void _Ready()
	{
		EventBus.Connect("OpenSaveMenu", new Callable(this, "OnOpenSaveMenu"));
		newButton = GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/New");
		newButton.Pressed += OnNewButtonPressed;
		overwriteButton = GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/Overwrite");
		overwriteButton.Pressed += OnOverwriteButtonPressed;
		closeButton = GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/CloseButton");
		closeButton.Pressed += OnCloseButtonPressed;
	}
	/// <summary> Opens the save menu. </summary>
	public void OnOpenSaveMenu()
	{
		Visible = true;
	}
	/// <summary> Handles the button press event for the "New" button. </summary>
	private void OnNewButtonPressed()
	{
		EventBus.EmitSignal("SaveGame", false);
		Visible = false;
	}
	/// <summary> Handles the button press event for the "Overwrite" button. </summary>
	private void OnOverwriteButtonPressed()
	{
		EventBus.EmitSignal("SaveGame", true);
		Visible = false;
	}
	/// <summary> Handles the button press event for the "Close" button. </summary>
	private void OnCloseButtonPressed()
	{
		Visible = false;
	}
}

