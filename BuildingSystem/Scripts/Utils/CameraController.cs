namespace Godot.GodotInGameBuildingSystem;

/// <summary> Controls the movement and rotation of the camera in a 3D space. </summary>
/// <remarks> 
/// This is a very simple and basic camera controller.
/// Create a Node3D as a parent of Camera3D and attach this script to a parent node to enable camera movement and rotation. 
/// </remarks>
public partial class CameraController : Node3D
{
    /// <summary> The movement speed of the camera. </summary>
    [Export] public float MovementSpeed = 10.0f;
    /// <summary> The rotation speed of the camera. </summary>
    [Export] public float RotationSpeed = 2f;

    /// <inheritdoc/>
    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = new();

        // Handle forward and backward movements
        if (Input.IsActionPressed("move_forward"))
            velocity += -Transform.Basis.Z;
        if (Input.IsActionPressed("move_backward"))
            velocity += Transform.Basis.Z;

        // Handle strafe movements (left and right)
        if (Input.IsActionPressed("move_left"))
            velocity += -Transform.Basis.X;
        if (Input.IsActionPressed("move_right"))
            velocity += Transform.Basis.X;

        // Normalize and scale velocity
        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * MovementSpeed * (float)delta;
            GlobalTransform = new Transform3D(GlobalTransform.Basis, GlobalTransform.Origin + velocity);
        }

        // Handle rotations (Q for left, E for right)
        if (Input.IsActionPressed("rotate_left"))
            RotateY(RotationSpeed * (float)delta);
        if (Input.IsActionPressed("rotate_right"))
            RotateY(-RotationSpeed * (float)delta);
    }
}
