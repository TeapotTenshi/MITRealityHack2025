using Haptikos.Exoskeleton;
using Haptikos.Gloves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HaptikosSimpleSelectable : HaptikosSelectable
{
    int clickCounter = 0;
    int hoverCounter = 0;
    int prevClickCounter = 0;
    int prevHoverCounter = 0;
    HaptikosExoskeleton currentHand;

    protected override void ClickHandler(HaptikosRaycast raycast, HaptikosExoskeleton hand)
    {
        currentHand = hand;
        clickCounter += 1;
    }

    protected override void ClickReleaseHandler(HaptikosRaycast raycast, HaptikosExoskeleton hand)
    {
        currentHand = hand;
        clickCounter -= 1;
    }

    protected override void HoverEnterHandler(HaptikosRaycast raycast, HaptikosExoskeleton hand)
    {
        currentHand = hand;
        hoverCounter += 1;
    }

    protected override void HoverExitHandler(HaptikosRaycast raycast, HaptikosExoskeleton hand)
    {
        currentHand = hand;
        hoverCounter -= 1;
    }


    protected void LateUpdate()
    {
        if (prevClickCounter == 0 && clickCounter > 0)
        {
            OnClick?.Invoke(currentHand);
        }

        else if (prevClickCounter > 0 && clickCounter == 0)
        {
            OnClickRelease?.Invoke(currentHand);
        }
    
        if (prevHoverCounter == 0 && hoverCounter > 0)
        {
            OnHoverEnter?.Invoke(currentHand);
        }

        else if(prevHoverCounter > 0 && hoverCounter == 0)
        {
            OnHoverExit?.Invoke(currentHand);
        }

        prevClickCounter = clickCounter;
        prevHoverCounter = hoverCounter;
    }

    new protected void OnEnable()
    {
        base.OnEnable();
        clickCounter = 0;
        hoverCounter = 0;
    }
}
