namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents a UI slot for displaying single buildable object. </summary>
public partial class Slot : PanelContainer
{
	private EventBus eventBus;
	private TextureRect textureRect;
	private Label label;

	/// <summary> Called when the node is ready. </summary>
	public override void _Ready()
	{
		textureRect = GetNode<TextureRect>("MarginContainer/TextureRect");
		label = GetNode<Label>("Name");
	}

	/// <summary> Called when a GUI input event occurs. </summary>
	/// <param name="event">The input event.</param>
	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed)
		{
			AcceptEvent();
			eventBus.EmitSignal("SlotClicked", GetIndex(), (int)(@event as InputEventMouseButton).ButtonIndex);
		}
	}

	/// <summary> Sets the slot data for the slot. </summary>
	/// <param name="slotData">The buildable resource data for the slot.</param>
	public void SetSlotData(BuildableResource slotData)
	{
		var size = "";
		switch (slotData.SnapBehaviour)
		{
			case SnapBehaviour.Wall:
				size = $"{slotData.Size.X} x {slotData.Size.Y}";
				break;
			case SnapBehaviour.Ground:
				size = $"{slotData.Size.X} x {slotData.Size.Z}";
				break;
			default:
				break;
		}
		textureRect.Texture = slotData.TextureAtlas ?? slotData.TextureImage;
		TooltipText = $"{slotData.Name}\n{slotData.Description}";
		label.Text = $"{slotData.Name}\n{size}";
	}

	/// <summary> Sets the event bus for the slot. </summary>
	/// <param name="bus">The event bus.</param>
	public void SetEventBus(EventBus bus)
	{
		eventBus = bus;
	}
}

