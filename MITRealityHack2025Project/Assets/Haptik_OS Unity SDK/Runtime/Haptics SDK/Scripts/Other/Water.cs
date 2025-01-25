using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;
using Haptikos;

public class Water : HapticItem
{
    [HideInInspector]
    public HaptikosExoskeleton leftGlove, rightGlove;
    List<HandPart> parts = new List<HandPart>();

    private void Start()
    {
        leftGlove = HaptikosPlayer.GetExoskeleton(HandType.LeftHand);
        rightGlove = HaptikosPlayer.GetExoskeleton(HandType.RightHand);
    }

    private void OnTriggerEnter(Collider collider)
    {
        HandPart hp = collider.gameObject.GetComponent<HandPart>();

        if (hp != null && !parts.Contains(hp))
        {
            onHapticFeedbackStarted?.Invoke(true, hp.Name, hp.ParentHand.hand.HandType);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        HandPart hp = collider.gameObject.GetComponent<HandPart>();

        if (hp != null && !parts.Contains(hp))
        {
            onHapticFeedbackStarted?.Invoke(false, hp.Name, hp.ParentHand.hand.HandType);
        }
    }
}
