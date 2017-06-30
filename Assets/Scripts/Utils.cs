using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Utils
{
	public static int[] SuffleArray (int[] array)
	{
		for (int i = array.Length - 1; i > 0; i--)
		{
			int j = (int)Mathf.Floor(UnityEngine.Random.Range(0f, 1f) * (i + 1));
			int temp = array[i];
			array[i] = array[j];
			array[j] = temp;
		}
		return array;
	}
	public static float[] SuffleArray (float[] array)
	{
		for (int i = array.Length - 1; i > 0; i--)
		{
			int j = (int)Mathf.Floor(UnityEngine.Random.Range(0f, 1f) * (i + 1));
			float temp = array[i];
			array[i] = array[j];
			array[j] = temp;
		}
		return array;
	}

	public static void MapVertexUV (Mesh[] meshArray, int resolution)
	{
		int vertexIndex = 0;
		foreach (Mesh mesh in meshArray) {
			Vector2[] uvs2 = new Vector2[mesh.vertices.Length];
			for (int i = 0; i < uvs2.Length; ++i) {
				float x = vertexIndex % resolution;
				float y = Mathf.Floor(vertexIndex / (float)resolution);
				uvs2[i] = new Vector2(x, y) / (float)resolution;
				++vertexIndex;
			}
			mesh.uv2 = uvs2;
		}
	}

	public static int[] CreateIndices (int count)
	{
		int[] indices = new int[count];
		for (int i = 0; i < count; ++i) {
			indices[i] = i;
		}
		return indices;
	}

	public static void SaveMeshes (GameObject gameObject)
	{
#if UNITY_EDITOR
		string path = Application.dataPath + "/Meshes";
		if (!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}
		MeshFilter[] meshFilterArray = gameObject.GetComponentsInChildren<MeshFilter>();
		for (int index = 0; index < meshFilterArray.Length; ++index) {
			Mesh mesh = meshFilterArray[index].sharedMesh;
			AssetDatabase.CreateAsset(mesh, "Assets/Meshes/" + gameObject.name + "_" + index + ".asset");
			AssetDatabase.SaveAssets();
		}
		AssetDatabase.Refresh();
#endif
	}

	public static GameObject CreateGameObjectWithMesh (Mesh mesh, string name = "GeneratedMesh", Transform parent = null)
	{
		GameObject meshGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		GameObject.DestroyImmediate(meshGameObject.GetComponent<Collider>());
		meshGameObject.GetComponent<MeshFilter>().mesh = mesh;
		meshGameObject.name = name;
		meshGameObject.layer = parent.gameObject.layer;
		meshGameObject.transform.parent = parent;
		meshGameObject.transform.localPosition = Vector3.zero;
		meshGameObject.transform.localRotation = Quaternion.identity;
		meshGameObject.transform.localScale = Vector3.one;
		return meshGameObject;
	}

	public static T[] RandomizeArray<T> (T[] array)
	{
		for (int i = array.Length - 1; i > 0; --i) {
			int r = UnityEngine.Random.Range(0,i);
			T tmp = array[i];
			array[i] = array[r];
			array[r] = tmp;
		}
		return array;
	}

	public static Texture2D GetTextureFrom (Vector3[] array)
	{
		int dimension = (int)Utils.GetNearestPowerOfTwo(Mathf.Sqrt(array.Length));
		Color[] colorArray = new Color[dimension * dimension];
		for (int c = 0; c < array.Length; c++) {
			colorArray[c] = new Color(array[c].x, array[c].y, array[c].z);
		}
		Texture2D texture = new Texture2D(dimension, dimension, TextureFormat.RGBAFloat, false);
		texture.filterMode = FilterMode.Point;
		texture.SetPixels(colorArray);
		texture.Apply(false);
		return texture;
	}

	public static Texture2D GetTextureFrom (Color[] array)
	{
		int dimension = (int)Utils.GetNearestPowerOfTwo(Mathf.Sqrt(array.Length));
		Color[] colorArray = new Color[dimension * dimension];
		for (int c = 0; c < array.Length; c++) {
			colorArray[c] = array[c];
		}
		Texture2D texture = new Texture2D(dimension, dimension, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Point;
		texture.SetPixels(colorArray);
		texture.Apply(false);
		return texture;
	}

	public static Vector3 RotateX (Vector3 v, float t)
	{
		float cost = Mathf.Cos(t); float sint = Mathf.Sin(t);
		return new Vector3(v.x, v.y * cost - v.z * sint, v.y * sint + v.z * cost);
	}

	// http://stackoverflow.com/questions/12964279/whats-the-origin-of-this-glsl-rand-one-liner
	public static float Rand (Vector2 co)
	{
		return Utils.Frac(Mathf.Sin(Vector2.Dot(co, new Vector2(12.9898f,78.233f))) * 43758.5453f);
	}

	// http://stackoverflow.com/questions/8712044/is-there-an-equivalent-method-to-delphis-system-frac
	public static float Frac (float value)
	{
		return value - (int)value;
	}

	// hash based 3d value noise
	// function taken from https://www.shadertoy.com/view/XslGRr
	// Created by inigo quilez - iq/2013
	// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

	// ported from GLSL to HLSL
	public static float Hash ( float n )
	{
		return Utils.Frac(Mathf.Sin(n)*43758.5453f);
	}

	public static float NoiseIQ ( Vector3 x )
	{
	  // The noise function returns a value in the range -1.0f -> 1.0f
		float px = Mathf.Floor(x.x);
		float py = Mathf.Floor(x.y);
		float pz = Mathf.Floor(x.z);
		float fx = Utils.Frac(x.x);
		float fy = Utils.Frac(x.y);
		float fz = Utils.Frac(x.z);
		fx = fx*fx*(3f-2f*fx);
		fy = fy*fy*(3f-2f*fy);
		fz = fz*fz*(3f-2f*fz);
		float n = px + py*57f + 113f*pz;
		return Mathf.Lerp(Mathf.Lerp(Mathf.Lerp( Utils.Hash(n+0f), Utils.Hash(n+1f),fx),
			Mathf.Lerp( Utils.Hash(n+57f), Utils.Hash(n+58f),fx),fy),
		Mathf.Lerp(Mathf.Lerp( Utils.Hash(n+113f), Utils.Hash(n+114f),fx),
			Mathf.Lerp( Utils.Hash(n+170f), Utils.Hash(n+171f),fx),fy),fz);
	}

	public static Vector3 RandomVector (float min, float max)
	{
		return new Vector3(UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max));
	}

	public static float TriangleArea (Vector3 a, Vector3 b, Vector3 c)
	{
		return Vector3.Cross(a - b, a - c).magnitude / 2f;
	}
	
	public static Vector3 Vec3Lerp (Vector3 a, Vector3 b, Vector3 t)
	{
		return new Vector3(Mathf.Lerp(a.x, b.x, t.x), Mathf.Lerp(a.y, b.y, t.y), Mathf.Lerp(a.z, b.z, t.z));
	}
	
	public static Vector3 Vec3InverseLerp (Vector3 a, Vector3 b, Vector3 t)
	{
		return new Vector3(Mathf.InverseLerp(a.x, b.x, t.x), Mathf.InverseLerp(a.y, b.y, t.y), Mathf.InverseLerp(a.z, b.z, t.z));
	}

	// http://stackoverflow.com/questions/466204/rounding-up-to-nearest-power-of-2
	public static float GetNearestPowerOfTwo (float x)
	{
		return Mathf.Pow(2f, Mathf.Ceil(Mathf.Log(x) / Mathf.Log(2f)));
	}

	// Thank to Tomas Akenine-Möller
	// For sharing his Triangle Box Overlaping algorithm
	// http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/tribox3.txt

	public static int PlaneBoxOverlap (Vector3 normal, Vector3 vert, Vector3 maxbox)
	{
		Vector3 vmin = Vector3.zero;
		Vector3 vmax = Vector3.zero;
		if (normal.x > 0) { vmin.x = -maxbox.x - vert.x; vmax.x = maxbox.x - vert.x;	}
		else { vmin.x = maxbox.x - vert.x; vmax.x = -maxbox.x - vert.x; }
		if (normal.y > 0) { vmin.y = -maxbox.y - vert.y; vmax.y = maxbox.y - vert.y;	}
		else { vmin.y = maxbox.y - vert.y; vmax.y = -maxbox.y - vert.y; }
		if (normal.z > 0) { vmin.z = -maxbox.z - vert.z; vmax.z = maxbox.z - vert.z;	}
		else { vmin.z = maxbox.z - vert.z; vmax.z = -maxbox.z - vert.z; }
		Vector3 min = new Vector3(normal.x, normal.y, normal.z);
		Vector3 max = new Vector3(normal.x, normal.y, normal.z);
		if (Vector3.Dot(min, vmin) > 0) return 0;	
		if (Vector3.Dot(max, vmax) >= 0) return 1;	
		return 0;
	}

	public static int TriBoxOverlap (Vector3 boxcenter, Vector3 boxhalfsize, Vector3 a, Vector3 b, Vector3 c)
	{
		Vector3 v0 = Vector3.zero;
		Vector3 v1 = Vector3.zero;
		Vector3 v2 = Vector3.zero;
		float min, max, p0, p1, p2, rad, fex, fey, fez;
		Vector3 normal = Vector3.zero;
		Vector3 e0 = Vector3.zero;
		Vector3 e1 = Vector3.zero;
		Vector3 e2 = Vector3.zero;

		/* This is the fastest branch on Sun */
		/* move everything so that the boxcenter is in (0,0,0) */
		v0.x = a.x - boxcenter.x; v0.y = a.y - boxcenter.y; v0.z = a.z - boxcenter.z;
		v1.x = b.x - boxcenter.x; v1.y = b.y - boxcenter.y; v1.z = b.z - boxcenter.z;
		v2.x = c.x - boxcenter.x; v2.y = c.y - boxcenter.y; v2.z = c.z - boxcenter.z;
		/* compute triangle edges */
		e0.x = v1.x - v0.x; e0.y = v1.y - v0.y; e0.z = v1.z - v0.z;
		e1.x = v2.x - v1.x; e1.y = v2.y - v1.y; e1.z = v2.z - v1.z;
		e2.x = v0.x - v2.x; e2.y = v0.y - v2.y; e2.z = v0.z - v2.z;

		/* Bullet 3:  */
		/*  test the 9 tests first (this was faster) */
		fex = Mathf.Abs(e0.x);
		fey = Mathf.Abs(e0.y);
		fez = Mathf.Abs(e0.z);
		//
		p0 = e0.z * v0.y - e0.y * v0.z;
		p2 = e0.z * v2.y - e0.y * v2.z;
		if (p0 < p2) { min = p0; max = p2; } else { min = p2; max = p0; }  
		rad = fez * boxhalfsize.y + fey * boxhalfsize.z;
		if (min > rad || max < -rad) return 0;
		//
		p0 = -e0.z * v0.x + e0.x * v0.z;
		p2 = -e0.z * v2.x + e0.x * v2.z;	
		if(p0 < p2) { min = p0; max = p2; } else { min = p2; max = p0; }
		rad = fez * boxhalfsize.x + fex * boxhalfsize.z;
		if (min > rad || max < -rad) return 0;
		//
		p1 = e0.y * v1.x - e0.x * v1.y;
		p2 = e0.y * v2.x - e0.x * v2.y;
		if (p2 < p1) { min = p2; max = p1; } else { min = p1; max = p2; }
		rad = fey * boxhalfsize.x + fex * boxhalfsize.y;
		if (min > rad || max < -rad) return 0;
		//
		fex = Mathf.Abs(e1.x);
		fey = Mathf.Abs(e1.y);
		fez = Mathf.Abs(e1.z);
		//
		p0 = e1.z * v0.y - e1.y * v0.z;
		p2 = e1.z * v2.y - e1.y * v2.z;
		if (p0 < p2) { min = p0; max = p2; } else { min = p2; max = p0; }  
		rad = fez * boxhalfsize.y + fey * boxhalfsize.z;
		if (min > rad || max < -rad) return 0;
		//
		p0 = -e1.z * v0.x + e1.x * v0.z;
		p2 = -e1.z * v2.x + e1.x * v2.z;
		if (p0 < p2) { min = p0; max = p2; } else { min = p2; max = p0; }
		rad = fez * boxhalfsize.x + fex * boxhalfsize.z;
		if (min > rad || max < -rad) return 0;
		//	
		p0 = e1.y * v0.x - e1.x * v0.y;
		p1 = e1.y * v1.x - e1.x * v1.y;
		if (p0 < p1) { min = p0; max = p1; } else { min = p1; max = p0; }
		rad = fey * boxhalfsize.x + fex * boxhalfsize.y;
		if (min > rad || max < -rad) return 0;
		//
		fex = Mathf.Abs(e2.x);
		fey = Mathf.Abs(e2.y);
		fez = Mathf.Abs(e2.z);
		//
		p0 = e2.z * v0.y - e2.y * v0.z;
		p1 = e2.z * v1.y - e2.y * v1.z;
		if (p0 < p1) { min = p0; max = p1; } else { min = p1; max = p0; }
		rad = fez * boxhalfsize.y + fey * boxhalfsize.z;
		if (min > rad || max < -rad) return 0;
		//
		p0 = -e2.z * v0.x + e2.x * v0.z;
		p1 = -e2.z * v1.x + e2.x * v1.z;
		if (p0 < p1) { min = p0; max = p1; } else { min = p1; max = p0; }
		rad = fez * boxhalfsize.x + fex * boxhalfsize.z;
		if (min > rad || max < -rad) return 0;
		//
		p1 = e2.y * v1.x - e2.x * v1.y;
		p2 = e2.y * v2.x - e2.x * v2.y;
		if (p2 < p1) { min = p2; max = p1; } else { min = p1; max = p2;}
		rad = fey * boxhalfsize.x + fex * boxhalfsize.y;
		if (min > rad || max < -rad) return 0;

		/* Bullet 1: */
		/*  first test overlap in the {x,y,z}-directions */
		/*  find min, max of the triangle each direction, and test for overlap in */
		/*  that direction -- this is equivalent to testing a minimal AABB around */
		/*  the triangle against the AABB */
		/* test in X-direction */
		min = Mathf.Min(v0.x, Mathf.Min(v1.x, v2.x));
		max = Mathf.Max(v0.x, Mathf.Max(v1.x, v2.x));
		if (min > boxhalfsize.x || max < -boxhalfsize.x) return 0;
		/* test in Y-direction */
		min = Mathf.Min(v0.y, Mathf.Min(v1.y, v2.y));
		max = Mathf.Max(v0.y, Mathf.Max(v1.y, v2.y));
		if (min > boxhalfsize.y || max < -boxhalfsize.y) return 0;
		/* test in Z-direction */
		min = Mathf.Min(v0.z, Mathf.Min(v1.z, v2.z));
		max = Mathf.Max(v0.z, Mathf.Max(v1.z, v2.z));
		if (min > boxhalfsize.z || max < -boxhalfsize.z) return 0;
		/* Bullet 2: */
		/*  test if the box intersects the plane of the triangle */
		/*  compute plane equation of triangle: normal*x+d=0 */
		normal = Vector3.Cross(e0, e1);
		if ( 0 == Utils.PlaneBoxOverlap(normal, v0, boxhalfsize)) return 0;	
		return 1;   /* box and triangle overlaps */
	}

	// https://forum.unity3d.com/threads/gameobject-find-vs-transform-find-vs-transform-findchild.207877/
	public static Transform FindDescendentTransform(Transform searchTransform, string descendantName)
	{
		Transform result = null;

		int childCount = searchTransform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform childTransform = searchTransform.GetChild(i);

        // Not it, but has children? Search the children.
			if (childTransform.name != descendantName
				&& childTransform.childCount > 0)
			{
				Transform grandchildTransform = FindDescendentTransform(childTransform, descendantName);
				if (grandchildTransform == null)
				continue;

				result = grandchildTransform;
				break;
			}
        // Not it, but has no children?  Go on to the next sibling.
			else if (childTransform.name != descendantName
				&& childTransform.childCount == 0)
			{
				continue;
			}

        // Found it.
			result = childTransform;
			break;
		}

		return result;
	}
}