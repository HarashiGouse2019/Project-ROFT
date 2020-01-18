using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoftGate : MonoBehaviour
{
    public Animator tmpAnimator;
    public Animator roftScenePanel;
    // Update is called once per frame
    void Update()
    {
        DetectAnyKey();
    }

    void DetectAnyKey()
    {
        if (Input.anyKeyDown)
        {
            tmpAnimator.SetBool("keyPressed", true);
            roftScenePanel.enabled = true;
            //RoftSceneNavi.Instance.SendMeToScene("Gameplay");
        }
    }
}
