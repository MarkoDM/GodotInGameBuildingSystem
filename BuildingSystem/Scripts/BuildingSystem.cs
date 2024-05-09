using System.Collections.Generic;

namespace Godot.GodotInGameBuildingSystem;

/// <summary>
/// This is the main class and represents a building system in a 3D environment.
/// This class handles object placement, demolition, and other related functionalities.
/// </summary>
public partial class BuildingSystem : Node3D
{
	#region Export Variables
	/// <summary> The main camera used in the building system. </summary>
	[ExportGroup("Dependancies")]
	[Export] public Camera3D MainCamera { get; set; }
	/// <summary> Gets or sets the library of buildable resources. </summary>
	/// <remarks> The <see cref="BuildableResourceLibrary"/> class represents a collection of <see cref="BuildableResource"/>. </remarks>
	[Export] public BuildableResourceLibrary BuildableObjectLibrary { get; set; }
	/// <summary> The size of each cell in the grid. </summary>
	[Export] public float CellSize { get; set; } = BSConstants.DEFAULT_CELL_SIZE;
	/// <summary> The height of each cell in the grid. </summary>
	[Export] public float CellHeight { get; set; } = BSConstants.DEFAULT_CELL_HEIGHT;
	/// <summary> The layer mask for the ground. </summary>
	[Export(PropertyHint.Layers3DPhysics)] public uint GroundLayerMask { get; set; } = BSConstants.DEFAULT_GROUND_LAYER_MASK;
	/// <summary> The layer mask for the floor. </summary>
	[Export(PropertyHint.Layers3DPhysics)]
	public uint FloorLayerMask { get; set; } = BSConstants.DEFAULT_FLOOR_LAYER_MASK;
	/// <summary> The layer mask for the walls. </summary>
	[Export(PropertyHint.Layers3DPhysics)] public uint WallLayerMask { get; set; } = BSConstants.DEFAULT_WALL_LAYER_MASK;
	/// <summary> The layer mask for free objects. </summary>
	[Export(PropertyHint.Layers3DPhysics)] public uint FreeLayerMask { get; set; } = BSConstants.DEFAULT_FREE_LAYER_MASK;
	/// <summary> The size of the grid in the X direction. </summary>
	[Export] public int XSize { get; set; } = BSConstants.DEFAULT_GRID_X_SIZE;
	/// <summary> The size of the grid in the Z direction. </summary>
	[Export] public int ZSize { get; set; } = BSConstants.DEFAULT_GRID_Z_SIZE;
	/// <summary> The number of levels in the grid. </summary>
	[Export] public int Levels { get; set; } = BSConstants.DEFAULT_GRID_LEVELS;
	/// <summary> The drag behavior for placing objects. </summary>
	[Export] public DragBehavior DragBehavior { get; set; } = DragBehavior.InstantPlacement;
	/// <summary> The threshold for delayed placement. </summary>
	[Export] public float DragThreshold { get; set; } = BSConstants.DEFAULT_DRAG_THRESHOLD;
	/// <summary> The offset of the drag visual from the ground. </summary>
	[Export] public float DragVisualGroundOffset { get; set; } = BSConstants.DEFAULT_DRAG_VISUAL_GROUND_OFFSET;

	/// <summary> Gets or sets a value indicating whether the ground mouse is visible. </summary>
	/// <value><c>true</c> if the ground mouse is visible; otherwise, <c>false</c>.</value>
	[ExportSubgroup("Debug")]
	[Export] public bool GroundMouseVisible { get; set; } = false;

	#endregion Export Variables

	#region Private Variables
	private uint _demolishLayerMask;
	private List<BuildableInstance> _freeObjectsList;
	private List<BuildingSystemGrid> _grids;
	private BuildingSystemGrid _activeGrid;
	private bool _isBuildModeActive = false;
	private bool _isDemolishModeActive = false;
	private BuildableResource _selectedObject = null;
	private Vector3 _currentMousePosition;
	private bool _gameStarted = false;
	private Vector3 _lastSnappedPosition = Vector3.Zero;
	private Vector3 _currentSnappedPosition = Vector3.Zero;
	private bool _isMousePressed = false;
	private int _currentRotation = 0;
	private uint _activeLayerMask;
	private Vector3 _mouseStartPosition;
	private Vector3 _mouseEndPosition;
	private MeshInstance3D _dragRectangleMesh;
	private Vector2 _mousePressPosition;
	private BSSaveSystem _saveSystem;

