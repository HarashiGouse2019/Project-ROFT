using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPress : MonoBehaviour
{
    //Taking the binded keys, and making them interactive
    public Key_Layout key_Layout;
    
    // Start is called before the first frame update
    void Start()
    {
        if (key_Layout == null)
            key_Layout = GetComponent<Key_Layout>();
    }

    // Update is called once per frame
    void Update()
    {
        RunInteractivity();
    }

    void RunInteractivity()
    {
        for (int keyNum = 0; keyNum < key_Layout.bindedKeys.Count; keyNum++)
        {
            if (Input.GetKey(key_Layout.bindedKeys[keyNum]))
            {
                
            }
        }
    }
}
