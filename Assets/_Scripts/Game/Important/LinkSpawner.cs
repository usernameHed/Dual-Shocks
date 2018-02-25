using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// LinkSpawner Description
/// </summary>
public class LinkSpawner : MonoBehaviour, IPooledObject, IKillable
{
    #region Attributes
    private bool enabledObject = true;
    #endregion

    #region Initialization
    /// <summary>
    /// appelé lors du spawn de l'objet depuis la pool !
    /// </summary>
    public void OnObjectSpawn()
    {
        Debug.Log("active !!");
        enabledObject = true;
    }

    #endregion

    #region Core
    private void OnTriggerEnter(Collider other)
    {
        if (!enabledObject)
            return;

        if (other.CompareTag("BallColliderLink"))
        {
            Debug.Log("Ici ajotue 1 au player :" + other.transform.parent.gameObject.name);
            Kill();
        }
    }
    #endregion

    #region Unity ending functions
    [FoldoutGroup("Debug"), Button("Kill")]
    public void Kill()
    {
        enabledObject = false;
        //Debug.Log("linkSpawner desactive !");
        gameObject.SetActive(false);
    }
    #endregion
}
