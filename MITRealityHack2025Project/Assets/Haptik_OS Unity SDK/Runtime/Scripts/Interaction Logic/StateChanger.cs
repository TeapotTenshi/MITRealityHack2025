using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Use this class in order for an object to change states
/// </summary>
[RequireComponent(typeof(InteractionDelayer))]
[RequireComponent(typeof(InteractionDetector))]
public class StateChanger : MonoBehaviour
{
    [Header("ON State")]
    [SerializeField] Transform objectOnTransform;

    [Space(2)]
    [Header("OFF State")]
    [SerializeField] Transform objectOffTransform;

    [Space(2)]
    [Header("Starting state")]
    [SerializeField] bool onState;

    [Space(2)]
    public UnityEvent<bool> onStateChanged;

    InteractionDetector interactionDetector;
    InteractionDelayer delayer;


    public bool OnState { get => onState; set => onState = value; }
    public Transform ObjectOnTransform { get => objectOnTransform; set => objectOnTransform = value; }
    public Transform ObjectOffTransform { get => objectOffTransform; set => objectOffTransform = value; }

    private void Awake()
    {
        interactionDetector = GetComponent<InteractionDetector>();
        delayer = GetComponent<InteractionDelayer>();
    }

    private void OnEnable()
    {
        SwitchState();

        interactionDetector.onObjectStartTouching.AddListener(() =>
        {
            if (!delayer.StopInteracting)
            {
                SwitchState();
                delayer.DelayInteraction();
            }
        });
    }

    public void SwitchState()
    {
        if (OnState)
        {
            transform.position = ObjectOnTransform.position;
            transform.rotation = ObjectOnTransform.rotation;
        }
        else
        {
            transform.position = ObjectOffTransform.position;
            transform.rotation = ObjectOffTransform.rotation;
        }

        OnState = !OnState;

        onStateChanged?.Invoke(OnState);
    }
}
