using System.Collections.Generic;

namespace Godot.GodotInGameBuildingSystem;

/// <summary> Provides extension methods for saving and converting positions and objects in the grid building system. </summary>
public static class SaveExtensions
{
    /// <summary> Converts a <see cref="GridPosition"/> to a <see cref="Vector3"/> with the specified Y coordinate. </summary>
    /// <param name="position"> The grid position to convert. </param>
    /// <param name="y"> The Y coordinate of the resulting vector. </param>
    /// <returns> A <see cref="Vector3"/> representing the converted position. </returns>
    public static Vector3 ToVector3(this GridPosition position, float y)
    {
        return new Vector3(position.X, y, position.Z);
    }

    /// <summary> Converts a <see cref="Vector3"/> to a <see cref="GridPosition"/>. </summary>
    /// <param name="position"> The vector position to convert. </param>
    /// <returns> A <see cref="GridPosition"/> representing the converted position. </returns>
    public static GridPosition ToGridPosition(this Vector3 position)
    {
        return new GridPosition()
        {
            X = position.X,
            Z = position.Z
        };
    }

    /// <summary> Converts a <see cref="FreePosition"/> to a <see cref="Vector3"/>. </summary>
    /// <param name="position"> The free position to convert. </param>
    /// <returns> A <see cref="Vector3"/> representing the converted position. </returns>
    public static Vector3 ToVector3(this FreePosition position)
    {
        return new Vector3(position.X, position.Y, position.Z);
    }

    /// <summary> Converts a <see cref="Vector3"/> to a <see cref="FreePosition"/>. </summary>
    /// <param name="position"> The vector position to convert. </param>
    /// <returns> A <see cref="FreePosition"/> representing the converted position. </returns>
    public static FreePosition ToFreePosition(this Vector3 position)
    {
        return new FreePosition()
        {
            X = position.X,
            Y = position.Y,
            Z = position.Z
        };
    }

    /// <summary> Converts a <see cref="BuildableInstance"/> to a <see cref="SaveGridObject"/>. </summary>
    /// <param name="buildableObject"> The buildable object to convert. </param>
    /// <returns> A <see cref="SaveGridObject"/> representing the converted object. </returns>
    public static SaveGridObject ToSaveGridObject(this BuildableInstance buildableObject)
    {
        return new SaveGridObject
        {
            Name = buildableObject.BuildableResource.Name,
            ResourcePath = buildableObject.BuildableResource.ResourcePath,
            Position = buildableObject.GlobalPosition.ToGridPosition(),
            YRotationRadiants = buildableObject.RotationDegrees.Y
        };
    }

    /// <summary> Converts a <see cref="BuildableInstance"/> to a <see cref="SaveFreeObject"/>. </summary>
    /// <param name="buildableObject"> The buildable object to convert. </param>
    /// <returns> A <see cref="SaveFreeObject"/> representing the converted object. </returns>
    public static SaveFreeObject ToSaveFreeObject(this BuildableInstance buildableObject)
    {
        return new SaveFreeObject
        {
            Name = buildableObject.BuildableResource.Name,
            ResourcePath = buildableObject.BuildableResource.ResourcePath,
            Position = buildableObject.ObjectInstance.Position.ToFreePosition(),
            YRotationRadiants = buildableObject.ObjectInstance.Rotation.Y
        };
    }

    /// <summary> Converts a <see cref="BuildingSystemGrid"/> to a <see cref="SaveGrid"/>. </summary>
    /// <param name="grid"> The building system grid to convert. </param>
    /// <returns> A <see cref="SaveGrid"/> representing the converted grid. </returns>
    public static SaveGrid ToSaveGrid(this BuildingSystemGrid grid)
    {
        List<SaveGridObject> saveObjects = new();
        foreach (var gridCell in grid.GridCells)
        {
            if (gridCell.GroundObject != null) saveObjects.Add(gridCell.GroundObject.ToSaveGridObject());
            foreach (var wall in gridCell.WallObjects)
            {
                if (wall != null) saveObjects.Add(wall.ToSaveGridObject());
            }
        }
        SaveGrid saveGrid = new()
        {
            Objects = saveObjects
        };
        return saveGrid;
    }
}

