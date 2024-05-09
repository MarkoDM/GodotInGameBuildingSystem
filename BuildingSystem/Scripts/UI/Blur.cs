namespace Godot.GodotInGameBuildingSystem;

/// <summary> Represents a custom control that applies a blur effect to its content. </summary>
public partial class Blur : ColorRect
{
	/// <summary> Called when the node is ready. </summary>
	public override void _Ready()
	{
		Size = GetViewportRect().Size;
		ShaderMaterial material = (ShaderMaterial)Material;
		material.SetShaderParameter("viewport_size", GetViewportRect().Size);
	}
}
