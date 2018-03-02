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


    private static TimeManager instance;
    public static TimeManager GetSingleton
    {
        get { return instance; }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// test si on met le script en UNIQUE
    /// </summary>
    public void SetSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Awake()
    {
        SetSingleton();
    }
    #endregion

    #region Core
    public void DoSlowMothion()
    {
        return;
        Time.timeScale = slowDonwFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
    #endregion

    #region Unity ending functions
    private void Update()
    {
        Time.timeScale += (1f / slowDonwLenght) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
    }
    #endregion
}
