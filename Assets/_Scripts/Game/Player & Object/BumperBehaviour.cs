using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

/// <summary>
/// PlayerController handle player movement
/// <summary>
public class BumperBehaviour : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("Vecteur direction de la poussé"), SerializeField]
    private Transform direction;

    [Space(10)]
    [FoldoutGroup("GamePlay"), Tooltip("force de poussée"), SerializeField]
    private float forceObjects = 15;
    [Space(10)]
    //[FoldoutGroup("GamePlay"), Tooltip("force de poussée d'un link quand il touche un bumper"), SerializeField]
    //private float forceLinkAlone = 5;
    [FoldoutGroup("GamePlay"), Tooltip("force de poussée"), SerializeField]
    private float forcePlayer = 10;
    [FoldoutGroup("GamePlay"), Tooltip("stun player"), SerializeField]
    private bool stunPlayer = true;
    [FoldoutGroup("GamePlay"), EnableIf("stunPlayer"), Tooltip("stun player"), SerializeField]
    private float stunPlayerTime = 0.5f;

    [Space(10)]
    [FoldoutGroup("GamePlay"), Tooltip("list des prefabs à push"), SerializeField]
    private List<GameData.Layers> listLayerToPush;

    [FoldoutGroup("Object"), Tooltip("display"), SerializeField]
    private MeshRenderer mesh;

    private bool enabledObject = true;
    #endregion

    #region Initialize
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        enabledObject = true;
        //mesh.enabled = false;
    }

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.GameOver, StopAction);
    }
    #endregion

    #region Core
    /// <summary>
    /// trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (GameData.IsInList(listLayerToPush, other.gameObject.layer))
        {
            DoBump(other.gameObject);
        }
    }

    /// <summary>
    /// Bump u nobjet !
    /// </summary>
    private void DoBump(GameObject obj)
    {
        Rigidbody rbOther = obj.GetComponent<Rigidbody>();
        if (!rbOther)
            return;

        Vector3 dir = (transform.position - direction.position).normalized;
        rbOther.ClearVelocity();

        if (obj.CompareTag(GameData.Prefabs.Ball.ToString()))
        {
            Debug.Log("Stun Ball !");
            
            Balls ball = obj.gameObject.GetComponent<Balls>();
            if (stunPlayer)
                ball.Stun(true, stunPlayerTime);
            ball.PlayerRef.RopeScript.ApplyForceOnAll(obj, dir, -forcePlayer);
        }
        /*else if (obj.CompareTag(GameData.Prefabs.Link.ToString()))
        {
            rbOther.AddForce(dir * -forceLinkAlone, ForceMode.VelocityChange);
        }*/
        else
        {
            rbOther.AddForce(dir * -forceObjects, ForceMode.VelocityChange);
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
    /// draw le vecteur
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (direction)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, direction.position);
        }
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, StopAction);
    }

    #endregion
}