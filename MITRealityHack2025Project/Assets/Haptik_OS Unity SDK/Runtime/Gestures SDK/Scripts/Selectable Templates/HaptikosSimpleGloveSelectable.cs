using Haptikos.Exoskeleton;
using Haptikos.Gloves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaptikosSimpleGloveSelectable : HaptikosSimpleSelectable
{
    LayerMask handsMask;

    void Start()
    {
        handsMask = LayerMask.GetMask("Haptikos Hands");
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.transform.gameObject.layer) & handsMask) != 0)
        {
            HaptikosExoskeleton hand = other.GetComponent<HandPart>().ParentHand;
            Click.Invoke(null, hand);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (((1 << other.transform.gameObject.layer) & handsMask) != 0)
        {
            HaptikosExoskeleton hand = other.GetComponent<HandPart>().ParentHand;
            ClickRelease.Invoke(null, hand);
        }
    }
}
