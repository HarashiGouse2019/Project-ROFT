using UnityEngine;

public class PanelEvents : MonoBehaviour
{
    public static PanelEvents Instance;
    private Animator panelAnimator;
    public Animator PanelAnimator
    {
        get
        {
            return panelAnimator;
        }
    }

    /*You can add to this Transitions
     * enumerate if you want to do specific transitions
     * to one scene to another.
     */
    
    public bool isLobbyTarget, 
        isSongRoomTarget, 
        isRecordsTarget, 
        isSettingsTarget, 
        isHelpTarget, 
        isGameRoomTarget,
        isResultsTarget;
  
    private void Awake()
    {
        Instance = this;
        panelAnimator = gameObject.GetComponent<Animator>();
    }
    public void Update()
    {
        panelAnimator.SetBool("isDoneLoading", RoftSceneNavi.Instance.isDoneLoading);
    }

    //Used for animation events
    public void EnableLoadingIcon(string _destinationAfter)
    {
        RoftSceneNavi.Instance.loadingIcon.gameObject.SetActive(true);
        RoftSceneNavi.Instance.SendMeToScene(_destinationAfter);
    }

    public void SetSelfOff()
    {
        gameObject.SetActive(false);
    }
}
