using UnityEngine;

public class ClosestObjectClass : MonoBehaviour
{
    public static GameObject[] closestObject = new GameObject[30];
    public static GameObject targetKey;
    public GameObject currentTargetKey;

    private void Update()
    {
        currentTargetKey = targetKey;
    }
}
