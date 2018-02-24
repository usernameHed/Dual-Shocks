using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// ClampToCamera Description
/// </summary>
public class ClampToCamera : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("Clamp la position du player aux borders de la caméra"), SerializeField]
    private float borderMin = 0.1f;
    [FoldoutGroup("GamePlay"), Tooltip("Clamp la position du player aux borders de la caméra"), SerializeField]
    private float borderMax = 0.9f;


    [Tooltip("opti fps"), SerializeField]
	private FrequencyTimer updateTimer;

    #endregion

    #region Initialization

    private void Start()
    {
		// Start function
    }
    #endregion

    #region Core
    /// <summary>
    /// clamp la position du player dans la caméra !
    /// </summary>
    private void ClampPlayer()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, borderMin, borderMax);
        pos.y = Mathf.Clamp(pos.y, borderMin, borderMax);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
    #endregion

    #region Unity ending functions

    private void FixedUpdate()
    {
        ClampPlayer();
    }

    #endregion
}
