using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelEvents : MonoBehaviour
{
    public void Update()
    {
        gameObject.GetComponent<Animator>().SetBool("isDoneLoading", RoftSceneNavi.Instance.isDoneLoading);
    }

    //Used for animation events
    public void EnableLoadingIcon()
    {
        RoftSceneNavi.Instance.loadingIcon.gameObject.SetActive(true);
        RoftSceneNavi.Instance.SendMeToScene("Gameplay");
    }

    public void SetSelfOff()
    {
        gameObject.SetActive(false);
    }
}
