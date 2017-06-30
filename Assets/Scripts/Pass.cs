using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pass
{
	public Material material;
	public FrameBuffer buffer;
	public RenderTexture result;
	public int resolution;
	public Vector2 dimension;

	private FloatTexture _texture;
	public Texture texture { get { return _texture.texture; } }

	public Pass (Material materialToUse, Mesh[] meshArray)
	{
		material = materialToUse;
		_texture = new FloatTexture(meshArray);
		_texture.PrintEmpty();
		buffer = new FrameBuffer(_texture);
		resolution = _texture.resolution;
		dimension = _texture.dimension;
		Update();
	}

	public void Print (Mesh[] meshArray)
	{
		_texture.PrintPosition(meshArray);
		buffer.Print(_texture.texture);
	}

	public void Print (Vector3[] array)
	{
		_texture.PrintVectorArray(array);
		buffer.Print(_texture.texture);
	}

	public void Update ()
	{
		result = buffer.Apply(material);
	}

	public void SetTexture (string uniformName, Texture textureToUse)
	{
		material.SetTexture(uniformName, textureToUse);
	}

	public void SetVector (string uniformName, Vector3 vec3)
	{
		material.SetVector(uniformName, vec3);
	}

	public void SetFloat (string uniformName, float value)
	{
		material.SetFloat(uniformName, value);
	}

	public void SetMatrix (string uniformName, Matrix4x4 value)
	{
		material.SetMatrix(uniformName, value);
	}
}