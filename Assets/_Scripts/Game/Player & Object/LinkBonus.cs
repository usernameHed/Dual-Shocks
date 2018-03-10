using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// LinkSpawner Description
/// </summary>
public class LinkBonus : MonoBehaviour, IPooledObject, IKillable
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

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.GameOver, StopAction);
    }

    #endregion

    #region Core
    /// <summary>
    /// gère les output aux players / balls..
    /// </summary>
    private void ReactionHandler(Collider other)
    {
        if (other.CompareTag(GameData.Prefabs.Ball.ToString()))
        {
            //est-ce que c'est une ball qui a touché ?
            Balls ballScript = other.transform.GetComponent<Balls>();
            if (ballScript)
            {
                ballScript.AddLink();
            }
        }
        else if (other.CompareTag(GameData.Prefabs.Link.ToString()))
        {
            Rope rope = other.gameObject.GetComponentInParents<Rope>();
            Link linkScript = other.GetComponent<Link>();
            if (rope && linkScript)
            {
                rope.AddLink(linkScript.IdFromRope);
            }
        }
    }

    /// <summary>
    /// action du prefabs à la collision
    /// </summary>
    private void DoAction(Collider other)
    {
        ReactionHandler(other);
        ObjectsPooler.Instance.SpawnFromPool(GameData.Prefabs.BonusTaken, transform.position, Quaternion.identity, ObjectsPooler.Instance.transform);
        GameObject scoreText = ObjectsPooler.Instance.SpawnFromPool(GameData.Prefabs.BonusText, transform.position, Quaternion.identity, ObjectsPooler.Instance.transform);
        scoreText.transform.GetChild(0).GetComponent<TextMesh>().text = "+1";
        SoundManager.GetSingleton.playSound(GameData.Sounds.Bonus.ToString() + transform.GetInstanceID().ToString());
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

        if (other.CompareTag(GameData.Prefabs.Ball.ToString()) || other.CompareTag(GameData.Prefabs.Link.ToString()))
        {
            DoAction(other);
        }
    }

    /// <summary>
    /// appelé quand le jeu est fini...
    /// </summary>
    private void StopAction()
    {
        enabledObject = false;
    }
    #endregion

    #region Unity ending functions
    /// <summary>
    /// appelé lorsque la pool clean up les objet actif et les désactif (lors d'une transition de scene)
    /// </summary>
    public void OnDesactivePool()
    {
        Debug.Log("DesactiveFromPool");
        Kill();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, StopAction);
    }

    [FoldoutGroup("Debug"), Button("Kill")]
    public void Kill()
    {
        StopAction();
        //Debug.Log("linkSpawner desactive !");
        gameObject.SetActive(false);
    }
    #endregion
}
