using Haptikos.Exoskeleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onetimefeedback : HapticItem
{

    public HapticFeedback hapticFeedback;
    List<HandPart> parts = new List<HandPart>();

    public bool isButtonExit = false;
    private bool isCurrentlyTriggered = false;
    private float exitDelay = 0.2f; // Time to wait before considering it fully exited
    private Coroutine exitRoutine = null;
    HandPart lastTouchedHandPart;

    // Start is called before the first frame update
    void Start()
    {
        hapticFeedback = GetComponent<HapticFeedback>();
        hapticFeedback.isContinuous = true;
        GetComponent<Collider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        HandPart hp = other.gameObject.GetComponent<HandPart>();
        lastTouchedHandPart = hp;

        if (hp != null && !parts.Contains(hp))
        {
            parts.Add(hp);

            //if (exitRoutine != null)
            //{
            //    StopCoroutine(exitRoutine); // Cancel any pending exit routine
            //    exitRoutine = null;
            //}
            onHapticFeedbackStartAndEnd?.Invoke(true, hp.Name, hp.ParentHand.hand.HandType,true);

            //TriggerHaptics(true, hp.Name, hp.ParentHand.hand.HandType);


        }
    }

    private void OnTriggerExit(Collider other)
    {
        HandPart hp = other.gameObject.GetComponent<HandPart>();
        lastTouchedHandPart = hp;

        if (hp != null && parts.Contains(hp))
        {
            parts.Remove(hp);
            //TriggerHaptics(false, hp.Name, hp.ParentHand.hand.HandType);
            onHapticFeedbackStartAndEnd?.Invoke(false, hp.Name, hp.ParentHand.hand.HandType,true);


            //if (exitRoutine == null)
            //{
            //    exitRoutine = StartCoroutine(DelayedExitRoutine(hp.Name, hp.ParentHand.hand.HandType));
            //}
        }


    }

}
