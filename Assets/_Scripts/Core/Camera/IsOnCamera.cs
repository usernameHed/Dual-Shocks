using UnityEngine;

/// <summary>
/// Check object is on camera
/// <summary>
public class IsOnCamera : MonoBehaviour
{
	[SerializeField]
	private FrequencyTimer updateTimer = new FrequencyTimer(1.0F);

	[SerializeField]
	private float xMargin;

	[SerializeField]
	private float yMargin;

	public bool isOnScreen = false;
	private Renderer objectRenderer;

	#region Initialization
	private void Awake()
	{
		objectRenderer = GetComponent<Renderer> ();
	}
	#endregion

    #region Core

	/// <summary>
	/// Check object is on screen
	/// <summary>
	bool CheckOnCamera()
	{
		if (!Camera.main)
		{
			return false;
		}

		Vector3 bottomCorner = Camera.main.WorldToViewportPoint(gameObject.transform.position - objectRenderer.bounds.extents);
		Vector3 topCorner = Camera.main.WorldToViewportPoint(gameObject.transform.position + objectRenderer.bounds.extents);

		return topCorner.x >= -xMargin && bottomCorner.x <= 1 + xMargin && topCorner.y >= -yMargin && bottomCorner.y <= 1 + yMargin;
	}

	// Unity functions

    private void Update()
    {
		if (updateTimer.Ready())
        {
			isOnScreen = CheckOnCamera();
        }
    }
	#endregion
}
