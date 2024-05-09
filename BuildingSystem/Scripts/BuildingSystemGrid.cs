using System;

namespace Godot.GodotInGameBuildingSystem;
/// <summary>
/// Represents a grid-based building system in a 3D space.
/// </summary>
public partial class BuildingSystemGrid : Node3D
{
	/// <summary>
	/// Gets or sets the array of grid cells in the building system.
	/// </summary>
	public GridCell[] GridCells { get; private set; }
	private int _xSize = 200;
	private int _zSize = 200;
	private float _cellSize = 1f;
	private float _cellHeight = 3f;
	private CollisionShape3D _collisionShape3D;
	private StaticBody3D _ground;
	private uint _floorLayerMask;
	private uint _wallLayerMask;
	private MeshInstance3D _gridVisual;

	/// <inheritdoc/>
	public override void _Ready()
	{
		_ground = GetNode<StaticBody3D>("GroundStaticBody3D");
		_collisionShape3D = GetNode<CollisionShape3D>("GroundStaticBody3D/Ground");
		_gridVisual = GetNode<MeshInstance3D>("GridVisual");
	}

	/// <summary> Initializes the building system grid with the specified parameters. </summary>
	/// <param name="xSize">The size of the grid along the X-axis.</param>
	/// <param name="zSize">The size of the grid along the Z-axis.</param>
	/// <param name="cellSize">The size of each grid cell.</param>
	/// <param name="cellHeight">The height of each grid cell.</param>
	/// <param name="groundLayerMask">The layer mask for the ground objects.</param>
	/// <param name="floorLayerMask">The layer mask for the floor objects.</param>
	/// <param name="wallLayerMask">The layer mask for the wall objects.</param>
	public void Initialize(
		int xSize,
		int zSize,
		float cellSize,
		float cellHeight,
		uint groundLayerMask,
		uint floorLayerMask,
		uint wallLayerMask
		)
	{
		_xSize = xSize;
		_zSize = zSize;
		_cellSize = cellSize;
		_cellHeight = cellHeight;
		GridCells = new GridCell[_xSize * _zSize];
		for (int i = 0; i < GridCells.Length; i++)
		{
			GridCells[i] = new GridCell();
		}

		_floorLayerMask = floorLayerMask;
		_wallLayerMask = wallLayerMask;
		_ground.CollisionLayer = groundLayerMask;

	}

	/// <summary> Sets the active state of the building system grid. </summary>
	/// <param name="isActive">The active state of the building system grid.</param>

	public void SetActive(bool isActive)
	{
		_collisionShape3D.Disabled = !isActive;
		_gridVisual.Visible = isActive;
	}

	/// <summary> Gets the global position of a cell in the grid. </summary>
	/// <param name="x">The X-coordinate of the cell.</param>
	/// <param name="z">The Z-coordinate of the cell.</param>
	/// <returns>The global position of the cell.</returns>
	public Vector3 GetGlobalPosition(int x, int z)
	{
		return new Vector3(x, 0, z) * _cellSize + GlobalPosition - new Vector3(_xSize / 2, 0, _zSize / 2);
	}

	/// <summary> Gets the global position of a cell in the grid. </summary>
	/// <param name="x">The X-coordinate of the cell.</param>
	/// <param name="z">The Z-coordinate of the cell.</param>
	/// <returns>The global position of the cell.</returns>
	public Vector3 GetCellGlobalPosition(int x, int z)
	{
		return new Vector3(x, 0, z) * _cellSize + GlobalPosition - new Vector3(_xSize / 2, 0, _zSize / 2) + new Vector3(_cellSize / 2, 0, _cellSize / 2); ;
	}

	/// <summary> Gets the grid position of a global position. </summary>
	/// <param name="globalPosition">The global position.</param>
	/// <returns>The grid position.</returns>
	public Vector2I GetGridPosition(Vector3 globalPosition)
	{
		int x = Mathf.FloorToInt((globalPosition.X - GlobalPosition.X) / _cellSize) + _xSize / 2;
		int z = Mathf.FloorToInt((globalPosition.Z - GlobalPosition.Z) / _cellSize) + _zSize / 2;
		return new Vector2I(x, z);
	}

	/// <summary> Gets the snapped position of the mouse on the grid for a buildable object. </summary>
	/// <param name="mousePosition">The position of the mouse.</param>
	/// <param name="mouseObject">The buildable object to snap to the grid.</param>
	/// <returns>The snapped position of the mouse on the grid.</returns>
	public Vector3 GetMouseSnappedPosition(Vector3 mousePosition, MouseObject mouseObject)
	{
		// As we are using layer mask to detect only ground/grid layer
		// there is no need for extra check whether the mouse is over the grid

		float x = Mathf.Round((mousePosition - GlobalPosition).X);
		float z = Mathf.Round((mousePosition - GlobalPosition).Z);

		float xOffset = 0;
		float zOffset = 0;

		if (mouseObject.HasXOffset || mouseObject.HasZOffset)
		{
			// We need to check object size and type
			// If size is not an even number, we need to add offset to snap at the mid point
			// instead of grid line/borders
			if (Math.Abs(mouseObject.GetYRotationInDegrees()) != 90f)
			{
				xOffset = mouseObject.HasXOffset ? _cellSize / 2 : 0;
				zOffset = mouseObject.HasZOffset ? _cellSize / 2 : 0;
			}
			else
			{
				xOffset = mouseObject.HasZOffset ? _cellSize / 2 : 0;
				zOffset = mouseObject.HasXOffset ? _cellSize / 2 : 0;
			}
			// Also keep snap point inside current cell
			if (xOffset != 0) xOffset *= x < (mousePosition - GlobalPosition).X ? 1 : -1;
			if (zOffset != 0) zOffset *= z < (mousePosition - GlobalPosition).Z ? 1 : -1;
		}

		return new Vector3(x + xOffset, 0, z + zOffset) + GlobalPosition;

	}

