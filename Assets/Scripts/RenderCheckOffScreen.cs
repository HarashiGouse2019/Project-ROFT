using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderCheckOffScreen : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (collision.name == "RULERBASEMASK")
            GetComponent<MeshRenderer>().enabled = true;
        else
            GetComponent<MeshRenderer>().enabled = false;
    }
}
