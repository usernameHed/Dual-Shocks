using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// LinkSpawner Description
/// </summary>
public class LinkSpawner : MonoBehaviour, IPooledObject, IKillable
{
    #region Attributes

    #endregion

    #region Initialization
    /// <summary>
    /// appelé lors du spawn de l'objet depuis la pool !
    /// </summary>
    public void OnObjectSpawn()
    {
        Debug.Log("active !!");
    }

    #endregion

    #region Core

    #endregion

    #region Unity ending functions
    [FoldoutGroup("Debug"), Button("Kill")]
    public void Kill()
    {
        //Debug.Log("linkSpawner desactive !");
        gameObject.SetActive(false);
    }
    #endregion
}
