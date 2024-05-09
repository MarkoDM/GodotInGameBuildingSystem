namespace Godot.GodotInGameBuildingSystem;
using System.Collections.Generic;

/// <summary> Utility class containing various helper methods for the building system. </summary>
public static class BSUtils
{
    /// <summary> Represents an event bus that allows communication between different parts of the application. </summary>
    /// <remarks> Place an EventBus at the top of the scene tree for optimization. </remarks>
    public static EventBus GetEventBus(Node node)
    {
        Node currentSceneRoot = node.GetTree().CurrentScene;
        var eventBus = currentSceneRoot.FindChild("EventBus", true, false);
        return eventBus as EventBus;
    }

    /// <summary> Gets position of the mouse cursor in the 3D world. </summary>
    /// <param name="mousePosition"></param>
    /// <param name="world"></param>
    /// <param name="camera"></param>
    /// <param name="layerMask"></param>
    /// <returns> Vector3 of intersection point. </returns>
    public static Vector3 GetRaycastPoint(Vector2 mousePosition, World3D world, Camera3D camera, uint layerMask)
    {
        var spaceState = world.DirectSpaceState;
        var origin = camera.ProjectRayOrigin(mousePosition);
        var end = origin + camera.ProjectRayNormal(mousePosition) * 999f;
        var query = PhysicsRayQueryParameters3D.Create(origin, end, layerMask);
        var result = spaceState.IntersectRay(query);

        if (result.ContainsKey("position"))
        {
            return (Vector3)result["position"];
        }

        return Vector3.Zero;
    }

    /// <summary> Finds the first MeshInstance3D child node of the specified parent node. </summary>
    /// <param name="parent">The parent node to search.</param>
    /// <returns>The first MeshInstance3D child node found, or null if none is found.</returns>
    public static MeshInstance3D FindMeshInstance(Node3D parent)
    {
        foreach (Node child in parent.GetChildren(true))
        {
            if (child is MeshInstance3D meshInstance)
            {
                return meshInstance;
            }
        }

        return null;
    }

    /// <summary> Registers the input actions for the grid building system. </summary>
    public static void RegisterInputActions()
    {
        // TODO - Load from configuration 
        var INPUT_ACTIONS = new Dictionary<string, Key>()
        {
            {"rotate_object", Key.R},
            {"build_mode", Key.B},
            {"grid_level_up", Key.Pageup},
            {"grid_level_down", Key.Pagedown},
            {"demolish", Key.Delete},
            {"toggle_menu", Key.Escape},
            {"quick_save", Key.F5},
            {"quick_load", Key.F9},
            {"move_forward", Key.W},
            {"move_backward", Key.S},
            {"move_left", Key.A},
            {"move_right", Key.D},
            {"rotate_left", Key.Q},
            {"rotate_right", Key.E},
        };

        foreach (var action in INPUT_ACTIONS)
        {
            InputMap.AddAction(action.Key);
            var inputKey = new InputEventKey
            {
                Keycode = action.Value
            };
            InputMap.ActionAddEvent(action.Key, inputKey);
        }
    }
}