using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;

[RequireComponent(typeof(HaptikosRaycast))]
public abstract class HaptikosRayCastController : MonoBehaviour
{
    protected HaptikosRaycast raycast;
    protected HaptikosExoskeleton hand;
    protected float factor;

    protected void CheckComponets()
    {
        raycast = GetComponent<HaptikosRaycast>();
        if (raycast == null)
        {
            Debug.Log("No Haptikos Raycast Found");
            enabled = false;
            return;
        }
        if (!raycast.clickRay && !raycast.hoverRay)
        {
            Debug.Log("Both Hover and Click ray are disabled");
            enabled = false;
            return;
        }
        hand = raycast.Hand;
        if (hand == null)
        {
            Debug.Log("No target hand found");
            enabled = false;
            return;
        }

        if (hand.hand.HandType == HandType.LeftHand)
        {
            factor = -1;
        }
        else if (hand.hand.HandType == HandType.RightHand)
        {
            factor = 1;
        }
        else
        {
            Debug.Log("Hand game object is not a Haptikos Hand");
            hand = null;
            enabled = false;
            return;
        }
    }

    abstract protected void Start();

    abstract protected void Update();

}
