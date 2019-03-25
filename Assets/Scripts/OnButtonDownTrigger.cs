using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnButtonDownTrigger : MonoBehaviour
{

    public UnityEvent Event;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Event.Invoke();
        }
    }
}
