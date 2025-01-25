using Haptikos.Exoskeleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackManager : HapticItem
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
            onHapticFeedbackStarted?.Invoke(true, hp.Name, hp.ParentHand.hand.HandType);

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
            onHapticFeedbackStarted?.Invoke(false, hp.Name, hp.ParentHand.hand.HandType);


            //if (exitRoutine == null)
            //{
            //    exitRoutine = StartCoroutine(DelayedExitRoutine(hp.Name, hp.ParentHand.hand.HandType));
            //}
        }


    }

    private IEnumerator DelayedExitRoutine(string other, HandType type)
    {
        yield return new WaitForSeconds(exitDelay);

        if (isCurrentlyTriggered)
        {
            isCurrentlyTriggered = false; // Mark as not triggered
            TriggerHaptics(false, other, type);

            //if (parts.Count == 0)
               // onHapticFeedbackStartAndEnd?.Invoke(true, lastTouchedHandPart.Name, lastTouchedHandPart.ParentHand.hand.HandType, true);
        }
        exitRoutine = null; // Clear the coroutine
    }

    private void TriggerHaptics(bool activate, string other, HandType type)
    {
        Debug.Log(activate);
        if (activate)
        {
            onHapticFeedbackStarted?.Invoke(activate, other, type);
            isButtonExit = false;
        }
        else
        {
            onHapticFeedbackStarted?.Invoke(activate, other, type);
            isButtonExit = true;
        }
    }

}
