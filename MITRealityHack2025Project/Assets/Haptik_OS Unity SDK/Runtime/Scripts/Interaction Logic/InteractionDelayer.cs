using CoolisCodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

/// <summary>
/// Delays the Interaction, to prevent multiple touches
/// </summary>
public class InteractionDelayer : MonoBehaviour
{
    [SerializeField] float delayTime = 0.25f;

    bool stopInteracting = false;

    float startingDelay;

    public bool StopInteracting
    {
        get { return stopInteracting; }
        set { stopInteracting = value; }
    }

    public float DelayTime { get => delayTime; set => delayTime = value; }

    public void DelayInteraction(float startingDelay = 0.5f)
    {
        DelayTime = startingDelay;
        stopInteracting = true;
    }

    private void Start()
    {
        startingDelay = delayTime;
    }

    private void FixedUpdate()
    {
        if (stopInteracting)
        {
            if (DelayTime > 0)
            {
                DelayTime -= Time.fixedDeltaTime;
                if (DelayTime <= 0)
                {
                    DelayTime = 0;
                    stopInteracting = false;
                }
            }
        }
    }
}
