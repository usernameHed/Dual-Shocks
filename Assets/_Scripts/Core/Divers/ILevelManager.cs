using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelManager
{
    /// <summary>
    /// initialise ce scripts
    /// </summary>
	void InitScene();
    /// <summary>
    /// est appelé quand il y a un changement de gamePad
    /// </summary>
    void CallGamePad();
}
