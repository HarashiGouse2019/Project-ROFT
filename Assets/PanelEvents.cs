using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelEvents : MonoBehaviour
{
    //Used for animation events
    public void EnableLoadingIcon()
    {
        RoftSceneNavi.Instance.loadingIcon.gameObject.SetActive(true);
    }
}