	/// <summary> Gets the grid cell at the specified coordinates. </summary>
	/// <param name="x">The X-coordinate of the cell.</param>
	/// <param name="y">The Y-coordinate of the cell.</param>
	/// <returns>The grid cell at the specified coordinates.</returns>
	public GridCell GetGridCell(int x, int y)
	{
		if (IsValidGridIndex(x, y))
		{
			int index = GetGridIndexWithoutValidation(x, y);
			return GridCells[index];
		}
		return null;
	}

	/// <summary> Checks if the grid index is valid. </summary>
	/// <param name="x">The X-coordinate of the index.</param>
	/// <param name="y">The Y-coordinate of the index.</param>
	/// <returns>True if the grid index is valid, false otherwise.</returns>
	public bool IsValidGridIndex(int x, int y) => x <= _xSize && y <= _zSize && x > 0 && y > 0;

	/// <summary> Tries to place an object on the grid. </summary>
	/// <param name="selectedObject">The object to be placed.</param>
	/// <param name="yRotation">The rotation of the object.</param>
	/// <param name="position">The position to place the object.</param>
	public void TryToPlaceObject(BuildableResource selectedObject, float yRotation, Vector3 position)
	{
		// As this is already placed and positioned properlly
		// for optimization, there will be no additional calculations and snaping to grid again

		// Get object size based on rotation
		int xLength = Math.Abs(yRotation) != 90f ? selectedObject.Size.X : selectedObject.Size.Z;
		int zLength = Math.Abs(yRotation) != 90f ? selectedObject.Size.Z : selectedObject.Size.X;

		// This is for the walls
		if (xLength == 0) xLength = 1;
		if (zLength == 0) zLength = 1;

		// Split by 2 so we can get start position
		var gridPosition = GetGridPosition(position);
		int xStart = gridPosition.X - xLength / 2;
		int zStart = gridPosition.Y - zLength / 2;

		// TODO - Handle edge case where wall is placed on max edge(no pun intended)

		// Without this check it can happen that by dragging or fast clicking
		// object gets placed on the existing one or overlap
		for (int i = xStart; i < xStart + xLength; i++)
		{
			for (int j = zStart; j < zStart + zLength; j++)
			{
				var gridCell = GetGridCell(i, j);
				if (selectedObject.SnapBehaviour == SnapBehaviour.Ground && gridCell?.HasGroundObject() == true) return;
				else if (selectedObject.SnapBehaviour == SnapBehaviour.Wall)
				{
					if (Math.Abs(yRotation) != 90f)
					{
						if (gridCell.HasWallObject(Side.MinusZ)) return;
					}
					else
					{
						if (gridCell.HasWallObject(Side.MinusX)) return;
					}
				}
			}
		}

		var buildableInstance = BuildableInstance.Create(selectedObject, selectedObject.SnapBehaviour == SnapBehaviour.Ground ? _floorLayerMask : _wallLayerMask);
		AddChild(buildableInstance);
		buildableInstance.GlobalPosition = position;
		buildableInstance.RotateY(Mathf.DegToRad(yRotation));

		// Add simple small animation when placing objects
		AnimationUtils.AnimatePlacement(buildableInstance.ObjectInstance, this);

		for (int i = xStart; i < xStart + xLength; i++)
		{
			for (int j = zStart; j < zStart + zLength; j++)
			{
				var gridCell = GetGridCell(i, j);
				if (selectedObject.SnapBehaviour == SnapBehaviour.Ground) gridCell.SetGroundObject(buildableInstance);
				else if (selectedObject.SnapBehaviour == SnapBehaviour.Wall)
				{
					if (Math.Abs(yRotation) != 90f)
					{
						gridCell.SetWallObject(buildableInstance, Side.MinusZ);
					}
					else
					{
						gridCell.SetWallObject(buildableInstance, Side.MinusX);
					}
				}
				buildableInstance.AddCell(gridCell);
				// CreateDebugMesh(GetGlobalPosition(i, j) + new Vector3(cellSize / 2, 0, cellSize / 2));
			}
		}
	}

	/// <summary> Resets the building system grid by clearing all objects from the grid cells. </summary>
	public void ResetGrid()
	{
		foreach (var cell in GridCells)
		{
			cell.GroundObject?.ClearObject();
			foreach (var wall in cell.WallObjects)
			{
				wall?.ClearObject();
			}
		}
	}

	// Leaving this for debugging purposes
	private void CreateDebugMesh(Vector3 position)
	{
		var meshInstance = new MeshInstance3D()
		{
			Name = $"DebugMesh{position}"
		};

		var capsuleMesh = new CapsuleMesh
		{
			Radius = 0.1f,
			Height = 0.5f
		};

		var material = new StandardMaterial3D
		{
			AlbedoColor = new Color(1.0f, 0.0f, 0.0f) // Set the color to red
		};

		capsuleMesh.Material = material;
		meshInstance.Mesh = capsuleMesh;

		AddChild(meshInstance);
		meshInstance.GlobalPosition = position;
	}

	private int GetGridIndexWithoutValidation(Vector2I gridPosition) => (gridPosition.X * _zSize) + gridPosition.Y;

	private int GetGridIndexWithoutValidation(int row, int column) => (row * _zSize) + column;
}
