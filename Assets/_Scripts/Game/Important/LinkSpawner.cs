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
    /// <summary>
    /// gère les output aux players / balls..
    /// </summary>
    private void ReactionHandler(Collider other)
    {
        //est-ce que c'est une ball qui a touché ?
        Balls ballScript = other.transform.parent.gameObject.GetComponent<Balls>();
        if (ballScript)
        {
            ballScript.AddLink();
        }

        /*
        //sinon, est-ce que c'est un link ?
        Line line = other.transform.parent.parent.gameObject.GetComponent<Line>();
        if (line)
        {
            line.PlayerControllerVariable.RopeScript.AddLink();
        }
        */
    }

    /// <summary>
    /// gère les collisions
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!enabledObject)
            return;

        if (other.CompareTag("BallColliderLink"))
        {
            ReactionHandler(other);
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
