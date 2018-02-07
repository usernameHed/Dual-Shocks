using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

	private LineRenderer lineRenderer;

	void Start () {
		lineRenderer = GetComponent<LineRenderer>();
	}
	
	/// <summary>
    /// update la position des extrémités de la corde
    /// </summary>
	void Update () {
		lineRenderer.SetPosition (0, this.gameObject.transform.GetChild(0).position);
        lineRenderer.SetPosition (1, this.gameObject.transform.GetChild(1).position);
	}
}