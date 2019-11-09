using UnityEngine;

public class ClosestObjectClass : MonoBehaviour
{
    public static ClosestObjectClass Instance;
    public static GameObject[] closestObject = new GameObject[30];
    public GameObject[] closeObjTemp;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        closeObjTemp = closestObject;
    }
}
