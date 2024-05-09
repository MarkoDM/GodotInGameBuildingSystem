using System.Collections.Generic;

namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents a save system for the grid building system. </summary>
public class BSSaveSystem
{
    private string _currentSaveFile = string.Empty;

    /// <summary> Initializes a new instance of the <see cref="BSSaveSystem"/> class. </summary>
    public BSSaveSystem() { }

    /// <summary> Saves the current state of the grid building system. </summary>
    /// <param name="overwrite"> Whether to overwrite the existing save file. </param>
    /// <param name="freeObjectList"> The list of free objects to save. </param>
    /// <param name="grids"> The list of grids to save. </param>
    public void Save(bool overwrite, List<BuildableInstance> freeObjectList, List<BuildingSystemGrid> grids)
    {
        List<SaveGrid> saveGrids = new();
        List<SaveFreeObject> freeObjects = new();

        foreach (var freeObject in freeObjectList)
        {
            freeObjects.Add(freeObject.ToSaveFreeObject());
        }

        for (int i = 0; i < grids.Count; i++)
        {
            var grid = grids[i].ToSaveGrid();
            if (grid.Objects.Count > 0)
            {
                grid.Index = i;
                saveGrids.Add(grid);
            }
        }

        SaveFile saveFile = new()
        {
            Grids = saveGrids,
            FreeObjects = freeObjects
        };

        _currentSaveFile = overwrite ? SaveSystem.Save(saveFile, _currentSaveFile) : SaveSystem.Save(saveFile);
    }

    /// <summary> Loads a saved state of the grid building system. </summary>
    /// <param name="filename"> The name of the save file to load. </param>
    /// <param name="buildableObjectLibrary"> The buildable object library. </param>
    /// <param name="freeObjectList"> The list of free objects to load. </param>
    /// <param name="freeObjectContainer"> The container node for free objects. </param>
    /// <param name="freeLayerMask"> The layer mask for free objects. </param>
    /// <param name="grids"> The list of grids to load. </param>
    public void Load(
        string filename,
        BuildableResourceLibrary buildableObjectLibrary,
        List<BuildableInstance> freeObjectList,
        Node3D freeObjectContainer,
        uint freeLayerMask,
        List<BuildingSystemGrid> grids
    )
    {
        _currentSaveFile = filename;
        var saveFile = SaveSystem.Load<SaveFile>(filename);

        if (saveFile != null)
        {
            for (int i = 0; i < saveFile.Grids.Count; i++)
            {
                if (saveFile.Grids[i].Index == i)
                {
                    foreach (var gridObject in saveFile.Grids[i].Objects)
                    {
                        var buildableObject = buildableObjectLibrary.GetByName(gridObject.Name);
                        grids[i].TryToPlaceObject(buildableObject, gridObject.YRotationRadiants, gridObject.Position.ToVector3(i));
                    }
                }
            }
        }

        foreach (var freeSaveObject in saveFile.FreeObjects)
        {
            var buildableObject = buildableObjectLibrary.GetByName(freeSaveObject.Name);
            var buildableInstance = BuildableInstance.Create(buildableObject, freeLayerMask);
            freeObjectContainer.AddChild(buildableInstance);
            buildableInstance.GlobalPosition = freeSaveObject.Position.ToVector3();
            buildableInstance.RotateY(Mathf.DegToRad(freeSaveObject.YRotationRadiants));

            freeObjectList.Add(buildableInstance);
        }
    }
}

