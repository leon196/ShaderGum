using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Paint : MonoBehaviour
{
	public GameObject cursor;
	RaycastHit hitInfo;
	Vector3 mousePos;
	Vector3 mousePosNew;
	Vector3 mousePosLast;
	float depth = 10f;
	float size = 2f;

	void Start ()
	{
		mousePosLast = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 2f));
		mousePosNew = mousePosLast;
		Camera.main.depthTextureMode |= DepthTextureMode.DepthNormals;
	}

	void Update ()
	{

		if (Input.GetMouseButton(0)) {
			Shader.SetGlobalFloat("_Paint", 1f);
		} else {
			Shader.SetGlobalFloat("_Paint", 0f);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out hitInfo)) {
      	depth = hitInfo.distance;
			}
		}

		mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth));
		mousePosNew = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth));

		if (Input.GetMouseButton(1) == false) {
			size = Mathf.Clamp(size - Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 100f, 0.1f, 40f);
		}

		Shader.SetGlobalVector("_PaintPoint", mousePos);
		Shader.SetGlobalVector("_PaintDrag", mousePosNew - mousePosLast);
		Shader.SetGlobalFloat("_PaintSize", size);

		if (cursor != null) {
			cursor.transform.position = mousePos;	
			cursor.transform.localScale = Vector3.one * size * 2f;	
		}

		mousePosLast = mousePosNew;
	}
}