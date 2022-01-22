using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ControlPointEventHandler : MonoBehaviour
{
    public UnityEvent ControlPointEvent;

    private void Start()
    {
        if (ControlPointEvent == null)
            ControlPointEvent = new UnityEvent();

        
    }

}
