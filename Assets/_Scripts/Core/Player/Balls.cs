using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Balls Description
/// </summary>
public class Balls : MonoBehaviour
{
    #region Attributes

	[FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
	private FrequencyTimer updateTimer;

    #endregion

    #region Initialization

    private void Start()
    {
		// Start function
    }
    #endregion

    #region Core

    #endregion

    #region Unity ending functions

    private void Update()
    {
        //optimisation des fps
        if (updateTimer.Ready())
        {

        }
    }

	#endregion
}
