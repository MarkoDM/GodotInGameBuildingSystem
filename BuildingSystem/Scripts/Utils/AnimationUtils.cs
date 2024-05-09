namespace Godot.GodotInGameBuildingSystem;

/// <summary> Utility class for animation-related functions. </summary>
public static class AnimationUtils
{
    /// <summary> Animates the placement of a Node3D by scaling it down and then back to its original scale. </summary>
    /// <param name="node">The Node3D to animate.</param>
    /// <param name="parent">The parent Node3D.</param>
    public static void AnimatePlacement(Node3D node, Node3D parent)
    {
        var originalScale = node.Scale;
        node.Scale *= 0.8f;
        var tween = parent.CreateTween();
        // tween.Finished += DoSomething;
        tween.TweenProperty(node, "scale", originalScale, 0.1).SetTrans(Tween.TransitionType.Bounce);//.SetEase(Tween.EaseType.Out);
        tween.Play();
    }
}
