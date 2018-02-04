using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manage camera
/// Smoothly move camera to m_DesiredPosition
/// m_DesiredPosition is the barycenter of target list
/// </summary>
public class CameraController : MonoBehaviour
{
    #region Attributes

	// Time before next camera move
    [SerializeField]
	private float smoothTime = 0.2f;

	[SerializeField]
	private float minZoom = 4.0f;

	[SerializeField]
	private float maxZoom = 15.0f;

	// Zoom applied with only one target
	[SerializeField]
	private float defaultZoom = 6.0f;

	// Target approximation threshold
	[SerializeField]
	private float focusThreshold = 0f;

	// Border margin before unzoom
	[SerializeField]
	private float borderMargin = 4f;

	//Fallback target if target list is empty
	[SerializeField, Space(10)]
	private Transform fallBackTarget;

	//Target list
	[SerializeField, Space(10), Header("Debug")]
	private List<CameraTarget> targetList = new List<CameraTarget>();

	[SerializeField]
	private FrequencyTimer updateTimer;

	private Vector3 currentVelocity;
	private bool freezeCamera = false;
	private Vector3 averageTargetPosition;
	public Vector3 TargetPosition
	{
		get { return averageTargetPosition; }
	}

    #endregion

    #region Core
    /// <summary>
    /// Add target to camera
    /// </summary>
	public void AddTarget(CameraTarget other)
    {
		// Check object is not already a target
        if (targetList.IndexOf(other) < 0)
        {
            freezeCamera = false;
            targetList.Add(other);
        }
    }

    /// <summary>
    /// Remove target from target list
    /// </summary>
	public void RemoveTarget(CameraTarget other)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i].GetInstanceID() == other.GetInstanceID())
            {
                targetList.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// clean la list des targets
    /// </summary>
    private void CleanListTarget()
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            if (!targetList[i])
                targetList.RemoveAt(i);
        }
        freezeCamera = (targetList.Count == 0 && !fallBackTarget);
    }

    /// <summary>
    /// Clear targets
    /// </summary>
    public void ClearTarget()
    {
        targetList.Clear();
    }

    /// <summary>
    /// Calculate new camera position
    /// </summary>
    private void FindAveragePosition()
    {
		// Final position
        Vector3 averagePos = new Vector3();
		int activeTargetAmount = 0;
        float minX = 0; 
        float maxX = 0;
        float minY = 0;
        float maxY = 0;

        // For each target
        for (int i = 0; i < targetList.Count; i++)
        {
			CameraTarget target = targetList[i];

			// Check target is active
			if (!target || !target.gameObject.activeSelf)
			{
				continue;
			}

			// Set first target as min max position
            if (i == 0)
            {
                minX = maxX = target.transform.position.x;
                minY = maxY = target.transform.position.y;
            }
            else
            {
				// Extends min max bounds
                minX = (target.transform.position.x < minX) ? target.transform.position.x : minX;
                maxX = (target.transform.position.x > maxX) ? target.transform.position.x : maxX;
                minY = (target.transform.position.y < minY) ? target.transform.position.y : minY;
                maxY = (target.transform.position.y > maxY) ? target.transform.position.y : maxY;
            }
				
            activeTargetAmount++;
        }

        // Find middle point for all targets
        if (activeTargetAmount > 0)
        {
            averagePos.x = (minX + maxX) / 2.0F;
            averagePos.y = (minY + maxY) / 2.0F;
        }

        // If no targets, select fallback focus
        if (targetList.Count == 0)
        {
            if (fallBackTarget)
            {
                averagePos = fallBackTarget.position;
            }
        }

        // Calculate zoom
        float dist = Mathf.Max(Mathf.Abs(maxX - minX), Mathf.Abs(maxY - minY));
        averagePos.z = (targetList.Count > 1) ? -Mathf.Min(Mathf.Max(minZoom, dist + borderMargin), maxZoom) : -defaultZoom;

        // Change camera target
        averageTargetPosition = averagePos;
    }

    /// <summary>
    /// Initialize camera
    /// </summary>
    public void InitializeCamera()
    {
        FindAveragePosition();
        transform.position = averageTargetPosition;
    }

    /// <summary>
    /// Check camera is placed on target position
    /// </summary>
    /// <returns></returns>
    private bool HasReachedTargetPosition()
    {
		float x = transform.position.x;
		float y = transform.position.y;
       
		return x > averageTargetPosition.x - focusThreshold && x < averageTargetPosition.x + focusThreshold && y > averageTargetPosition.y - focusThreshold && y < averageTargetPosition.y + focusThreshold;
    }

	///////////////////////////////////////////////////////////////////////////////////////////////////////////// Unity functions

    private void Update()
    {
		freezeCamera = targetList.Count == 0 && fallBackTarget == null;
        

        if (updateTimer.Ready())
        {
            CleanListTarget();

            if (freezeCamera)
			{
				return;
			}

            FindAveragePosition();
        }
    }

    /// <summary>
	/// Smoothly move camera toward averageTargetPosition
    /// </summary>
    private void FixedUpdate()
    {
		if (freezeCamera || HasReachedTargetPosition ())
		{
			return;
		}

        // Move to desired position
		transform.position = Vector3.SmoothDamp(transform.position, averageTargetPosition, ref currentVelocity, smoothTime);
    }

    #endregion
}