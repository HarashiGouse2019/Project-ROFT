using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewButton : MonoBehaviour
{
    [SerializeField]
    GameObject formObject;
    bool active = false;
    public void ToggleForm()
    {
        active = !active;
        formObject.SetActive(active);
    }
}
