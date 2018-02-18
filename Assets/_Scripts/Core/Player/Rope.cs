using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

/// <summary>
/// Rope Description
/// </summary>
public class Rope : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("Rope"), OnValueChanged("ChangeValueSpring"), Tooltip("Position de l'anchor pour les 2 balles"), SerializeField]
    private Vector3 ropeAnchors = Vector3.zero;

    [FoldoutGroup("Rope"), OnValueChanged("ChangeValueSpring"), Tooltip("Force de l'élastique"), SerializeField]
    private float spring = 10;

    [FoldoutGroup("Rope"), OnValueChanged("ChangeValueSpring"), Tooltip("Force de l'amortissement"), SerializeField]
    private float damper = 0.2f;

    [FoldoutGroup("Rope"), OnValueChanged("ChangeValueSpring"), Tooltip("Force de l'amortissement"), SerializeField]
    private float min = 0.5f;

    [FoldoutGroup("Rope"), OnValueChanged("ChangeValueSpring"), Tooltip("Force de l'amortissement"), SerializeField]
    private float max = 0.5f;

    [FoldoutGroup("Rope"), Range(0, 100), OnValueChanged("InitPhysicRope"), Tooltip("Nombre de maillons"), SerializeField]
    private int linkCount = 3;

    [FoldoutGroup("Object"), Tooltip("Prefab du Maillon"), SerializeField]
    private GameObject prefabLink;

    [FoldoutGroup("Debug"), Tooltip("points des link"), SerializeField]
    private List<GameObject> linkList;
    public List<GameObject> LinkList { get { return linkList; } }

    [FoldoutGroup("Debug"), Tooltip("playerController"), SerializeField]
    private PlayerController playerController;

    #endregion

    #region Initialization

    #endregion

    #region Core
    /// <summary>
    /// initialise les springJoints; appelé depuis player
    /// </summary>
    public void InitPhysicRope()
    {
        ClearJoints();  //clear les joints précédents

        //cree un sprintjoint sur la première ball si il n'y en a pas déja
        if (!playerController.BallsList[0].gameObject.GetComponent<SpringJoint>())
            playerController.BallsList[0].gameObject.AddComponent<SpringJoint>();

        linkList.Add(playerController.BallsList[0].gameObject); //ajoute la première balle (premier joint)

        //cree les balls intermédiaire à la bonne position par rapport aux 2 balls
        for (int i = 0; i < linkCount; i++)
        {
            //cherche la position que devrais prendre ce joints
            float maxMid = linkCount + 1;

            float x1 = ( (((maxMid - 1.0f) - i) / maxMid) * playerController.BallsList[0].transform.position.x )
                    + ( ((1.0f + i) / maxMid) * playerController.BallsList[1].transform.position.x );
            float y1 = ((((maxMid - 1.0f) - i) / maxMid) * playerController.BallsList[0].transform.position.y)
                    + (((1.0f + i) / maxMid) * playerController.BallsList[1].transform.position.y);
            float z1 = ((((maxMid - 1.0f) - i) / maxMid) * playerController.BallsList[0].transform.position.z)
                    + (((1.0f + i) / maxMid) * playerController.BallsList[1].transform.position.z);

            Vector3 posJoint = new Vector3(x1, y1, z1);

            //créé un joint, à une position quelquonque, en parent: la où se trouve les balls du player
            GameObject newLink = Instantiate(prefabLink, posJoint, Quaternion.identity, playerController.Rope);
            linkList.Add(newLink);
        }
        //détruit le springJoint de la dernière ball si il y a
        if (playerController.BallsList[1].gameObject.GetComponent<SpringJoint>())
            Destroy(playerController.BallsList[1].gameObject.GetComponent<SpringJoint>());

        linkList.Add(playerController.BallsList[1].gameObject);

        
        //set la ball numéro 2 en dernier dans la liste hiérarchie
        //playerController.BallsList[1].transform.SetAsLastSibling();

        //connecte tout les liens ensemble avec des springs joints;
        ChangeValueSpring();
    }

    /// <summary>
    /// clear la list des joints (et supprime les objets dedant)
    /// </summary>
    private void ClearJoints()
    {
        for (int i = 0; i < linkList.Count; i++)
        {
            if (!linkList[i] || linkList[i].GetComponent<Balls>())
                continue;
            Destroy(linkList[i]);
        }
        linkList.Clear();
    }

    /// <summary>
    /// remplie les infos dans les springs joints
    /// </summary>
    private void ChangeValueSpring()
    {
        for (int i = 0; i < linkCount + 1; i++)
        {
            SpringJoint joint = linkList[i].GetComponent<SpringJoint>();
            joint.connectedBody = linkList[i + 1].GetComponent<Rigidbody>();
            joint.minDistance = min;
            joint.maxDistance = max;
            joint.anchor = ropeAnchors;
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = ropeAnchors;
            joint.spring = spring;
            joint.damper = damper;
            joint.enableCollision = false;
        }
    }
    #endregion

    #region Unity ending functions

	#endregion
}
