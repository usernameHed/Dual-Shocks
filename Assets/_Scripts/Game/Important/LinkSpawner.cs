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
        if (other.CompareTag("Ball"))
        {
            //est-ce que c'est une ball qui a touché ?
            Balls ballScript = other.transform.GetComponent<Balls>();
            if (ballScript)
            {
                ballScript.AddLink();
            }
        }
        else if (other.CompareTag("Link"))
        {
            Link linkScript = other.GetComponent<Link>();
            if (linkScript)
            {
                linkScript.RopeScript.AddLink(linkScript.IdFromRope);
            }
        }
    }

    /// <summary>
    /// action du prefabs à la collision
    /// </summary>
    private void DoAction(Collider other)
    {
        ReactionHandler(other);
        GameObject bonusParticle = ObjectsPooler.GetSingleton.SpawnFromPool("BonusTaken", transform.position, Quaternion.identity, ObjectsPooler.GetSingleton.transform);
        GameObject scoreText = ObjectsPooler.GetSingleton.SpawnFromPool("BonusText", transform.position, Quaternion.identity, ObjectsPooler.GetSingleton.transform);
        scoreText.transform.GetChild(0).GetComponent<TextMesh>().text = "+1";
        SoundManager.GetSingleton.playSound("Bonus" + transform.GetInstanceID().ToString());
        Kill();
    }

    /// <summary>
    /// gère les collisions
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!enabledObject)
            return;

        if (other.CompareTag("Ball") || other.CompareTag("Link"))
        {
            DoAction(other);
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
