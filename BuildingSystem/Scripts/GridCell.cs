namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents a cell in a grid-based building system. </summary>
public partial class GridCell
{
    /// <summary> Gets the ground object placed on this grid cell. </summary>
    public BuildableInstance GroundObject { get; private set; }

    /// <summary> Gets the wall objects placed on this grid cell. </summary>
    public BuildableInstance[] WallObjects { get; private set; }

    /// <summary> Initializes a new instance of the <see cref="GridCell"/> class. </summary>
    public GridCell()
    {
        GroundObject = null;
        WallObjects = new BuildableInstance[2];
    }

    /// <summary> Sets the ground object on this grid cell. </summary>
    /// <param name="buildableInstance">The buildable instance representing the ground object.</param>
    public void SetGroundObject(BuildableInstance buildableInstance)
    {
        if (buildableInstance.BuildableResource.SnapBehaviour == SnapBehaviour.Ground)
        {
            GroundObject = buildableInstance;
        }
    }

    /// <summary> Clears the ground object from this grid cell. </summary>
    public void ClearGroundObject()
    {
        GroundObject = null;
    }

    /// <summary> Sets the wall object on this grid cell. </summary>
    /// <param name="buildableInstance">The buildable instance representing the wall object.</param>
    /// <param name="side">The <see cref="Side"/>  of the grid cell where the wall object is placed.</param>
    public void SetWallObject(BuildableInstance buildableInstance, Side side)
    {
        if (buildableInstance.BuildableResource.SnapBehaviour == SnapBehaviour.Wall)
        {
            WallObjects[(int)side] = buildableInstance;
        }
    }

    /// <summary> Clears the specified wall object from this grid cell. </summary>
    /// <param name="wall">The wall object to clear.</param>
    public void ClearWallObject(BuildableInstance wall)
    {
        for (int i = 0; i < WallObjects.Length; i++)
        {
            if (WallObjects[i] == wall)
            {
                WallObjects[i] = null;
            }
        }
    }

    /// <summary> Clears the wall object from the specified side of this grid cell. </summary>
    /// <param name="side">The <see cref="Side"/> of the grid cell where the wall object is placed.</param>
    public void ClearWallObject(Side side)
    {
        WallObjects[(int)side] = null;
    }

    /// <summary> Determines whether this grid cell has a ground object. </summary>
    /// <returns><c>true</c> if this grid cell has a ground object; otherwise, <c>false</c>.</returns>
    public bool HasGroundObject() => GroundObject != null;

    /// <summary> Determines whether this grid cell has a wall object on the specified side. </summary>
    /// <param name="side">The <see cref="Side"/> of the grid cell to check.</param>
    /// <returns><c>true</c> if this grid cell has a wall object on the specified side; otherwise, <c>false</c>.</returns>
    public bool HasWallObject(Side side) => WallObjects[(int)side] != null;

    /// <summary> Gets the wall object placed on the specified side of this grid cell.</summary>
    /// <param name="side">The <see cref="Side"/> of the grid cell to get the wall object from.</param>
    /// <returns>The wall object placed on the specified side of this grid cell.</returns>
    public BuildableInstance GetWallObject(Side side)
    {
        return WallObjects[(int)side];
    }
}
