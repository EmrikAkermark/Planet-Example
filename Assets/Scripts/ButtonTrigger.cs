using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonTrigger : MonoBehaviour
{

    public KeyCode Toggler;             //Key To Invoke
    public UnityEvent ToInvoke;         //Invoke list


    void Update()
    {
        if (Input.GetKeyDown(Toggler))
        {
            ToInvoke.Invoke();
        }
    }
}