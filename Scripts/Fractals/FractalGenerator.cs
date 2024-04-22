using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

public partial class FractalGenerator : Node3D
{
    [Export] Node3D storageNode;
    [Export] CSharpScript fractal;
    [Export] int iterations = 3;
    [Export] int maxIter = 3;
    [Export] int maxWeaponIter = 5;
    [Export] float startSize = 0.5f;
    [Export] bool addPhysics = false;
    MultiMesh multiMesh;
    Mesh mesh;
    Shape3D shape;
    List<Fractal> bodies = new();
    Queue<Fractal> fractals = new();

    bool hasPhysics = false;
    bool physicsAreFrozen = true;

    //List<RigidBody3D.BodyShapeEnteredEventHandler> eventHandlers = new();

    public override void _Ready()
    {
        if (storageNode == null)
            storageNode = this;


        Fractal mainFractal = (Fractal)fractal.New();
        mainFractal.Size = startSize;

        Queue<Fractal> fractals2 = new();
        fractals.Enqueue(mainFractal);

        storageNode.AddChild(mainFractal);

        for (int iter = 0; iter < iterations; iter++)
        {
            foreach (var ifractal in fractals) {

                foreach (Fractal IFractal2 in ifractal.Divide())
                {
                    fractals2.Enqueue(IFractal2);
                }
                fractals2.Enqueue(ifractal);
            }
            fractals.Clear();
            (fractals, fractals2) = (fractals2, fractals);
        }


        var fractal2 = fractals.Peek();
        mesh = fractal2.GenerateMesh(out shape, startSize);
             
        multiMesh = fractal2.CreateMultiMeshInstance(storageNode, mesh, fractals.Count);
        int ii = 0;
        foreach (var ifractal in fractals)
        {
            ifractal.Generate(multiMesh, ii);
            ii++;

            //CollisionShape3D collisionShape = new();

            //MeshInstance3D meshInstance3D = new MeshInstance3D();
            //var sphereMesh = new SphereMesh();
            //sphereMesh.Radius = 0.03f;

            //meshInstance3D.Mesh = sphereMesh;
            //meshInstance3D.Position = (ifractal as Node3D).Position;

            //storageNode.AddChild(meshInstance3D);
        }

        if(addPhysics)
            AddPhysics();

        //int i = 0;

        //foreach (var ifractal in fractals)
        //{
        //    CollisionShape3D collisionShape = new();
        //    collisionShape.Shape = shape;
        //    collisionShape.Scale = Vector3.One * ifractal.Size;

        //    ifractal.AddChild(collisionShape);

        //    bodies.Add(ifractal);

        //    int ii = i;

        //    RigidBody3D.BodyShapeEnteredEventHandler eventHandler = (bodyRid, body, bodyShaIndex, localShapeIndex) => Rb_BodyShapeEntered(bodyRid, body, bodyShaIndex, localShapeIndex, ii, iterations); //Rb_BodyShapeEntered(sender, args);
        //    ifractal.bodyShapeEnteredEventHandler = eventHandler;
        //    ifractal.BodyShapeEntered += eventHandler;

        //    ifractal.ContactMonitor = true;
        //    ifractal.MaxContactsReported = 1;
        //    i++;
        //}
        //i = 0;

    }

    public void AddPhysics()
    {
        if (hasPhysics)
            return;

        AddPhysics(fractals, iterations, 0);
        hasPhysics = true;
    }

    public void AddPhysicsGravity()
    {
        physicsAreFrozen = false;
        foreach (var rigidBody in bodies)
        {
            if (IsInstanceValid(rigidBody))
            {
                rigidBody.Freeze = false;
            }
        }
    }


    public void AddPhysics(IEnumerable<Fractal> fractals, int iteration, int startI, bool addEventHandler = false)
    {
        int i = startI;

        foreach (var fractal in fractals)
        {

            CollisionShape3D collisionShape = new();
            collisionShape.Shape = shape;
            collisionShape.Scale = Vector3.One * fractal.Size;

            fractal.AddChild(collisionShape);
            fractal.iteration = iteration + 1;
            fractal.fractalGenerator = this;


            if (addEventHandler) { 
                int count = i;
                //int iter = iteration + 1;
                RigidBody3D.BodyEnteredEventHandler eventHandler = (body) => Rb_BodyShapeEntered(body, fractal);
                fractal.bodyEnteredEventHandler = eventHandler;
                fractal.BodyEntered += eventHandler;
            }

            bodies.Add(fractal);


            fractal.ContactMonitor = true;
            fractal.MaxContactsReported = 1;
            i++;
        }
    }

