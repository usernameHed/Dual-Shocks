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

    [FoldoutGroup("Rope"), Range(0, 100), OnValueChanged("LinkCountAdd"), OnValueChanged("InitPhysicRope"), Tooltip("Nombre de maillons"), SerializeField]
    private int linkCount = 3;
    private void LinkCountAdd() { linkCount++; if (linkCount > linkCountMax) linkCount = linkCountMax; }
    [FoldoutGroup("Rope"), Tooltip("Maillons max"), SerializeField]
    private int linkCountMax = 30;


    [FoldoutGroup("Objects"), OnValueChanged("InitPhysicRope"), Tooltip("Les 2 objets à relié"), SerializeField]
    private GameObject[] objectToConnect = new GameObject[2];
    [FoldoutGroup("Objects"), Tooltip("le parent où mettre les Link"), SerializeField]
    private Transform parentLink;

    //[FoldoutGroup("Debug"), Tooltip("points des link"), SerializeField]
    //private List<GameObject> linkList;
    //public List<GameObject> LinkList { get { return linkList; } }


    [FoldoutGroup("Debug"), Tooltip("points des link"), SerializeField]
    private Color colorRope;


    [OnValueChanged("CreateFakeListForDebug")]
    private CircularLinkedList<GameObject> listCircular = new CircularLinkedList<GameObject>();
    public CircularLinkedList<GameObject> LinkCircular { get { return listCircular; } }

    [Tooltip("points des link"), SerializeField]
    private List<GameObject> listDebug;




    #endregion

    #region Initialization
    /// <summary>
    /// Setup les 2 objet à relié
    /// </summary>
    public void InitObjects(GameObject obj1, GameObject obj2, Transform parentRope)
    {
        objectToConnect[0] = obj1;
        objectToConnect[1] = obj2;
        parentLink = parentRope;
    }
    #endregion

    #region Core
    /// <summary>
    /// initialise les springJoints; appelé depuis player
    /// </summary>
    public void InitPhysicRope()
    {
        ClearJoints();  //clear les joints précédents

        //cree un sprintjoint sur la première ball si il n'y en a pas déja
        if (!objectToConnect[0].GetComponent<SpringJoint>())
            objectToConnect[0].AddComponent<SpringJoint>();

        //linkList.Add(objectToConnect[0]); //ajoute la première balle (premier joint)
        listCircular.AddLast(objectToConnect[0]);

        //cree les balls intermédiaire à la bonne position par rapport aux 2 balls
        for (int i = 0; i < linkCount; i++)
        {
            if (i > linkCountMax)
                break;

            //cherche la position que devrais prendre ce joints
            float maxMid = linkCount + 1;

            float x1 = ((((maxMid - 1.0f) - i) / maxMid) * objectToConnect[0].transform.position.x)
                    + (((1.0f + i) / maxMid) * objectToConnect[1].transform.position.x);
            float y1 = ((((maxMid - 1.0f) - i) / maxMid) * objectToConnect[0].transform.position.y)
                    + (((1.0f + i) / maxMid) * objectToConnect[1].transform.position.y);
            float z1 = ((((maxMid - 1.0f) - i) / maxMid) * objectToConnect[0].transform.position.z)
                    + (((1.0f + i) / maxMid) * objectToConnect[1].transform.position.z);

            Vector3 posJoint = new Vector3(x1, y1, z1);

            //créé un joint, à une position quelquonque, en parent: la où se trouve les balls du player
            //GameObject newLink = Instantiate(prefabLink, posJoint, Quaternion.identity, playerController.Rope);
            GameObject newLink = ObjectsPooler.GetSingleton.SpawnFromPool("Link", posJoint, Quaternion.identity, parentLink);
            SetupLink(newLink, i);
            //linkList.Add(newLink);
            listCircular.AddLast(newLink);
        }
        //détruit le springJoint de la dernière ball si il y a
        if (objectToConnect[1].GetComponent<SpringJoint>())
            Destroy(objectToConnect[1].GetComponent<SpringJoint>());

        //linkList.Add(objectToConnect[1]);
        listCircular.AddLast(objectToConnect[1]);

        //set la ball numéro 2 en dernier dans la liste hiérarchie
        //playerController.BallsList[1].transform.SetAsLastSibling();

        //connecte tout les liens ensemble avec des springs joints;
        ChangeValueSpring();
        ChangeColorLink(colorRope);  //change color

        CreateFakeListForDebug();
    }


    /// <summary>
    /// une fois créé, setup le link
    /// </summary>
    private void SetupLink(GameObject newLink, int index)
    {
        Link linkScript = newLink.GetComponent<Link>();
        linkScript.RopeScript = this;
        linkScript.IdFromRope = index;

        Debug.Log("color la link");
        //ChangeMeshRenrered(0); //ou max -1 ?
    }

    /// <summary>
    /// ajoute un link, proche de la ball [index] voulu
    /// index = 0 = le début de la list, index = 1 = la fin de la liste
    /// </summary>
    /// <param name="indexObject"></param>
    public void AddLinkFromExtremity(int indexObject)
    {
        Debug.Log("ici ajoute un link :" + indexObject);
        if (indexObject == 0)
            AddLink(0);
        else
            AddLink(listCircular.Count - 1);
    }



    /// <summary>
    /// ici est appelé directement depuis l'un des link
    /// </summary>
    public void AddLink(int index)
    {
        if (linkCount == linkCountMax)
            return;
        LinkCountAdd();

        GameObject closestLink = listCircular[index].Value;

        if (!closestLink)
        {
            listCircular.RemoveAllEmpty();
            closestLink = listCircular[index].Value;
        }


        GameObject newLink = ObjectsPooler.GetSingleton.SpawnFromPool("Link", closestLink.transform.position, Quaternion.identity, parentLink);

        ChangeMeshRenrered(newLink.GetComponent<MeshRenderer>());

        //si l'index n'est pas le dernier (l'un des 2 gros objet), on peut le créé après
        if (index != listCircular.Count - 1)
        {
            listCircular.AddAfter(listCircular[index], newLink);
            ChangeThisPring(index);
            ChangeThisPring(index + 1);
            SetupLink(newLink, index + 1);
        }
        else
        {
            Debug.Log("ICI on ajoute sur le dernier ???");
            listCircular.AddBefore(listCircular[index], newLink);
            ChangeThisPring(index - 1);
            ChangeThisPring(index - 0);
            SetupLink(newLink, index - 0);
        }





        CreateFakeListForDebug();
    }

    /// <summary>
    /// clear la list des joints (et supprime les objets dedant)
    /// </summary>
    public void ClearJoints()
    {
        for (int i = 0; i < listCircular.Count; i++)
        {
            GameObject linkTmp = listCircular[i].Value;
            if (!linkTmp /*|| linkList[i].GetComponent<Balls>()*/ || !linkTmp.CompareTag("Link"))
                continue;
            linkTmp.transform.SetParent(ObjectsPooler.GetSingleton.transform);
            linkTmp.SetActive(false);
            //Destroy(linkList[i]);
        }
        listCircular.Clear();
    }

    /// <summary>
    /// change la couleur de tout les links (pas le premier / dernier !)
    /// </summary>
    public void ChangeColorLink(Color color)
    {
        colorRope = color;
        for (int i = 1; i < listCircular.Count - 1; i++)
        {
            ChangeMeshRenrered(i);
        }
    }
    private void ChangeMeshRenrered(MeshRenderer meshLink)
    {
        if (!meshLink)
            return;
        meshLink.material.color = colorRope;
    }
    private void ChangeMeshRenrered(int index)
    {
        MeshRenderer meshLink = listCircular[index].Value.GetComponent<MeshRenderer>();
        if (!meshLink)
            return;
        meshLink.material.color = colorRope;
    }

    /// <summary>
    /// remplie les infos dans les springs joints
    /// </summary>
    private void ChangeValueSpring()
    {
        for (int i = 0; i < linkCount + 1; i++)
        {
            ChangeThisPring(i);
        }
    }
    private void ChangeThisPring(int index)
    {
        if (!listCircular[index + 1].Value)
        {
            listCircular.RemoveAllEmpty();
            return;
        }


        SpringJoint joint = listCircular[index].Value.GetComponent<SpringJoint>();
        joint.connectedBody = listCircular[index + 1].Value.GetComponent<Rigidbody>();
        joint.minDistance = min;
        joint.maxDistance = max;
        joint.anchor = ropeAnchors;
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = ropeAnchors;
        joint.spring = spring;
        joint.damper = damper;
        joint.enableCollision = false;
    }
    #endregion

    #region Unity ending functions

    [Button("clearListOfNull")]
    private void clearListOfNull()
    {
        listCircular.RemoveAllEmpty();
        CreateFakeListForDebug();
    }

    private void CreateFakeListForDebug()
    {
        Debug.Log("ici reset...");
        listDebug.Clear();
        for (int i = 0; i < listCircular.Count; i++)
        {
            listDebug.Add(listCircular[i].Value);
        }
    }

    #endregion
}