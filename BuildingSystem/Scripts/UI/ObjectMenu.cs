namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents the object menu in the grid building system. </summary>
public partial class ObjectMenu : Control
{
	private EventBus eventBus;
	private PackedScene slot;
	private HBoxContainer objectContainer;

	/// <summary> Called when the node is ready. </summary>
	public override void _Ready()
	{
		slot = GD.Load<PackedScene>("res://BuildingSystem/ui/slot.tscn");
		objectContainer = GetNode<HBoxContainer>("PanelContainer/MarginContainer/ScrollContainer/HBoxContainer");
		eventBus = BSUtils.GetEventBus(this);
	}
	/// <summary> Populates the object grid with buildable objects from the library. </summary>
	/// <param name="buildableObjectLibrary">The buildable object library.</param>
	public void PopulateObjectGrid(BuildableResourceLibrary buildableObjectLibrary)
	{
		var itemGridChildren = objectContainer.GetChildren();
		for (int i = 0; i < itemGridChildren.Count; i++)
		{
			itemGridChildren[i].QueueFree();
		}
		for (int i = 0; i < buildableObjectLibrary.BuildableObjects.Length; i++)
		{
			var slotInstance = slot.Instantiate();
			objectContainer.AddChild(slotInstance);
			// ((Slot)slotInstance).Connect("SlotClicked", new Callable(this, "OnObjectMenuInteract"));
			if (buildableObjectLibrary.BuildableObjects[i] != null)
			{
				((Slot)slotInstance).SetSlotData(buildableObjectLibrary.BuildableObjects[i]);
				((Slot)slotInstance).SetEventBus(eventBus);
			}
		}
	}
}
