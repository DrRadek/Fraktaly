using Godot;
using System;

public partial class FractalsReset : Node
{
    [Export] PackedScene fractalsScene;

    public override void _Ready()
    {
        InstantiateScene();
    }
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("reset_fractals"))
        {
            GetChild(0).QueueFree();
            InstantiateScene();
        }

    }

    void InstantiateScene()
    {
        var instance = fractalsScene.Instantiate();
        AddChild(instance);
    }
}
