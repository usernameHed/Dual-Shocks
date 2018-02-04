using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExt
{
	/// <summary>
	/// Remove every childs in a transform
	/// </summary>
	public static Transform ClearChild(this Transform transform)
	{
		foreach (Transform child in transform)
		{
			GameObject.Destroy(child.gameObject);
		}
		return (transform);
	}
}
