using UnityEngine;
using UnityEngine.Events;

public class FixedUpdateTrigger : MonoBehaviour
{
    public UnityEvent Event;

    void Update()
    {
        Event.Invoke();
    }
}
