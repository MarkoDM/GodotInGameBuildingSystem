namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents the user interface for displaying information about the game. </summary>
public partial class InfoInterface : Control
{
    [Export]
    public EventBus EventBus { get; set; }

    private Label _levelLabel;
    private Label _buildModeLabel;
    private Label _demolitionModeLabel;

    /// <summary> Called when the node is ready to be used. </summary>
    public override void _Ready()
    {
        _levelLabel = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/LevelLabel");
        _buildModeLabel = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/BuildModeLabel");
        _demolitionModeLabel = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/DemolitionModeLabel");

        EventBus.LevelChanged += OnLevelChanged;
        EventBus.BuildModeChanged += OnBuildModeChanged;
        EventBus.DemolitionModeChanged += OnDemolitionModeChanged;
    }

    /// <summary> Called when the level changes. </summary>
    /// <param name="level"> The new level. </param>
    public void OnLevelChanged(int level)
    {
        _levelLabel.Text = level.ToString();
    }

    /// <summary> Called when the build mode changes. </summary>
    /// <param name="enabled"> Whether build mode is enabled or disabled. </param>
    public void OnBuildModeChanged(bool enabled)
    {
        _buildModeLabel.Text = enabled ? "on" : "off";
    }

    /// <summary> Called when the demolition mode changes. </summary>
    /// <param name="enabled"> Whether demolition mode is enabled or disabled. </param>
    public void OnDemolitionModeChanged(bool enabled)
    {
        _demolitionModeLabel.Text = enabled ? "on" : "off";
    }
}
