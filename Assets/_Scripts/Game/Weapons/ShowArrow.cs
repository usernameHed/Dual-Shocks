using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ShowArrow Description
/// </summary>
public class ShowArrow : MonoBehaviour, IKillable
{
    #region Attributes

    [FoldoutGroup("Gameplay"), Tooltip("radius de l'explosion"), SerializeField]
    private float radiusDetection = 5f;
    [FoldoutGroup("Gameplay"), Tooltip("Scale max des arrow"), SerializeField]
    private float scaleMaxArrow = 2f;
    [FoldoutGroup("Gameplay"), Tooltip("Nom des layers à chercher et à pousser"), SerializeField]
    private GameData.Layers[] layerToShowArrow;
    [FoldoutGroup("Gameplay"), Tooltip("Nom des layers à chercher et à pousser"), SerializeField]
    private List<GameData.Prefabs> tagToShowArrow;

    [FoldoutGroup("Gameplay"), Tooltip("List des arrow"), SerializeField]
    private List<GameObject> Arrow;

    [FoldoutGroup("Debug"), Tooltip("List des objets à mettre l'arrow dessus"), ShowInInspector]
    private List<GameObject> targetToShowArrow = new List<GameObject>();
    public List<GameObject> TargetToShowArrow { get { return (targetToShowArrow); } }

    [FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
	private FrequencyTimer updateTimer;


    private Collider[] arrowToShow = new Collider[maxArrow];

    private const int maxArrow = 5;
    #endregion

    #region Initialization

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        targetToShowArrow.Clear();
        for (int i = 0; i < maxArrow; i++)
        {
            targetToShowArrow.Add(null);
        }
    }
    #endregion

    #region Core
    /// <summary>
    /// montre une fleche directionnel en présence des ennemys
    /// </summary>
    private void UpdateArrowPosition()
    {
        //par rapport à la position de la balle qui attaque, son radius, et recherche les layers voulu
        int numberEnnemy = Physics.OverlapSphereNonAlloc(transform.position, radiusDetection, arrowToShow, LayerMask.GetMask(UtilityFunctions.GetStringsFromEnum(layerToShowArrow)));

        //parcourt chaque collider trouvé
        for (int i = 0; i < numberEnnemy; i++)
        {
            GameObject ennemy = arrowToShow[i].gameObject;
            if (GameData.IsInList(tagToShowArrow, ennemy.tag) && !targetToShowArrow.Contains(ennemy))
            {
                if (ennemy.activeSelf)
                    targetToShowArrow[i] = ennemy;
                else
                    targetToShowArrow[i] = null;
            }
        }
        for (int i = numberEnnemy; i < maxArrow; i++)
        {
            targetToShowArrow[i] = null;
        }
    }

    /// <summary>
    /// affiche les 5 arrow (activé ou pas) sur les objets
    /// </summary>
    private void ShowingArrow()
    {
        for (int i = 0; i < targetToShowArrow.Count; i++)
        {
            if (targetToShowArrow[i] != null && targetToShowArrow[i].activeSelf)
            {
                if (!Arrow[i].activeSelf)
                    Arrow[i].SetActive(true);
                Arrow[i].transform.position = targetToShowArrow[i].transform.position;

                //Vector3 dir = transform.position - targetToShowArrow[i].transform.position;
                //Debug.Log("dir: " + dir);
                //Arrow[i].transform.rotation = Quaternion.Euler(dir);
                Vector3 relativePos = transform.position - targetToShowArrow[i].transform.position;
                //Arrow[i].transform.rotation.LookAtDir((transform.position - targetToShowArrow[i].transform.position) * -1);

                
                //Quaternion rotation = Quaternion.LookRotation(relativePos * -1);
                Arrow[i].transform.rotation = QuaternionExt.LookAtDir(relativePos);
                //Arrow[i].transform.LookAt(transform.position * -1);

                float dist = Vector3.Distance(transform.position, targetToShowArrow[i].transform.position); //get la distance ball - ennemy
                dist = Mathf.Min(dist, radiusDetection);     //clamp la distance max au radius (normalement la distance ne doit pas être plus grande que le radius)
                dist = radiusDetection - dist;   //met à l'envert
                dist = dist * scaleMaxArrow / radiusDetection;   //produit en croix pour clamp la distance à une valeur plus petite (radius 8 = 2 max)

                Arrow[i].transform.localScale = new Vector3(
                    Arrow[i].transform.localScale.x,
                    Arrow[i].transform.localScale.y,
                    dist);
            }
            else
            {
                Arrow[i].SetActive(false);
            }
        }
    }
    #endregion

    #region Unity ending functions

    private void Update()
    {
        //optimisation des fps
        if (updateTimer.Ready())
        {
            UpdateArrowPosition();
        }
    }

    private void LateUpdate()
    {
        ShowingArrow();    //ici le mettre dans lateUpdate ???
    }

    public void Kill()
    {
        for (int i = 0; i < TargetToShowArrow.Count; i++)
        {
            TargetToShowArrow[i] = null;
            Arrow[i].SetActive(false);
        }
    }

    #endregion
}
