namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents the snap behavior of an object. </summary>
public enum SnapBehaviour
{
    /// <summary> Represents the ground object. </summary>
    Ground,
    /// <summary> Represents the wall object. </summary>
    Wall,
    /// <summary> Represents the free object. </summary>
    Free
}

/// <summary> Represents the drag behavior of an object. </summary>
public enum DragBehavior
{
    /// <summary> No drag behavior. </summary>
    None,
    /// <summary> Instantly places the object as the mouse is dragged. </summary>
    InstantPlacement,
    /// <summary> This will draw an area and place object after dragging is done. </summary>
    DelayedPlacement
}

/// <summary> Represents the sides of a 2D grid. </summary>
/// <remarks>
/// The sides are represented as follows:
/// - MinusZ: The negative Z side
/// - MinusX: The negative X side
/// - Z: The positive Z side
/// - X: The positive X side
/// </remarks>
public enum Side
{
    /// <summary> Represents the negative Z side of a 2D grid. </summary>
    MinusZ,
    /// <summary> Represents the negative X side of a 2D grid. </summary>
    MinusX,
    /// <summary> Represents the positive Z side of a 2D grid. </summary>
    Z,
    /// <summary> Represents the positive X side of a 2D grid. </summary>
    X
}
