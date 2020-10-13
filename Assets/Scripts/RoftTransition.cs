using UnityEngine;

public class RoftTransition : MonoBehaviour
{
    public enum Transitions
    {
        TITLE_TO_LOBBY,
        LOBBY_TO_SONGROOM,
        LOBBY_TO_RECORDS,
        LOBBY_TO_SETTINGS,
        LOBBY_TO_HELP,
        SONGROOM_TO_LOBBY,
        SONGROOM_TO_GAMEROOM,
        GAMEROOM_TO_RESULT
    }

    public void RunTransition(int _transitionIndex)
    {
        /*When setting a new transition,
         make sure the Transition enumerate has the existing transition.
         Whatever the destination target is, use "is<Destination>Target
         */
        Transitions transitions = (Transitions)_transitionIndex;
        PanelEvents.Instance.gameObject.SetActive(true);
        RoftSceneNavi.Instance.isDoneLoading = false;
        switch (transitions)
        {
            case Transitions.TITLE_TO_LOBBY:
                PanelEvents.Instance.isLobbyTarget = true;
                PanelEvents.Instance.PanelAnimator.SetBool("isLobbyTarget", PanelEvents.Instance.isLobbyTarget);
                break;

            case Transitions.LOBBY_TO_SONGROOM:
                PanelEvents.Instance.isSongRoomTarget = true;
                PanelEvents.Instance.PanelAnimator.SetBool("isSongRoomTarget", PanelEvents.Instance.isSongRoomTarget);
                break;

            case Transitions.LOBBY_TO_RECORDS:
                PanelEvents.Instance.PanelAnimator.SetBool("isRecordsTarget", PanelEvents.Instance.isRecordsTarget);
                break;

            case Transitions.LOBBY_TO_SETTINGS:
                PanelEvents.Instance.PanelAnimator.SetBool("isSettingsTarget", PanelEvents.Instance.isSettingsTarget);
                break;

            case Transitions.LOBBY_TO_HELP:
                PanelEvents.Instance.PanelAnimator.SetBool("isHelpTarget", PanelEvents.Instance.isHelpTarget);
                break;

            case Transitions.SONGROOM_TO_LOBBY:
                PanelEvents.Instance.isLobbyTarget = true;
                PanelEvents.Instance.PanelAnimator.SetBool("isLobbyTarget", PanelEvents.Instance.isLobbyTarget);
                break;

            case Transitions.SONGROOM_TO_GAMEROOM:
                PanelEvents.Instance.isGameRoomTarget = true;
                PanelEvents.Instance.PanelAnimator.SetBool("isGameRoomTarget", PanelEvents.Instance.isGameRoomTarget);
                break;

            case Transitions.GAMEROOM_TO_RESULT:
                PanelEvents.Instance.PanelAnimator.SetBool("isResultsTarget", PanelEvents.Instance.isResultsTarget);
                break;

            default:
                break;

        }
    }
}
