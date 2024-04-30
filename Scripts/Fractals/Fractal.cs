using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Fractal : RigidBody3D
{
	protected float size = 0;
	public int iteration = 0;
	public BodyEnteredEventHandler bodyEnteredEventHandler;

	public FractalGenerator fractalGenerator;

	public float Size { get => size; set => size = value; }

	public Fractal(float size, Vector3 pos, Vector3 rotation, Fractal parent, bool freeze = true) : this(freeze)
	{
	   
		parent.GetParent().AddChild(this);


		Size = size;
		GlobalPosition = pos;
		Rotation = rotation;
	}
	public Fractal(bool freeze = true) {
		//GravityScale = 0;
		Freeze = freeze;

	}

	public virtual void Generate(MultiMesh multiMesh, int index)
	{
		multiMesh.SetInstanceTransform(index, Transform.ScaledLocal(Vector3.One * size));
	}
	public abstract Mesh GenerateMesh(out Shape3D collisionShape, float startSize);
	public MultiMesh CreateMultiMeshInstance(Node parent, Mesh mesh, int instanceCount)
	{
		MultiMesh multiMesh = new MultiMesh();
		multiMesh.Mesh = mesh;

		// Set the format first.
		multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
		// Then resize (otherwise, changing the format is not allowed)
		multiMesh.InstanceCount = instanceCount;
		// Maybe not all of them should be visible at first.
		multiMesh.VisibleInstanceCount = instanceCount;

		MultiMeshInstance3D multiMeshInstance3D = new();
		multiMeshInstance3D.Multimesh = multiMesh;

		parent.AddChild(multiMeshInstance3D);
		return multiMesh;
	}

	public abstract List<Fractal> Divide(bool freeze = true);

	public void DivideAndAddColliders(Vector3 force)
	{
		fractalGenerator.DivideAndAddColliders(this, force);

	}
}
