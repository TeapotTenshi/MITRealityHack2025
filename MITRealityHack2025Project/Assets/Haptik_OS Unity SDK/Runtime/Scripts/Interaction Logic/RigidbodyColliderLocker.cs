using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionDetector))]
[RequireComponent(typeof(ColliderChanger))]
public class RigidbodyColliderLocker : MonoBehaviour
{
    InteractionDetector interactionStabilizer;
    ColliderChanger colliderChanger;

    // Start is called before the first frame update
    void Awake()
    {
        interactionStabilizer = GetComponent<InteractionDetector>();
        colliderChanger = GetComponent<ColliderChanger>();
    }

    private void OnEnable()
    {
        interactionStabilizer.onObjectStartTouching.AddListener(colliderChanger.ChangeCollider);
        interactionStabilizer.onObjectStoppedTouching.AddListener(colliderChanger.ChangeCollider);
    }

    private void OnDisable()
    {
        interactionStabilizer.onObjectStartTouching.RemoveListener(colliderChanger.ChangeCollider);
        interactionStabilizer.onObjectStoppedTouching.RemoveListener(colliderChanger.ChangeCollider);
    }
}
