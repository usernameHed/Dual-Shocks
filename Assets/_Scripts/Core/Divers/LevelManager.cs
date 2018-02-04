using UnityEngine;

/// <summary>
/// LevelManager Description
/// </summary>
public class LevelManager : MonoBehaviour
{
    #region Attributes

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
