using Sirenix.OdinInspector;
using UnityEngine;

public class DisableOnEndAnimation : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("desactiver l'objet courant ou le parent ?"), SerializeField]
    public bool desactiveParent = false;

    [FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
    private FrequencyTimer updateTimer;

    private bool wantToDisable = false;
    private GameObject parent;

    private void Awake()
    {
        if (desactiveParent)
        {
            parent = transform.parent.gameObject; 
        }
    }

    public void DisableObject()
    {
        wantToDisable = true;
        //print("yo");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (updateTimer.Ready())
        {
            if (wantToDisable)
            {
                wantToDisable = false;
                if (desactiveParent && parent)
                    parent.SetActive(false);
                else
                    gameObject.SetActive(false);
            }
        }

        
	}
}
