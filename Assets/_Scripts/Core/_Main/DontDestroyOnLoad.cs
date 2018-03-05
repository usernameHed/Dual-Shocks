using UnityEngine;

/// <summary>
/// DontDestroyOnLoad Description
/// </summary>
public class DontDestroyOnLoad : ISingleton<DontDestroyOnLoad>
{
    protected DontDestroyOnLoad() { } // guarantee this will be always a singleton only - can't use the constructor!

    #region Attributes

    #endregion

    #region Initialization

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    #endregion
}
