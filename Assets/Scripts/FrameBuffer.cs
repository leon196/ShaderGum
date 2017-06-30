using UnityEngine;
using System;
using System.Collections;

public class FrameBuffer
{
	RenderTexture[] textures;
	int currentTexture;
	int width, height;
	Vector2 dimension;

	public FrameBuffer (int width_, int height_,
		int count = 2,
		RenderTextureFormat format = RenderTextureFormat.ARGB32,
		FilterMode filterMode = FilterMode.Bilinear,
		TextureWrapMode wrapMode = TextureWrapMode.Clamp)
	{
		currentTexture = 0;
		textures = new RenderTexture[count];
		width = width_;
		height = height_;
		for (int i = 0; i < textures.Length; ++i) {
			if (textures[i]) {
				textures[i].Release();
			}
			textures[i] = new RenderTexture(width, height, 24, format, RenderTextureReadWrite.Linear);
			textures[i].Create();
			textures[i].filterMode = filterMode;
			textures[i].wrapMode = wrapMode;
		}
		dimension = new Vector2(width, height);
	}

	public FrameBuffer (FloatTexture floatTexture)
	{
		currentTexture = 0;
		textures = new RenderTexture[2];
		width = floatTexture.resolution;
		height = floatTexture.resolution;
		for (int i = 0; i < textures.Length; ++i) {
			if (textures[i]) {
				textures[i].Release();
			}
			textures[i] = new RenderTexture(width, height, 24, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
			textures[i].Create();
			textures[i].filterMode = FilterMode.Point;
			textures[i].wrapMode = TextureWrapMode.Clamp;
		}
		Print(floatTexture.texture);
		dimension = new Vector2(width, height);
	}

	public RenderTexture Apply (Material material)
	{
		material.SetVector("_Resolution", dimension);
		RenderTexture buffer = this.Get();
		this.Swap();
		Graphics.Blit(buffer, this.Get(), material);
		return this.Get();
	}

	public void Print (Texture2D input)
	{
		Graphics.Blit(input, this.Get());
		this.Swap();
		Graphics.Blit(input, this.Get());
	}

	public void Swap ()
	{
		currentTexture = (currentTexture + 1) % textures.Length;
	}

	public RenderTexture Get ()
	{
		return textures[currentTexture];
	}

	public RenderTexture GetLast ()
	{
		return textures[(currentTexture + 2) % textures.Length];
	}

	public RenderTexture GetNext ()
	{
		return textures[(currentTexture + 1) % textures.Length];
	}

	public FilterMode GetFilterMode ()
	{
		if (textures[0]) {
			return textures[0].filterMode;
		} else {
			return FilterMode.Bilinear;
		}
	}

	public void SetFilterMode (FilterMode filterMode)
	{
		for (int i = 0; i < textures.Length; ++i) {
			if (textures[i]) {
				textures[i].filterMode = filterMode;
			}
		}
	}
}