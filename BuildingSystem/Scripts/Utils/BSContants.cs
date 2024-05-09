namespace Godot.GodotInGameBuildingSystem;

/// <summary> Contains constants used in the building system. </summary>
public static class BSConstants
{
    /// <summary> The default size of a cell. </summary>
    public const int DEFAULT_CELL_SIZE = 1;

    /// <summary> The default height of a cell. </summary>
    public const int DEFAULT_CELL_HEIGHT = 2;

    /// <summary> The default ground layer mask. </summary>
    public const uint DEFAULT_GROUND_LAYER_MASK = 1u << 1;

    /// <summary> The default floor layer mask. </summary>
    public const uint DEFAULT_FLOOR_LAYER_MASK = 1u << 2;

    /// <summary> The default wall layer mask. </summary>
    public const uint DEFAULT_WALL_LAYER_MASK = 1u << 3;

    /// <summary> The default free layer mask. </summary>
    public const uint DEFAULT_FREE_LAYER_MASK = 1u << 4;

    /// <summary> The default X size of the grid. </summary>
    public const int DEFAULT_GRID_X_SIZE = 200;

    /// <summary> The default Z size of the grid. </summary>
    public const int DEFAULT_GRID_Z_SIZE = 200;

    /// <summary> The default number of levels in the grid. </summary>
    public const int DEFAULT_GRID_LEVELS = 10;

    /// <summary> The default drag threshold. </summary>
    public const float DEFAULT_DRAG_THRESHOLD = 10f;

    /// <summary> The default drag visual ground offset. </summary>
    public const float DEFAULT_DRAG_VISUAL_GROUND_OFFSET = 0.2f;
}