using UnityEngine;
using System.Collections;

public class CameraToTexture : MonoBehaviour
{
	public string textureName = "_CameraTexture";
	public Material material;
	RenderTexture renderTexture;
	
	void Start ()
	{
		renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
		renderTexture.antiAliasing = 2;
		renderTexture.Create();
		GetComponent<Camera>().targetTexture = renderTexture;
		Shader.SetGlobalTexture(textureName, renderTexture);
		if (material != null) {
			material.mainTexture = renderTexture;
		}
	}

	void OnEnable ()
	{
		Shader.SetGlobalTexture(textureName, renderTexture);
	}
}