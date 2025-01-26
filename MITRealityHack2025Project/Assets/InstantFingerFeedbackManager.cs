using Haptikos.Exoskeleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantFingerFeedbackManager : HapticItem
{
    //public HapticFeedback hapticFeedback;
    public CustomVibrationCurve hapticOne;
    public CustomVibrationCurve hapticTwo;
    List<HandPart> parts = new List<HandPart>();
    List<Collider> colliderCollection = new List<Collider>();

   // public bool isButtonExit = false;
   // private bool isCurrentlyTriggered = false;
    //private float exitDelay = 0.2f; // Time to wait before considering it fully exited
   // private Coroutine exitRoutine = null;
    //HandPart lastTouchedHandPart;

    

    // Start is called before the first frame update
    void Start()
    {
        //hapticFeedback = GetComponent<HapticFeedback>();
        //hapticFeedback.isContinuous = true;
        //GetComponent<Collider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        HandPart hp = gameObject.GetComponent<HandPart>();
        Collider otherCollider = other.GetComponent<Collider>();
        //lastTouchedHandPart = hp;

        if (otherCollider != null)
        {
            if (!colliderCollection.Contains(otherCollider))
            {
                colliderCollection.Add(otherCollider);
            }

            int priority = 0;

            foreach (Collider collider in colliderCollection)
            {
                
                switch(otherCollider.tag)
                {
                    case "corner":
                        priority = 1;
                        break;

                    case "stan":
                        if (priority == 1)
                        {
                            //do nothing
                        }
                        else
                        {
                            priority = 2;
                        }
                        break;

                    default:

                        break;
                }
            }

            if (priority == 1)
            {
                //select the haptic to play
               // otherCollider.GetComponent<HapticFeedback>().curve = hapticOne;
                onHapticFeedbackStartAndEnd?.Invoke(true, hp.Name, hp.ParentHand.hand.HandType, true);
            }
            else if (priority == 2)
            {
                //select the haptic
                otherCollider.GetComponent<HapticFeedback>().curve = hapticTwo;
                onHapticFeedbackStartAndEnd?.Invoke(true, hp.Name, hp.ParentHand.hand.HandType, true);
            }


        }



        //HandPart hp = other.gameObject.GetComponent<HandPart>();
        //lastTouchedHandPart = hp;

        //if (hp != null && !parts.Contains(hp))
        //{
        //    parts.Add(hp);

        //    //if (exitRoutine != null)
        //    //{
        //    //    StopCoroutine(exitRoutine); // Cancel any pending exit routine
        //    //    exitRoutine = null;
        //    //}
        //    onHapticFeedbackStartAndEnd?.Invoke(true, hp.Name, hp.ParentHand.hand.HandType, true);

        //    //TriggerHaptics(true, hp.Name, hp.ParentHand.hand.HandType);


        //}
    }

    private void OnTriggerExit(Collider other)
    {
        HandPart hp = gameObject.GetComponent<HandPart>();
        //lastTouchedHandPart = hp;

        Collider otherCollider = other.GetComponent<Collider>();

        if (colliderCollection.Contains(otherCollider))
        {
            colliderCollection.Remove(otherCollider);
        }

        //if (hp != null && parts.Contains(hp))
        //{
        //    parts.Remove(hp);
        //    //TriggerHaptics(false, hp.Name, hp.ParentHand.hand.HandType);
        //    onHapticFeedbackStartAndEnd?.Invoke(false, hp.Name, hp.ParentHand.hand.HandType, true);


        //    //if (exitRoutine == null)
        //    //{
        //    //    exitRoutine = StartCoroutine(DelayedExitRoutine(hp.Name, hp.ParentHand.hand.HandType));
        //    //}
        //}


    }
}
