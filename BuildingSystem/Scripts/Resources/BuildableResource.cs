namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents a buildable resource in the grid building system. </summary>
/// <remarks> 
/// Every object that can be built in the grid building system needs to be a buildable resource.
/// Creating a buildable resource allows you to define the visual representation of the object, its size, snap behavior, and other properties.
/// </remarks>
[GlobalClass]
public partial class BuildableResource : Resource
{
    /// <summary> Gets or sets the name of the buildable resource. </summary>
    [Export]
    public string Name { get; set; }

    /// <summary> Gets or sets the description of the buildable resource. </summary>
    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; }

    /// <summary> Gets or sets the texture atlas for the buildable resource to be used in UI. </summary>
    /// <remarks>
    /// Use texture atlas or texture image. 
    /// Atlas has a priority over texture image.
    /// </remarks>
    [Export]
    public AtlasTexture TextureAtlas { get; set; }

    /// <summary> Gets or sets the texture image for the buildable resource to be used in UI. </summary>
    /// <remarks>
    /// Use texture atlas or texture image. 
    /// Atlas has a priority over texture image.
    /// </remarks>
    [Export]
    public Texture2D TextureImage { get; set; }

    /// <summary> Gets or sets the 3D model for the buildable resource. </summary>
    [Export]
    public PackedScene Object3DModel { get; set; }

    /// <summary> Gets or sets the snap behavior of the buildable resource. </summary>
    [Export]
    public SnapBehaviour SnapBehaviour { get; set; }

    /// <summary> Gets or sets the size of the buildable resource. </summary>
    /// <remarks>
    /// Size is only used for walls and floors.
    /// For walls, set X and Y and leave Z at 0.
    /// For floors, set X and Z and leave Y at 0.
    /// </remarks>
    [Export]
    public Vector3I Size { get; set; }

    /// <summary> Initializes a new instance of the <see cref="BuildableResource"/> class. </summary>
    /// <remarks> Empty constructor required for Godot serialization. </remarks>
    public BuildableResource() { }
}