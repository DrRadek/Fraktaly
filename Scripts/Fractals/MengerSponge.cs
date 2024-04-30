using Godot;
using System;
using System.Collections.Generic;

public partial class MengerSponge : Fractal
{
	public MengerSponge(float size, Vector3 pos, Vector3 rotation, MengerSponge parent, bool freeze = true) : base(size, pos, rotation, parent, freeze)
	{
	}
	public MengerSponge()
	{

	}

	public override List<Fractal> Divide(bool freeze = true)
	{
		size /= 3;
		Position -= GetOffset();

		List<Fractal> fractals = new();

		for (int x = 0; x < 3; x++)
		{
			for (int y = 0; y < 3; y++)
			{
				for (int z = 0; z < 3; z++)
				{
					if ((x == 1 && z == 1) || (x == 1 && y == 1) || (y == 1 && z == 1) || (x == 0 && z == 0 && y == 0))
					{
						continue;
					}
					fractals.Add(new MengerSponge(size, ToGlobal(new Vector3(x, y, z) * size), Rotation, this, freeze));
				}
			}
		}

		return fractals;
	}

	public Vector3 GetOffset()
	{
		return Vector3.One * size;
	}

	public override Mesh GenerateMesh(out Shape3D collisionShape, float startSize)
	{
		var boxMesh = new BoxMesh();

		boxMesh.Size = Vector3.One;

		BoxShape3D shape3D = new BoxShape3D();
		shape3D.Size = boxMesh.Size;
		collisionShape = shape3D;

		(GetParent() as Node3D).Position += Vector3.One * startSize / 2;

		return boxMesh;
	}
}