    public void DivideAndAddColliders(Fractal thisFractal, Vector3 position)
    {
        int iteration = thisFractal.iteration;

        if (thisFractal == null || !IsInstanceIdValid(thisFractal.GetInstanceId()))
        {
            GD.Print("invalid");
            return;
        }

        var collider = thisFractal.GetChild(0) as CollisionShape3D;

        var posDelta2 = thisFractal.GlobalPosition - position;
        var strength2 = 30 / (posDelta2.LengthSquared() + 1);
        thisFractal.ApplyCentralImpulse(posDelta2.Normalized() * strength2);

        if (iteration >= maxWeaponIter)
        {
            return;
        }

        var newBodies = thisFractal.Divide(physicsAreFrozen);

        multiMesh.InstanceCount += newBodies.Count;
        multiMesh.VisibleInstanceCount = multiMesh.InstanceCount;

        AddPhysics(newBodies, iteration, bodies.Count, false);

        foreach (var body in newBodies )
        {
            var posDelta = body.GlobalPosition - position;
            var strength = 30 / (posDelta.LengthSquared() + 1);
            body.ApplyCentralImpulse(posDelta.Normalized() * strength);
        }

        collider.Scale = Vector3.One * thisFractal.Size;
        thisFractal.LinearVelocity = Vector3.Zero;
        thisFractal.AngularVelocity = Vector3.Zero;
        thisFractal.iteration += 1;

    }

    void DivideAndAddColliders(Fractal thisFractal)
    {
        int iteration = thisFractal.iteration;

        if (thisFractal == null || !IsInstanceIdValid(thisFractal.GetInstanceId()))
        {
            GD.Print("invalid");
            return;
        }

        var collider = thisFractal.GetChild(0) as CollisionShape3D;


        if (iteration == maxIter)
        {
            //thisFractal.QueueFree();

            //multiMesh.InstanceCount -=1;
            //multiMesh.VisibleInstanceCount = multiMesh.InstanceCount;
            //bodies.Remove(thisFractal);

            //thisFractal.QueueFree();
            //return;

            //if(thisFractal.bodyEnteredEventHandler != null)
            //{
                thisFractal.BodyEntered -= thisFractal.bodyEnteredEventHandler;
                thisFractal.bodyEnteredEventHandler = null;
            //}
            
            return;
        }

        var newBodies = thisFractal.Divide();

        multiMesh.InstanceCount += newBodies.Count;
        multiMesh.VisibleInstanceCount = multiMesh.InstanceCount;

        thisFractal.BodyEntered -= thisFractal.bodyEnteredEventHandler;


        //int iter = iteration + 1;

        thisFractal.bodyEnteredEventHandler =
            (body) => Rb_BodyShapeEntered(body, thisFractal); //Rb_BodyShapeEntered(sender, args);
        thisFractal.BodyEntered += thisFractal.bodyEnteredEventHandler;
        thisFractal.iteration += 1;

        AddPhysics(newBodies, iteration, bodies.Count);
        foreach (var fractal in newBodies)
        {
            //bodies.Add(fractal);
            //fractal.ApplyImpulse(new Vector3(0.1f, 1, 0.1f) * 10);
        }

        collider.Scale = Vector3.One * thisFractal.Size;

        //thisFractal.GravityScale = -0.02f;
        thisFractal.LinearVelocity = Vector3.Zero;
        thisFractal.AngularVelocity = Vector3.Zero;

        //thisFractal.QueueFree();

    }

    //public void DivideAndAddColliders(int rbIndex)
    //{
    //    DivideAndAddColliders(bodies[rbIndex]);
    //}

    private void Rb_BodyShapeEntered(Node body, Fractal fractal)
    {
        if (fractal.bodyEnteredEventHandler is null)
        {
            GD.Print("event handler doesn't exist");
            return;
        }

        if (fractal.GetSignalConnectionList("body_entered").Count == 0)
        {
            GD.Print("???");
            return;
        }

        DivideAndAddColliders(fractal);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("add_physics"))
        {
            AddPhysicsGravity();
        }

        int index = 0;
        foreach (var rigidBody in bodies)
        {
            if (IsInstanceValid(rigidBody))
            {
                rigidBody.Generate(multiMesh, index);
            }

            index++;
        }
    }
}
