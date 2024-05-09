using System.Linq;
namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents a library of <see cref="BuildableResource"/> </summary>
/// <remarks>
/// This class is used to store an array of buildable objects that can be built in the grid building system.
/// It provides an easy way of creating and managing buildable objects using multiple libraries(for testing or menus etc.).
/// </remarks>
[GlobalClass]
public partial class BuildableResourceLibrary : Resource
{
    /// <summary> Gets or sets the array of buildable objects. </summary>
    [Export]
    public BuildableResource[] BuildableObjects { get; set; }

    /// <summary> Initializes a new instance of the <see cref="BuildableResourceLibrary"/> class with an empty array of buildable objects. </summary>
    /// <remarks> Empty constructor required for Godot serialization. </remarks>
    public BuildableResourceLibrary() : this(new BuildableResource[99]) { }

    /// <summary> Initializes a new instance of the <see cref="BuildableResourceLibrary"/> class with the specified array of buildable objects. </summary>
    /// <param name="buildableObjects"> The array of buildable objects. </param>
    public BuildableResourceLibrary(BuildableResource[] buildableObjects)
    {
        BuildableObjects = buildableObjects;
    }

    /// <summary> Gets the buildable resource with the specified name. </summary>
    /// <param name="name"> The name of the buildable resource. </param>
    /// <returns> The buildable resource with the specified name, or null if not found. </returns>
    public BuildableResource GetByName(string name)
    {
        return BuildableObjects.FirstOrDefault(obj => obj.Name == name);
    }
}
