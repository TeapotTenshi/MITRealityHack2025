using Haptikos.Exoskeleton;
using Haptikos.Gloves;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Example of Switch, it requires 3 components and it works automatically
/// Use the RequireComponent to get all the components you want from this script
/// </summary>
[RequireComponent(typeof(StateChanger))]
[RequireComponent(typeof(InteractionDetector))]
[RequireComponent(typeof(InteractionDelayer))]
public class Haptikos_Switch : HapticItem
{
    [Header("Interaction Parameters")]
    public Hand_Part_Type interactionPart;
    public float delay;
    public bool onState;

    [Space(2)]
    [Header("States")]
    [SerializeField] Transform onStateTransform;
    [SerializeField] Transform offStateTransform;

    StateChanger changer;
    InteractionDelayer delayer;
    InteractionDetector interactionDetector;

    HandType type => interactionDetector.LastTouchedFinger.ParentHand.hand.HandType;
    string fingerName => interactionDetector.LastTouchedFinger.Name;

    [ExecuteInEditMode]
    private void OnValidate()
    {
        GetComponents();

        changer.OnState = onState;
        delayer.DelayTime = delay;
        interactionDetector.InteractionPart = interactionPart;

        changer.ObjectOnTransform = onStateTransform;
        changer.ObjectOffTransform = offStateTransform;
    }

    private void GetComponents()
    {
        if (changer == null)
            changer = GetComponent<StateChanger>();

        if (delayer == null)
            delayer = GetComponent<InteractionDelayer>();

        if (interactionDetector == null)
            interactionDetector = GetComponent<InteractionDetector>();
    }

    private void Awake()
    {
        GetComponents();
    }

    private void Start()
    {
        changer.onStateChanged.AddListener((state) =>
        {
            onHapticFeedbackStarted?.Invoke(true, fingerName, type);

            StartCoroutine(InvokeWithDelay(0.5f));

        });
    }

    private IEnumerator InvokeWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        onHapticFeedbackStarted?.Invoke(false, fingerName, type);
    }
}
