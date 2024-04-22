using Godot;
using Godot.Collections;
using System;

public partial class Bullet : RigidBody3D
{
    double timeAlive = 0;
    double maxTimeAlive = 10;
    [Export] GpuParticles3D particles;
    public override void _Ready()
    {

        AddConstantForce(-20 * GlobalTransform.Basis.Z);
        //AddConstantTorque(GlobalTransform.Basis.Z * 10);

        GravityScale = 0;

        BodyEntered += Bullet_BodyEntered;
        MaxContactsReported = 1;
        ContactMonitor = true;
        

    }

    private void Bullet_BodyEntered(Node body)
    {
        ShapeCast3D shapeCast3D = new ShapeCast3D();
        shapeCast3D.Shape = new SphereShape3D();
        shapeCast3D.Scale = Vector3.One * 3;
        shapeCast3D.CollideWithBodies = true;
        
        AddChild(shapeCast3D);
        shapeCast3D.ForceShapecastUpdate();
        foreach (var result in shapeCast3D.CollisionResult)
        {

            var dict = new ShapeCastCollisionResultWrapper(result);//(result.AsGodotDictionary());
            var collider = dict.GetCollider();
            if(collider is Fractal)
            {
                var fractal = collider as Fractal;
                
                fractal.DivideAndAddColliders(GlobalPosition);
            }
            //GD.Print(dict.GetCollider());


            //GD.Print(result);
        }

        particles.Reparent(GetParent(), true);
        Timer timer = new Timer();
        timer.Autostart = true;
        timer.Timeout += Timer_Timeout;
        timer.WaitTime = 2;


        particles.Emitting = true;
        QueueFree();

    }

    private void Timer_Timeout()
    {
        particles.QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        timeAlive += delta;
        if(timeAlive > maxTimeAlive)
        {
            QueueFree();
        }
    }
}

class ShapeCastCollisionResultWrapper{
    Dictionary dict;
    public ShapeCastCollisionResultWrapper(Variant var)
    {
        dict = var.AsGodotDictionary();
    }
    public GodotObject GetCollider()
    {
        return dict["collider"].AsGodotObject();
    }
}
