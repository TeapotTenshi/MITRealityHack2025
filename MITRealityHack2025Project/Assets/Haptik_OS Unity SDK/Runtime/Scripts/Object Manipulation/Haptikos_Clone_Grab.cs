using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;


public class Haptikos_Clone_Grab : HapticItem
{
    public bool palmNeeded = false;
    List<HandPart> parts = new List<HandPart>();
    public bool thumb_part = false;
    public bool other_part = false;
    public bool index_part = false;
    public bool palm_part = false;
    private bool grabbed = false;
    private GameObject hand = null;



    private void Update()
    {
        if (!grabbed && thumb_part && other_part && (!palmNeeded || palm_part))
        {
            this.transform.parent.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            this.transform.parent.GetComponent<Rigidbody>().isKinematic = true;
            this.transform.parent.transform.parent = hand.transform;

            grabbed = true;

        }
        
        if(grabbed && (!thumb_part || !other_part || (palmNeeded && !palm_part)))
        {
            this.transform.parent.transform.parent = null;
            this.transform.parent.GetComponent<Rigidbody>().isKinematic = false;
            this.transform.parent.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;

            grabbed = false;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        HandPart hp = other.gameObject.GetComponent<HandPart>();
        if (hp != null && !parts.Contains(hp))
        {
            if (!palmNeeded)
            {
                // Only 2 last parts of every finger are acceptable
                if (hp.Type != Hand_Part_Type.Palm && hp.Type != Hand_Part_Type.Finger_Base)
                {
                    parts.Add(hp);
                    hand = hp.ParentHand.gameObject;

                    if (!thumb_part && (hp.Name == "thumb2" || hp.Name == "thumb3"))
                    {
                        thumb_part = true;
                    }
                    else if (!other_part && hp.Name != "thumb2" && hp.Name != "thumb3")
                    {
                        other_part = true;
                    }
                }
                onHapticFeedbackStarted?.Invoke(true, hp.Name, hp.ParentHand.hand.HandType);
            }
            else
            {
                // Only 2 last parts of every finger and palm ("wrist") are acceptable
                if (hp.Type != Hand_Part_Type.Finger_Base)
                {
                    parts.Add(hp);
                    hand = hp.ParentHand.gameObject;

                    if (!thumb_part && (hp.Name == "thumb2" || hp.Name == "thumb3"))
                    {
                        thumb_part = true;
                    }
                    else if (!other_part && hp.Name != "thumb2" && hp.Name != "thumb3" && !hp.Name.Contains("wrist"))
                    {
                        other_part = true;
                    }
                    else if(!palm_part && hp.Name.Contains("wrist"))
                    {
                        palm_part = true;
                    }
                    onHapticFeedbackStarted?.Invoke(true, hp.Name, hp.ParentHand.hand.HandType);
                }
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        HandPart hp = other.gameObject.GetComponent<HandPart>();
        if (hp != null && parts.Contains(hp))
        {
            parts.Remove(hp);

            if (hp.Name == "thumb2" || hp.Name == "thumb3")
            {
                if (thumb_part)
                {
                    if (!(parts.Exists(x => x.Name == "thumb2") || parts.Exists(x => x.Name == "thumb3")))
                        thumb_part = false;
                }
            }
            else if (hp.Name.Contains("wrist"))
            {
                palm_part = false;
            }
            else
            {
                if (other_part)
                {
                    int sum = 0;
                    if (parts.Exists(x => x.Name == "thumb2"))
                        sum++;
                    if (parts.Exists(x => x.Name == "thumb3"))
                        sum++;
                    if (parts.Exists(x => x.Name.Contains("wrist")))
                        sum++;

                    if (parts.Count <= sum)
                        other_part = false;
                }
            }
            onHapticFeedbackStarted?.Invoke(false, hp.Name, hp.ParentHand.hand.HandType);
        }
    }
}
