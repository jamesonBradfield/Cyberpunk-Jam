using Godot;
using System;
[Tool]
public static class Utils
{
    public static void AddNodeAsChild<T>(ref T type, String name, Godot.Node parent) where T : Godot.Node, new()
    {
        type = parent.GetNode<T>(name);
        if (type == null)
        {
            type = new();
            parent.AddChild(type);
            type.Name = name;
            type.Owner = type.GetTree().EditedSceneRoot;
        }
    }

}
