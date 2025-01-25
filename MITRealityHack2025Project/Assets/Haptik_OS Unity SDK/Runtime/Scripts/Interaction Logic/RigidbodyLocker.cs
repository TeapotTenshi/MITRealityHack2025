using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RigidbodyLocker : MonoBehaviour
{
    [SerializeField] bool lockX, lockY, lockZ;
    
    [SerializeField] InteractionDetector interactionDetector;
    [SerializeField] Rigidbody rigidbody;

    RigidbodyConstraints initialConstraints;

    public UnityEvent onTriggerTouched;

    private void Awake()
    {
        initialConstraints = rigidbody.constraints;
    }

    private void OnEnable()
    {

        interactionDetector.onObjectStartTouching.AddListener(OnTriggerZoneTouched);
        interactionDetector.onObjectStoppedTouching.AddListener(OnTriggerZoneUntouched);

    }

    private void OnTriggerZoneUntouched()
    {
        rigidbody.constraints = initialConstraints;
    }

    private void OnDisable()
    {
        interactionDetector.onObjectStartTouching.RemoveListener(OnTriggerZoneTouched);
        interactionDetector.onObjectStoppedTouching.RemoveListener(OnTriggerZoneUntouched);

    }

    private void OnTriggerZoneTouched()
    {
        if (lockX)
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
        }

        if (lockY)
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        }

        if (lockZ)
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;

        }

        if(lockY && lockX && lockZ)
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        }
    }
}