	#endregion Private Variables

	#region OnReady Varables

	private EventBus _eventBus;
	private Node3D _gridContainer;
	private Node3D _freeObjectsContainer;
	private Node3D _groundMouseNode;
	private MouseObject _mouseObject;
	private Control _objectMenu;
	private Control _mainMenu;
	private PackedScene _gridResource;
	private Node3D _demolishCollider = null;


	#endregion OnReady Varables

	#region Built-In Methods
	/// <inheritdoc/>
	public override void _Ready()
	{
		// Initialize the building system
		BSUtils.RegisterInputActions();
		_freeObjectsList = new();
		_grids = new List<BuildingSystemGrid>();
		_dragRectangleMesh = null;
		// Combine all layer masks for demolition
		_demolishLayerMask = FloorLayerMask | WallLayerMask | FreeLayerMask;
		// Set active layer mask to ground
		_activeLayerMask = GroundLayerMask;

		// Get all necessary nodes
		_gridContainer = GetNode<Node3D>("GridContainer");
		_freeObjectsContainer = GetNode<Node3D>("FreeObjectsContainer");
		_mouseObject = GetNode<MouseObject>("MouseObject");
		_eventBus = GetNode<EventBus>("EventBus");
		_gridResource = GD.Load<PackedScene>("res://BuildingSystem/scenes/building_system_grid.tscn");
		// For debug purposes
		_groundMouseNode = GetNode<Node3D>("GroundMouseDebug");
		// Get UI nodes
		_objectMenu = GetNode<Control>("UI/ObjectInterface");
		_mainMenu = GetNode<Control>("UI/MainMenuInterface");


		// Populate object menu and register UI events
		((ObjectMenu)_objectMenu).PopulateObjectGrid(BuildableObjectLibrary);
		_eventBus.SlotClicked += OnObjectMenuInteract;
		_eventBus.SaveGame += Save;
		_eventBus.LoadGame += Load;
		_eventBus.ExitGame += OnExitGameEvent;
		_eventBus.ToggleMenu += TogglePauseGame;

		// Initialize grids
		for (int i = 0; i < Levels; i++)
		{
			var gridInstance = _gridResource.Instantiate<BuildingSystemGrid>();
			_gridContainer.AddChild(gridInstance);
			gridInstance.Initialize(XSize, ZSize, CellSize, CellHeight, GroundLayerMask, FloorLayerMask, WallLayerMask);
			gridInstance.Name = $"Level{i}";
			Vector3 pos = new(0, i * CellHeight, 0);
			gridInstance.GlobalPosition = pos;
			_grids.Add(gridInstance);

		}

		// Set active grid to the first grid
		_activeGrid = _grids[0];
		_activeGrid.SetActive(true);

		// Initialize save system
		_saveSystem = new BSSaveSystem();

		_groundMouseNode.Visible = GroundMouseVisible;
	}

