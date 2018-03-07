#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

/// <summary>
/// SaveGeneratedMesh Description
/// </summary>
[System.Serializable]
public class SaveGeneratedMesh
{
    #region Attributes
    //[FoldoutGroup("Save"), Tooltip("type"), SerializeField]
    private Transform selectedGameObject;
    #endregion

    #region Initialization
    public SaveGeneratedMesh(Transform selected)
    {
        this.selectedGameObject = selected;
    }
    #endregion

    #region Core
    public void SaveAsset(string pathToSave, string saveName)
    {
        var mf = selectedGameObject.GetComponent<MeshFilter>();
        if (mf)
        {
            string savePath = pathToSave + saveName + ".asset";
            
            if (AssetDatabase.GetAssetPath(mf.mesh) == null || AssetDatabase.GetAssetPath(mf.mesh) == "")
            {
                Debug.Log("Saved Mesh to:" + savePath);
                AssetDatabase.CreateAsset(mf.mesh, savePath);
            }
            else
            {
                Debug.Log("Mesh already created, suppress and recreate...");
                savePath = UtilityFunctions.GetNextFileName(savePath);
                Debug.Log("name: " + savePath);

                AssetDatabase.CreateAsset(mf.mesh, savePath);
            }
        }
    }
    #endregion

    #region Unity ending functions

    #endregion
}
#endif