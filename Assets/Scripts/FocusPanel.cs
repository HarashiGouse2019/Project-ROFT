using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject volumeDashboard;

    public void OnPointerClick(PointerEventData eventData)
    {
        volumeDashboard.SetActive(false);
    }
}
