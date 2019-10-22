using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestObjectClass : MonoBehaviour
{
    public static ClosestObjectClass Instance;
    public GameObject[] closestObject = new GameObject[30];
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }
}
