namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents the user interface for loading game files. </summary>
public partial class LoadInterface : Control
{
	[Export]
	public EventBus EventBus { get; set; }
	private ItemList itemList;
	private Button loadButton;
	private Button closeButton;

	/// <summary> Called when the node is ready to be used. </summary>
	public override void _Ready()
	{
		EventBus.Connect("OpenLoadMenu", new Callable(this, "OnOpenLoadMenu"));
		itemList = GetNode<ItemList>("PanelContainer/MarginContainer/VBoxContainer/ItemList");
		itemList.Connect("item_activated", new Callable(this, "OnItemActivated"));

		loadButton = GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/Button");
		loadButton.Pressed += OnLoadButtonPressed;

		closeButton = GetNode<Button>("PanelContainer/CloseButton");
		closeButton.Pressed += OnCloseButtonPressed;
	}

	/// <summary> Event handler for the close button pressed event. </summary>
	private void OnCloseButtonPressed()
	{
		Visible = false;
	}

	/// <summary> Event handler for the load button pressed event. </summary>
	private void OnLoadButtonPressed()
	{
		var selectedItems = itemList.GetSelectedItems();
		if (selectedItems.Length > 0)
		{
			string fileName = itemList.GetItemText(selectedItems[0]);
			LoadGame(fileName);
		}
	}

	/// <summary> Opens the load menu and populates it with save files. </summary>
	public void OnOpenLoadMenu()
	{
		itemList.Clear();
		var saveFiles = SaveSystem.GetSaveFilesInfo();
		foreach (var file in saveFiles)
		{
			itemList.AddItem(file.Name);
		}

		Visible = true;
	}

	/// <summary> Event handler for the item activated event. </summary>
	/// <param name="index"> The index of the activated item. </param>
	public void OnItemActivated(int index)
	{
		string fileName = itemList.GetItemText(index);
		LoadGame(fileName);
	}

	/// <summary> Loads the game with the specified file name. </summary>
	/// <param name="fileName"> The name of the file to load. </param>
	private void LoadGame(string fileName)
	{
		EventBus.EmitSignal("LoadGame", fileName);
		EventBus.EmitSignal("ToggleMenu");
		Visible = false;
	}
}

