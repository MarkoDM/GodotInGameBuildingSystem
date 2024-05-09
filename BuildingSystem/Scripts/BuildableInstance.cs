using System.Collections.Generic;

namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents an instance of a buildable object in the grid building system. </summary>
public partial class BuildableInstance : Node3D
{
	/// <summary> Gets the buildable resource associated with this instance. </summary>
	public BuildableResource BuildableResource { get; private set; }

	/// <summary> Gets the object instance of the buildable resource. </summary>
	public Node3D ObjectInstance { get; private set; }

	private StaticBody3D _body;
	private CollisionShape3D _collider;
	private MeshInstance3D _demolitionVisual;
	// We have a reference to cells this object is in.
	// This is an optimization, similar to Godot parent object, but here we have multiple parents.
	private List<GridCell> _cells;

	/// <summary> Creates a new instance of the <see cref="BuildableInstance"/> class with the specified buildable resource and layer mask. </summary>
	/// <remarks> This will instantiate the 3D object of the buildable resource and create a collider for it. </remarks>
	/// <param name="resource">The buildable resource.</param>
	/// <param name="layerMask">The layer mask.</param>
	/// <returns>The created <see cref="BuildableInstance"/>.</returns>
	public static BuildableInstance Create(BuildableResource resource, uint layerMask)
	{
		var instance = new BuildableInstance();
		instance.Initialize(resource, layerMask);
		return instance;
	}

	private void Initialize(BuildableResource resource, uint layerMask)
	{
		ObjectInstance = resource.Object3DModel.Instantiate<Node3D>();
		AddChild(ObjectInstance);
		BuildableResource = resource;
		_cells = new();
		CreateCollider(layerMask);
		CreateDemolishVisual();
	}

	/// <summary> Adds a grid cell to the buildable instance. </summary>
	/// <param name="cell">The grid cell to add.</param>
	public void AddCell(GridCell cell)
	{
		_cells.Add(cell);
	}

	/// <summary> Clears the buildable instance and removes it from the grid cells. </summary>
	public void ClearObject()
	{
		foreach (var cell in _cells)
		{
			if (BuildableResource.SnapBehaviour == SnapBehaviour.Ground)
			{
				cell.ClearGroundObject();
			}
			else
			{
				cell.ClearWallObject(this);
			}
		}
		_cells = new();
		ObjectInstance.QueueFree();
		QueueFree();
	}

	/// <summary> Sets the demolition view of the buildable instance. </summary>
	/// <param name="enabled">A value indicating whether the demolition view is enabled.</param>
	public void SetDemolitionView(bool enabled)
	{
		if (enabled)
		{
			_demolitionVisual.Visible = true;
			ObjectInstance.Visible = false;
		}
		else
		{
			_demolitionVisual.Visible = false;
			ObjectInstance.Visible = true;
		}
	}

	private void CreateCollider(uint layerMask)
	{
		var shape = new BoxShape3D
		{
			Size = GetSize()
		};

		_collider = new CollisionShape3D
		{
			Shape = shape
		};
		_body = new StaticBody3D
		{
			CollisionLayer = layerMask
		};
		_body.AddChild(_collider);

		AddChild(_body);
		// Optionally set floor colider offset based on the thickness of the floor
		Vector3 offset = BuildableResource.SnapBehaviour == SnapBehaviour.Wall ? new Vector3(0, (float)BuildableResource.Size.Y / 2, 0) : Vector3.Zero;
		_collider.Position = offset;
	}

	private void CreateDemolishVisual()
	{
		BoxMesh cubeMesh = new()
		{
			Size = GetSize()
		};

		StandardMaterial3D material = new()
		{
			AlbedoColor = new Color(Colors.DarkRed, 0.8f),
			Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
		};

		_demolitionVisual = new()
		{
			Mesh = cubeMesh,
			MaterialOverride = material
		};

		AddChild(_demolitionVisual);
		_demolitionVisual.Visible = false;
		// Same as collider
		Vector3 offset = BuildableResource.SnapBehaviour == SnapBehaviour.Wall ? new Vector3(0, (float)BuildableResource.Size.Y / 2, 0) : Vector3.Zero;
		_demolitionVisual.Position = offset;
	}

	private Vector3 GetSize()
	{
		var size = new Vector3(1, 1, 1);
		switch (BuildableResource.SnapBehaviour)
		{
			case SnapBehaviour.Ground:
				size = new Vector3(BuildableResource.Size.X, 0.2f, BuildableResource.Size.Z);
				break;
			case SnapBehaviour.Wall:
				size = new Vector3(BuildableResource.Size.X, BuildableResource.Size.Y, 0.2f);
				break;
			case SnapBehaviour.Free:
				{
					var meshInstance = BSUtils.FindMeshInstance(ObjectInstance);
					if (meshInstance != null)
					{
						Aabb aabb = meshInstance.Mesh.GetAabb();
						Vector3 freeSize = aabb.Size;
						GD.Print("Mesh size: " + freeSize);
						size = new Vector3(freeSize.X, freeSize.Y, freeSize.Z);
					}
				}
				break;
			default:
				break;
		}
		return size;
	}
}
