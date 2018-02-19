using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// TimeManager Description
/// </summary>
public class TimeManager : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("slowDonwFactor"), SerializeField]
    private float slowDonwFactor = 0.05f;             //le type de ball (bleu, red...)
    [FoldoutGroup("GamePlay"), Tooltip("slowDonwFactor"), SerializeField]
    private float slowDonwLenght = 2f;             //le type de ball (bleu, red...)

    #endregion

    #region Initialization

    private void Start()
    {
		// Start function
    }
    #endregion

    #region Core
    private void DoSlowMothion()
    {
        Time.timeScale = slowDonwFactor;
    }
    #endregion

    #region Unity ending functions

	#endregion
}
