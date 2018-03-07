using UnityEditor;
using UnityEngine;

using Sirenix.OdinInspector;
using TMPro;

/// <summary>
/// Plane Description
/// </summary>
public class PPlane : Generate
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("Length"), SerializeField]
    private float length = 1f;
    [FoldoutGroup("GamePlay"), Tooltip("width"), SerializeField]
    private float width = 1f;
    [FoldoutGroup("GamePlay"), Range(2, 100), OnValueChanged("ChangeRes"), Tooltip("resX"), SerializeField]
    private int res = 2;


    private int resX = 2; // 2 minimum
    private int resZ = 2;
    #endregion

    #region Initialization
    private void ChangeRes()
    {
        resX = resZ = res;
    }

    protected override void InitMesh()
    {
        Debug.Log("init...");
        typeMesh = TypeMesh.Plane;
    }
    #endregion

    #region Core
    protected override void GenerateMesh()
    {
        Debug.Log("generate...");
        MeshFilter meshFilter = gameObject.transform.GetOrAddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.transform.GetOrAddComponent<MeshRenderer>();

        Mesh mesh = meshFilter.mesh;
        //Mesh mesh = meshFilter.sharedMesh;
        mesh.Clear();

        #region Vertices		
        Vector3[] vertices = new Vector3[resX * resZ];
        for (int z = 0; z < resZ; z++)
        {
            // [ -length / 2, length / 2 ]
            float zPos = ((float)z / (resZ - 1) - .5f) * length;
            for (int x = 0; x < resX; x++)
            {
                // [ -width / 2, width / 2 ]
                float xPos = ((float)x / (resX - 1) - .5f) * width;
                vertices[x + z * resX] = new Vector3(xPos, 0f, zPos);
            }
        }
        #endregion

        #region Normales
        Vector3[] normales = new Vector3[vertices.Length];
        for (int n = 0; n < normales.Length; n++)
            normales[n] = Vector3.up;
        #endregion

        #region UVs		
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < resZ; v++)
        {
            for (int u = 0; u < resX; u++)
            {
                uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
            }
        }
        #endregion

        #region Triangles
        int nbFaces = (resX - 1) * (resZ - 1);
        int[] triangles = new int[nbFaces * 6];
        int t = 0;
        for (int face = 0; face < nbFaces; face++)
        {
            // Retrieve lower left corner from face ind
            int i = face % (resX - 1) + (face / (resZ - 1) * resX);

            triangles[t++] = i + resX;
            triangles[t++] = i + 1;
            triangles[t++] = i;

            triangles[t++] = i + resX;
            triangles[t++] = i + resX + 1;
            triangles[t++] = i + 1;
        }
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        //mesh.Optimize();

        //MeshUtility.Optimize(mesh);

        DisplayText(vertices);
    }
    #endregion

    #region Unity ending functions

    #endregion
}
