using UnityEngine;
using UnityEngine.Events;

public class FixedUpdateTrigger : MonoBehaviour
{
    public UnityEvent Event;

    void FixedUpdate()
    {
        Event.Invoke();
    }
}
