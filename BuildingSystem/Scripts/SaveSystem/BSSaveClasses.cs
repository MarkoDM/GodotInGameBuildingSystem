using System.Collections.Generic;

namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents a save object. </summary>
public class SaveObject
{
    /// <summary> Gets or sets the name of the object. </summary>
    public string Name { get; set; }

    /// <summary> Gets or sets the resource path of the object. </summary>
    public string ResourcePath { get; set; }

    /// <summary> Gets or sets the Y rotation in radians of the object. </summary>
    public float YRotationRadiants { get; set; }
}

/// <summary> Represents a save object placed on a grid. </summary>
public class SaveGridObject : SaveObject
{
    /// <summary> Gets or sets the position of the object on the grid. </summary>
    public GridPosition Position { get; set; }
}

/// <summary> Represents a save object placed freely in the scene. </summary>
public class SaveFreeObject : SaveObject
{
    /// <summary> Gets or sets the position of the object in the scene. </summary>
    public FreePosition Position { get; set; }
}

/// <summary> Represents a position on a grid. </summary>
public class GridPosition
{
    /// <summary> Gets or sets the X coordinate of the position. </summary>
    public float X { get; set; }

    /// <summary> Gets or sets the Z coordinate of the position. </summary>
    public float Z { get; set; }
}

/// <summary> Represents a position in the scene. </summary>
public class FreePosition
{
    /// <summary> Gets or sets the X coordinate of the position. </summary>
    public float X { get; set; }

    /// <summary> Gets or sets the Y coordinate of the position. </summary>
    public float Y { get; set; }

    /// <summary> Gets or sets the Z coordinate of the position. </summary>
    public float Z { get; set; }
}

/// <summary> Represents a save grid. </summary>
public class SaveGrid
{
    /// <summary> Gets or sets the index of the grid. </summary>
    public int Index { get; set; }

    /// <summary> Gets or sets the list of objects placed on the grid. </summary>
    public List<SaveGridObject> Objects { get; set; }
}

/// <summary> Represents a save file. </summary>
public class SaveFile
{
    /// <summary> Gets or sets the list of grids in the save file. </summary>
    public List<SaveGrid> Grids { get; set; }

    /// <summary> Gets or sets the list of free objects in the save file. </summary>
    public List<SaveFreeObject> FreeObjects { get; set; }
}
