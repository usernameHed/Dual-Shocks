using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class WeaponStop : Weapon
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    [FoldoutGroup("Gameplay"), Tooltip("drag à mettre à la ball quand on stop"), SerializeField]
    private float dragWhenStop = 3f;

    [FoldoutGroup("Object"), Tooltip("l'objet à mettre actif lors du pouvoir"), SerializeField]
    private GameObject Spikes;

    private float saveDrag = 0;

    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation à l'activation
    /// </summary>
    private void Start()
    {
        saveDrag = ballRef.BallBody.drag;
    }

    /// <summary>
    /// appelé lors de l'initialisation de ce weapon
    /// </summary>
    protected override void InitParticularWeapon()
    {
        Debug.Log("init this weapon id: " + IdWeapon);
    }
    #endregion

    #region core script
    /// <summary>
    /// functionTest
    /// </summary>
	protected override void OnShoot()
    {
        Spikes.SetActive(true);
        SoundManager.GetSingleton.playSound("SpiksOn" + transform.GetInstanceID().ToString());
        ballRef.BallBody.drag = dragWhenStop;
    }

    /// <summary>
    /// appelé quand on reste appuyé
    /// </summary>
    protected override void OnShootHold()
    {
        //Debug.Log("Throwing...");
    }

    /// <summary>
    /// appelé lorsque on relache la touche
    /// </summary>
    protected override void OnShootRelease()
    {
        Spikes.SetActive(false);
        ballRef.BallBody.drag = saveDrag;

        SoundManager.GetSingleton.playSound("SpiksOff" + transform.GetInstanceID().ToString());
    }

    #endregion

    #region unity fonction and ending

    #endregion
}
