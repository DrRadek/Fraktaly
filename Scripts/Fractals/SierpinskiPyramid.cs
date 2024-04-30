using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

public partial class SierpinskiPyramid : Fractal
{
	
	static List<Vector3> points = new List<Vector3>
	{
		new Vector3(0,0,0),
		new Vector3(1,0,0),
		new Vector3(1,0,1),
		new Vector3(0,0,1),
		new Vector3(0.5f,1,0.5f),
	};

	public SierpinskiPyramid(float size, Vector3 pos, Vector3 rotation, SierpinskiPyramid parent, bool freeze = true) : base(size, pos, rotation, parent, freeze)
	{

	}

	public SierpinskiPyramid()
	{

	}

	public override List<Fractal> Divide(bool freeze = true)
	{
		size /= 2;
		List<Fractal> fractals = new();
		for (int i = 1; i < points.Count; i++)
		{
			fractals.Add(new SierpinskiPyramid(size, ToGlobal(points[i] * size), Rotation, this, freeze)); 
		}

		return fractals;
	}

	public override Mesh GenerateMesh(out Shape3D collisionShape, float startSize)
	{
		List<Vector3> verts = new List<Vector3>();

		Vector3 startPos = Vector3.Zero;
		float scale = 1;

		// bottom
		verts.Add(startPos);
		verts.Add(startPos + points[2] * scale);
		verts.Add(startPos + points[1] * scale);

		verts.Add(startPos);
		verts.Add(startPos + points[3] * scale);
		verts.Add(startPos + points[2] * scale);


		// sides
		for (int i = 0; i < 4; i++)
		{
			int secondIndex = (i + 1) % 4;
			verts.Add(startPos + points[i] * scale);
			verts.Add(startPos + points[secondIndex] * scale);
			verts.Add(startPos + points[4] * scale);
		}

		// Array Mesh
		var surfaceArray = new Godot.Collections.Array();
		surfaceArray.Resize((int)Mesh.ArrayType.Max);

		var normals = new List<Vector3>();

		for (int i = 0; i < verts.Count; i += 3)
		{
			Vector3 A = verts[i + 1] - verts[i];
			Vector3 B = verts[i + 2] - verts[i];
			var normal = -A.Cross(B);

			normals.Add(normal);
			normals.Add(normal);
			normals.Add(normal);
		}
		var vertArray = verts.ToArray();
		surfaceArray[(int)Mesh.ArrayType.Vertex] = vertArray;
		//surfaceArray[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
		surfaceArray[(int)Mesh.ArrayType.Normal] = normals.ToArray();
		//surfaceArray[(int)Mesh.ArrayType.Index] = indices.ToArray();

		ArrayMesh mesh = new();
		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);

		collisionShape = new ConvexPolygonShape3D();
		(collisionShape as ConvexPolygonShape3D).Points = vertArray;

		return mesh;
	}
}
