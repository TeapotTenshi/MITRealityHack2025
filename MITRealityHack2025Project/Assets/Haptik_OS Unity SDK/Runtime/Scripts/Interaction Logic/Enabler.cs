using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionDetector))]
public class Enabler : MonoBehaviour
{

    [SerializeField] GameObject gameobjectToInteractWith;
    [SerializeField] bool enableOnTouch;

    InteractionDetector interactionDetector;

    private void Awake()
    {
        interactionDetector = GetComponent<InteractionDetector>();    
    }

    private void OnEnable()
    {
        interactionDetector.onObjectStartTouching.AddListener(()=> EnableAccordingToTouch());
        interactionDetector.onObjectStoppedTouching.AddListener(()=> EnableAccordingToTouch());
    }

    private void OnDisable()
    {
        interactionDetector.onObjectStartTouching.AddListener(() => EnableAccordingToTouch());
        interactionDetector.onObjectStoppedTouching.AddListener(() => EnableAccordingToTouch());

    }

    void EnableAccordingToTouch()
    {
        if (enableOnTouch)
        {
            gameobjectToInteractWith.SetActive(true);
        }
        else
        {
            gameobjectToInteractWith.SetActive(false);
        }

        enableOnTouch = !enableOnTouch;
    }
}
