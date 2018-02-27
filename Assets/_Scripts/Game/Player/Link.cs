using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Link Description
/// </summary>
public class Link : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("Force de l'élastique"), SerializeField]
    private int idFromRope = 0;
    public int IdFromRope { get { return (idFromRope); } set { idFromRope = value; } }

    [FoldoutGroup("Object"), Tooltip("Ref à la rope"), SerializeField]
    private Rope rope;
    public Rope RopeScript { get { return (rope); } set { rope = value; } }

    #endregion

    #region Initialization

    #endregion

    #region Core

    #endregion

    #region Unity ending functions


	#endregion
}
