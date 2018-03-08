using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

/// <summary>
/// PlayerController handle player movement
/// <summary>
public class EnemyBehaviour : MonoBehaviour, IKillable, IPooledObject
{
    #region Attributes
    private bool enabledEnemy = true;
    private Rigidbody rigidBody;
    #endregion

    #region Initialize
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        Init();
    }

    /// <summary>
    /// activé quand l'ennemy est spawn
    /// </summary>
    public void OnObjectSpawn()
    {
        Init();
    }

    private void Init()
    {
        enabledEnemy = true;
        rigidBody.velocity = Vector3.zero;
    }

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.GameOver, StopAction);
    }
    #endregion

    #region Core
    /// <summary>
    /// appelé quand le jeu est fini...
    /// </summary>
    private void StopAction()
    {
        enabledEnemy = false;
    }

    /// <summary>
    /// appelé lorsque la pool clean up les objet actif et les désactif (lors d'une transition de scene)
    /// </summary>
    public void OnDesactivePool()
    {
        Debug.Log("DesactiveFromPool");
        StopAction();
        gameObject.SetActive(false);
    }
    #endregion

    #region Unity ending functions

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, StopAction);
    }

    #endregion


    [FoldoutGroup("Debug"), Button("Kill")]
    public void Kill()
	{
        if (!enabledEnemy)
            return;

		Debug.Log ("Enemy dead !");
        enabledEnemy = false;

        /*GameObject bonusParticle = */
        ObjectsPooler.Instance.SpawnFromPool(GameData.Prefabs.EnemyExplode, transform.position, Quaternion.identity, ObjectsPooler.Instance.transform);

        gameObject.SetActive(false);
    }
}