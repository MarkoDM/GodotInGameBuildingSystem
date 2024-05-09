namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents a tile that is a part of mouse object visual. </summary>
/// <remarks> 
/// Mouse tile has two states - normal and colliding. When the tile is colliding with another body, it changes its color to red. 
/// This is the place to update the visual representation of the tile when it is colliding with another body.
/// </remarks>
public partial class MouseTile : Node3D
{
	private MeshInstance3D _meshInstance;
	private Area3D _area;
	private CollisionShape3D _collisionShape3D;
	private EventBus _eventBus;

	private int _colideCount = 0;
	private StandardMaterial3D _overrideMaterial;
	private uint _layerMask = 1u << 2;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
		_area = GetNode<Area3D>("Area3D");
		_collisionShape3D = GetNode<CollisionShape3D>("Area3D/CollisionShape3D");
		_eventBus = BSUtils.GetEventBus(this);
		_area.BodyEntered += (Node3D body) => AreaBodyEntered(body, _area);
		_area.BodyExited += (Node3D body) => AreaBodyExited(body, _area);
		// Create a basic material to override the tile color when colliding
		_overrideMaterial = new StandardMaterial3D
		{
			AlbedoColor = new Color(Colors.DarkRed, 0.8f), // Initial color white
			Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
			RenderPriority = 2
		};
	}

	/// <summary> Sets the collision layer mask for the tile. </summary>
	/// <param name="layerMask"> The layer mask to set. </param>
	public void SetLayerMask(uint layerMask) => _area.CollisionMask = layerMask;

	/// <summary> Sets the size of the tile. </summary>
	/// <param name="size"> The size of the tile. </param>
	public void SetSize(int size)
	{
		var planeMesh = _meshInstance.Mesh as PlaneMesh;
		planeMesh.Size = new Vector2(size, size);
		var shape = _collisionShape3D.Shape as BoxShape3D;
		shape.Size = new Vector3(size * 0.8f, 0.3f, size * 0.8f);
	}

	/// <summary> Sets the visibility of the tile. </summary>
	/// <param name="visible"> Whether the tile should be visible or not. </param>
	public void SetVisibility(bool visible)
	{
		_meshInstance.Visible = visible;
	}

	/// <summary> Checks if the tile is currently colliding with any other bodies. </summary>
	/// <returns> True if the tile is colliding, false otherwise. </returns>
	public bool IsColliding() => _colideCount != 0;

	private void AreaBodyEntered(Node3D body, Area3D area)
	{
		_colideCount++;
		_meshInstance.MaterialOverride ??= _overrideMaterial;
		_eventBus.EmitSignal("MouseTileBodyEntered");
	}

	private void AreaBodyExited(Node3D body, Area3D area)
	{
		_colideCount--;
		if (_colideCount == 0)
		{
			_meshInstance.MaterialOverride = null;
			_eventBus.EmitSignal("MouseTileBodyExited");
		}
	}
}