	/// <inheritdoc/>
	public override void _PhysicsProcess(double delta)
	{
		// When build mode is active we need to raycast to be able to snap objects to grid
		// As for demolition mode, this is usefull if you want to show visual indicator which object will be demolished
		// as is currentlly implemented. If you don't need it, just remove _isDemolishModeActive
		if (!_mainMenu.Visible && (_isBuildModeActive || _isDemolishModeActive))
		{
			// This is the most reliable place to cast a ray by Godot documentation
			var spaceState = GetWorld3D().DirectSpaceState;
			var mousePos = GetViewport().GetMousePosition();
			// Use global coordinates, not local to node
			var origin = MainCamera.ProjectRayOrigin(mousePos);
			var end = origin + MainCamera.ProjectRayNormal(mousePos) * 999f;
			var query = PhysicsRayQueryParameters3D.Create(origin, end, _activeLayerMask);
			var result = spaceState.IntersectRay(query);

			if (result.Count > 0)
			{
				_currentMousePosition = (Vector3)result["position"];
				if (_activeLayerMask == _demolishLayerMask)
				{
					if (IsInstanceValid(_demolishCollider))
					{
						_demolishCollider?.GetParent<BuildableInstance>()?.SetDemolitionView(false);
					}
					_demolishCollider = (Node3D)result["collider"];
					_demolishCollider.GetParent<BuildableInstance>()?.SetDemolitionView(true);
				}
			}
			else if (_activeLayerMask == _demolishLayerMask && _demolishCollider != null)
			{
				if (IsInstanceValid(_demolishCollider))
				{
					_demolishCollider?.GetParent<BuildableInstance>()?.SetDemolitionView(false);
				}
				_demolishCollider = null;
			}

			if (_activeLayerMask == GroundLayerMask)
			{
				UpdateMouseObjectVisual(delta);
			}
		}
	}

