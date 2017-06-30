using UnityEngine;
using System.Collections;

public class VertexToTexture : MonoBehaviour 
{
	public Material material;
	public Material materialPaint;
	private Pass pass;
	private Renderer render;

	void Start ()
	{
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		Mesh[] meshes = new Mesh[meshFilters.Length];
		for (int i = 0; i < meshFilters.Length; ++i) {
			meshes[i] = meshFilters[i].sharedMesh;
		}
		pass = new Pass(materialPaint, meshes);
		pass.Print(meshes);

		render = GetComponent<Renderer>();
		render.material = material;
	}

	void Update ()
	{
		pass.SetTexture("_OriginTexture", pass.texture);
		pass.SetVector("_TransformPosition", transform.position);
		pass.SetMatrix("_RendererMatrix", render.localToWorldMatrix);
		pass.SetMatrix("_InverseMatrix", render.worldToLocalMatrix);
		pass.Update();
		material.SetTexture("_VertexTexture", pass.result);
		material.SetMatrix("_RendererMatrix", render.localToWorldMatrix);
		material.SetVector("_TransformPosition", transform.position);

		// debug
		Shader.SetGlobalTexture("_"+gameObject.name+"VertexTexture", pass.result);
	}
}