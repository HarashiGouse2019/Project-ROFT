using UnityEngine;
using UnityEngine.EventSystems;

public class VolumeControl : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject volumeDashboard;

    bool outsideOfDashboard = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (volumeDashboard.activeInHierarchy == false)
        {
            volumeDashboard.SetActive(!volumeDashboard.activeInHierarchy);
            return;
        }
    }
}
