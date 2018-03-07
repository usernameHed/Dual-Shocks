using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

/// <summary>
/// Generate Description
/// </summary>
public abstract class Generate : MonoBehaviour
{
    #region Attributes

    public enum TypeMesh
    {
        Plane,         //tout les objets du décors actif (les boules)
        Box,         //les balls
        Cone,           //les link sont dans ce layer
        Tube,
        Torus,
        Sphere,
        IcoSphere,
    }

    [FoldoutGroup("GamePlay"), Tooltip("type"), SerializeField]
    protected TypeMesh typeMesh;

    [FoldoutGroup("GamePlay"), Tooltip("text"), SerializeField]
    protected TextMesh textprefabs;

    [FoldoutGroup("Save"), Tooltip("name"), SerializeField]
    protected string saveName = "SavedMesh";

    [FoldoutGroup("Save"), Tooltip("name"), SerializeField]
    protected string pathToSave = "Assets/_Prefabs/Game/Generate/";


    protected SaveGeneratedMesh saveMesh;
    /*protected MeshFilter meshFilter;
    protected MeshRenderer meshRenderer;*/
    //protected Mesh mesh;

    #endregion

    #region Initialization
    private void Awake()
    {
        

        saveMesh = new SaveGeneratedMesh(transform);
    }

    private void Start()
    {
        InitMesh();
    }
    #endregion

    #region Core

    [Button("GeneratePlease")]
    private void GeneratePlease()
    {
        transform.ClearChildImediat();
        GenerateMesh();
    }

    [Button("Save")]
    private void SaveMesh()
    {
        saveMesh.SaveAsset("Assets/_Prefabs/Game/Generate/", "SavedMesh");
    }

    abstract protected void InitMesh(); //appelé à l'initialisation
    abstract protected void GenerateMesh(); //appelé à l'initialisation

    protected void DisplayText(Vector3[] vertices)
    {
        int j = 0;
        foreach (Vector3 pos in vertices)
        {
            //Vector3 globalPos = new Vector3(transform.position.x - pos.x, transform.position.x - pos.y, transform.position.x - pos.z);
            TextMesh tm = Instantiate(textprefabs, pos, Quaternion.identity/*, transform*/) as TextMesh;
            tm.name = (j).ToString();
            tm.text = (j++).ToString();
            tm.transform.SetParent(transform);
            tm.transform.position = new Vector3(tm.transform.position.x + transform.position.x, tm.transform.position.y + transform.position.y, tm.transform.position.z + transform.position.z);
        }
    }
    #endregion

    #region Unity ending functions

    #endregion
}
