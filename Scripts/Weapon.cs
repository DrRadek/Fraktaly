using Godot;
using System;

public partial class Weapon : Node
{
    [Export] PackedScene bullet;
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton)
        {
            var inputEvent = (InputEventMouseButton)@event;
            if (inputEvent.ButtonIndex == MouseButton.Left && inputEvent.IsPressed())
            {
                var parent = (Node3D)(GetParent());
                var scene = (Node3D)bullet.Instantiate();
                parent.AddChild(scene);
                scene.Reparent(parent.GetParent(), true);
            }
        }
    }
}
