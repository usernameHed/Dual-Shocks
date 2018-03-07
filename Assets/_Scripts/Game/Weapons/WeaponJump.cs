using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Networking;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class WeaponJump : Weapon
{
    #region variable
    /// <summary>
    /// variable
    /// </summary>
    [FoldoutGroup("Jump"), Tooltip("Impulsion du joueur lors du tir"), SerializeField]
    private float forceImpulse = 10f;
    [FoldoutGroup("Jump"), Tooltip("Impulsion du joueur lors du tir"), SerializeField]
    private float forceLinkImpulse = 10f;
    [FoldoutGroup("Jump"), Range(0, 100), Tooltip("Nombre de link à pousser aussi (en %age, pousse les link les plus proch du la ball)"), SerializeField]
    private int linkToPush = 0;

    [FoldoutGroup("Jump"), Tooltip("la distance à ajouter à la taille des colliders de la ball"), SerializeField]
    private float marginDistToGround = 0.1f;

    private float distToGround;
    #endregion

    #region  initialisation
    /// <summary>
    /// appelé lors de l'initialisation de ce weapon
    /// </summary>
    protected override void InitParticularWeapon()
    {
        Debug.Log("init this weapon id: " + IdWeapon);
        distToGround = ballRef.GetComponent<Collider>().bounds.extents.y;
    }
    #endregion

    #region core script
    /// <summary>
    /// Jump
    /// </summary>
	protected override bool OnShoot()
    {
		Debug.Log("Jumpi !");
        if (!IsGrounded())
            return (false);

        //pousse la balle
        ApplyForce(ballRef.BallBody, forceImpulse);
        //ballRef.BallBody.AddForce(Vector3.up * forceImpulse, ForceMode.VelocityChange);

        //pousse les X link aussi
        if (linkToPush > 0)
            PushLink();

        SoundManager.GetSingleton.playSound("Jump" + transform.GetInstanceID().ToString());
        return (true);
    }

    /// <summary>
    /// pousse les X link attaché à la ball
    /// </summary>
    private void PushLink()
    {
        Rope rope = playerRef.RopeScript;

        int startLoop = 0;
        int stopLoop = 0;
        int numberLink = (linkToPush * rope.LinkCircular.Count) / 100;

        //si la ball de référence est au début de la chaine, partir du bas vers le haut de la list !
        if (rope.LinkCircular[0].Value.GetInstanceID() == ballRef.gameObject.GetInstanceID())
        {
            stopLoop = Mathf.Min(numberLink, rope.LinkCircular.Count - 1);
            for (int i = 1; i < stopLoop; i++)
            {
                ApplyForce(rope.LinkCircular[i].Value.GetComponent<Rigidbody>(), forceLinkImpulse);
            }
        }
        //si la ball de référence est à la fin, partir du haut vers le bas
        else if (rope.LinkCircular[rope.LinkCircular.Count - 1].Value.GetInstanceID() == ballRef.gameObject.GetInstanceID())
        {
            startLoop = rope.LinkCircular.Count - 2;
            stopLoop = Mathf.Max(0, (rope.LinkCircular.Count - 1) - numberLink);
            for (int i = startLoop; i > stopLoop; i--)
            {
                ApplyForce(ballRef.BallBody, forceLinkImpulse);
            }
        }

    }

    /// <summary>
    /// applique la force sur la ball / le link
    /// </summary>
    private void ApplyForce(Rigidbody rbToPush, float force)
    {
        rbToPush.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    /// <summary>
    /// renvoi si le joueur est grounded ou pas
    /// </summary>
    private bool IsGrounded()
    {
        return Physics.Raycast(ballRef.transform.position, -Vector3.up, distToGround + marginDistToGround);
    }
    #endregion

    #region unity fonction and ending

    #endregion
}