	/// <inheritdoc/>
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("toggle_menu")) TogglePauseGame();
		if (@event.IsActionPressed("build_mode")) SetBuildMode(!_isBuildModeActive);
		if (@event.IsActionPressed("demolish"))
		{
			SetDemolitionMode(!_isDemolishModeActive);
			if (_isDemolishModeActive && _selectedObject != null)
			{
				_selectedObject = null;
				ClearMouseObject();
			}
		}

		if (!_mainMenu.Visible)
		{
			HandleGridLevelChangeEvent(@event);
			HandleObjectRotationEvent(@event);

			HandleCancelEvent(@event);
		}

	}

	/// <inheritdoc/>
	public override void _UnhandledInput(InputEvent @event)
	{
		// This is here to prevent placing object when mouse is over UI elements
		HandleObjectPlacementEvent(@event);
		HandleObjectDemolishEvent(@event);
	}

	#endregion Built-In Methods

	#region Object Placing Logic

	private void TryToPlaceObject()
	{
		// Duplicate() can be used insted of new object instance, 
		// But because of not duplicating script with initialization parameter, 
		// which limits customization, it is not used.
		if (_selectedObject.SnapBehaviour != SnapBehaviour.Free)
		{
			// Handle object in grid as it is snapable
			_activeGrid.TryToPlaceObject(_selectedObject, _mouseObject.GetYRotationInDegrees(), _lastSnappedPosition);
		}
		else
		{
			// Handle not snappable (or free) objects here as they do not belong to the grid
			var buildableInstance = BuildableInstance.Create(_selectedObject, GetLayerMask(_selectedObject.SnapBehaviour));
			_freeObjectsContainer.AddChild(buildableInstance);
			_freeObjectsList.Add(buildableInstance);
			buildableInstance.GlobalPosition = new(_currentMousePosition.X, _activeGrid.GlobalPosition.Y, _currentMousePosition.Z);
			buildableInstance.RotateY(_mouseObject.GetYRotationInRadians());

			// Add simple small animation when placing objects
			AnimationUtils.AnimatePlacement(buildableInstance.ObjectInstance, this);
		}
	}

	#endregion Object Placing Logic

	#region Input Handlers

	private void HandleCancelEvent(InputEvent @event)
	{
		if (_selectedObject != null
			&& @event is InputEventMouseButton mouseEvent
			&& mouseEvent.ButtonIndex == MouseButton.Right
			&& mouseEvent.Pressed)
		{
			_selectedObject = null;
			ClearMouseObject();
		}
	}

	private void HandleObjectDemolishEvent(InputEvent @event)
	{
		if (
			_isDemolishModeActive
			&& _demolishCollider != null
			&& @event is InputEventMouseButton mouseEvent
			&& mouseEvent.ButtonIndex == MouseButton.Left
			&& mouseEvent.Pressed)
		{
			var mainNode = _demolishCollider.GetParent<BuildableInstance>();
			mainNode?.ClearObject();
		}
	}

	private void HandleObjectPlacementEvent(InputEvent @event)
	{
		if (_selectedObject == null || _isDemolishModeActive)
		{
			return;
		}

		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if (mouseEvent.Pressed)
			{
				if (DragBehavior == DragBehavior.None)
				{
					TryToPlaceObject();
				}
				else
				{
					_isMousePressed = true;
					_mousePressPosition = mouseEvent.Position;
					if (DragBehavior == DragBehavior.DelayedPlacement)
					{
						_mouseStartPosition = BSUtils.GetRaycastPoint(mouseEvent.Position, GetWorld3D(), MainCamera, _activeLayerMask);
					}
				}
			}
			else if (!mouseEvent.Pressed)
			{
				// Mouse button released
				if (_isMousePressed)
				{
					if (_mousePressPosition.DistanceTo(mouseEvent.Position) < DragThreshold)
					{
						// The mouse was not moved beyond the threshold, treat it as a click
						if (DragBehavior == DragBehavior.InstantPlacement)
						{
							TryToPlaceObject();
						}
					}
					if (DragBehavior == DragBehavior.DelayedPlacement)
					{
						CompleteDelayedDrag();
					}
					_isMousePressed = false;
				}
			}
		}
		else if (@event is InputEventMouseMotion mouseMotion && _isMousePressed && DragBehavior != DragBehavior.None)
		{
			if (_mousePressPosition.DistanceTo(mouseMotion.Position) >= DragThreshold)
			{
				// Mouse moved beyond the threshold, treat it as a drag
				if (DragBehavior == DragBehavior.InstantPlacement)
				{
					TryToPlaceObject();
				}
				else
				{
					// Update end position and redraw
					_mouseEndPosition = BSUtils.GetRaycastPoint(mouseMotion.Position, GetWorld3D(), MainCamera, _activeLayerMask);
					OnDragDelayed();
				}
			}

		}
	}

	private void HandleGridLevelChangeEvent(InputEvent @event)
	{
		if (@event.IsActionPressed("grid_level_up") || @event.IsActionPressed("grid_level_down"))
		{
			_activeGrid.SetActive(false);
			var modifier = @event.IsActionPressed("grid_level_up") ? 1 : -1;
			int nextSelectedGridIndex = (_grids.IndexOf(_activeGrid) + modifier) % _grids.Count;
			if (nextSelectedGridIndex < 0) nextSelectedGridIndex = _grids.Count - 1;
			_activeGrid.SetActive(false);
			_activeGrid = _grids[nextSelectedGridIndex];
			_activeGrid.SetActive(true);

			// Move camera
			var camera = MainCamera.GetParent() is CameraController ? MainCamera.GetParent<Node3D>() : MainCamera;
			Vector3 newCameraPosition = new(
				camera.GlobalPosition.X,
				_activeGrid.GlobalPosition.Y,
				camera.GlobalPosition.Z);
			camera.GlobalPosition = newCameraPosition;

			_eventBus.EmitSignal("LevelChanged", nextSelectedGridIndex);
		}
	}

	private void HandleObjectRotationEvent(InputEvent @event)
	{
		if (@event.IsActionPressed("rotate_object") && _mouseObject.HasMouseObject())
		{
			// To avoid loss of precision due to floating-point error rotation is reseted and remembered
			_currentRotation = _currentRotation == 270 ? 0 : _currentRotation + 90;
			_mouseObject.Rotate(_currentRotation);
		}
	}

	#endregion Input Handlers

	#region Save system

	private void SetBuildMode(bool value)
	{
		_isBuildModeActive = value;
		_objectMenu.Visible = _isBuildModeActive;
		if (!_isBuildModeActive) ClearMouseObject();
		_eventBus.EmitSignal("BuildModeChanged", _isBuildModeActive);
	}

	private void SetDemolitionMode(bool value)
	{
		_isDemolishModeActive = value;
		if (_isDemolishModeActive)
		{
			_activeLayerMask = _demolishLayerMask;
		}
		else
		{
			_activeLayerMask = GroundLayerMask;
			if (IsInstanceValid(_demolishCollider))
			{
				_demolishCollider?.GetParent<BuildableInstance>()?.SetDemolitionView(false);
			}
			_demolishCollider = null;
		}
		_eventBus.EmitSignal("DemolitionModeChanged", _isDemolishModeActive);
	}

	/// <summary> Saves the current state of the building system. </summary>
	/// <param name="overwrite">Whether to overwrite an existing save file.</param>
	public void Save(bool overwrite)
	{
		_saveSystem.Save(overwrite, _freeObjectsList, _grids);
	}

	/// <summary> Loads a building system from a file. </summary>
	/// <param name="filename">The name of the file to load.</param>
	public void Load(string filename)
	{
		Reset();
		_saveSystem.Load(filename, BuildableObjectLibrary, _freeObjectsList, _freeObjectsContainer, FreeLayerMask, _grids);
	}

	#endregion Save system

	#region Utility methods
	private uint GetLayerMask(SnapBehaviour type)
	{
		return type switch
		{
			SnapBehaviour.Ground => GroundLayerMask,
			SnapBehaviour.Wall => WallLayerMask,
			SnapBehaviour.Free => FreeLayerMask,
			_ => GroundLayerMask,
		};
	}
	private void OnDragDelayed()
	{
		// TODO: Complete implementation
		UpdateDragRectangle();
	}

	private void CompleteDelayedDrag()
	{
		// TODO: Implementation
		GD.Print("Drag completed");
	}

	private void UpdateDragRectangle()
	{
		if (_dragRectangleMesh == null)
		{
			_dragRectangleMesh = new();
			AddChild(_dragRectangleMesh);
		}
		// Create a PlaneMesh for the rectangle
		PlaneMesh planeMesh = new()
		{
			Size = new Vector2((_mouseEndPosition - _mouseStartPosition).X, (_mouseEndPosition - _mouseStartPosition).Z),
			CenterOffset = new Vector3((_mouseEndPosition.X + _mouseStartPosition.X) / 2, (_mouseEndPosition.Y + _mouseStartPosition.Y) / 2 + DragVisualGroundOffset, (_mouseEndPosition.Z + _mouseStartPosition.Z) / 2)
		};
		_dragRectangleMesh.Mesh = planeMesh;
	}

	private void UpdateMouseObjectVisual(double delta)
	{
		if (_isBuildModeActive && _mouseObject.HasMouseObject())
		{
			if (_selectedObject.SnapBehaviour != SnapBehaviour.Free)
			{
				//Snap to grid
				_lastSnappedPosition = _activeGrid.GetMouseSnappedPosition(_currentMousePosition, _mouseObject);
				_mouseObject.GlobalPosition = _mouseObject.GlobalPosition.Lerp(_lastSnappedPosition, (float)(delta * 15));
			}
			else
			{
				_mouseObject.GlobalPosition = new(_currentMousePosition.X, _activeGrid.GlobalPosition.Y, _currentMousePosition.Z);
			}

		}
		if (GroundMouseVisible) _groundMouseNode.GlobalPosition = new Vector3(_currentMousePosition.X, _currentMousePosition.Y, _currentMousePosition.Z);
	}

	private void Reset()
	{
		foreach (var freeObject in _freeObjectsList)
		{
			freeObject.ClearObject();
		}
		_freeObjectsList = new();
		foreach (var grid in _grids)
		{
			grid.ResetGrid();
		}
	}

	private void ClearMouseObject()
	{
		_currentRotation = 0;
		_mouseObject.ClearMouseObject();
	}

	#endregion Utility methods

	#region Event Handlers

	private void TogglePauseGame()
	{
		_mainMenu.Visible = !_mainMenu.Visible;
		// Godot pause can be used here if needed
		// GetTree().Paused = !GetTree().Paused;
	}
	private void OnNewGameEvent()
	{
		Reset();
		_mainMenu.Visible = !_mainMenu.Visible;
	}

	private void OnExitGameEvent()
	{
		GetTree().Quit();
	}

	private void OnObjectMenuInteract(int index, int button)
	{
		if (_isDemolishModeActive) SetDemolitionMode(false);
		_selectedObject = BuildableObjectLibrary.BuildableObjects[index];
		ClearMouseObject();
		_mouseObject.UpdateVisual(_selectedObject);
	}

	#endregion Event Handlers
}
