using Haptikos.Exoskeleton;
using Haptikos.Gloves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaptikosSelectableFeedbackController : MonoBehaviour
{
    HapticFeedbackSelectables hapticFeedbackSelectables;
    HaptikosSelectable selectable;
    public bool feedbackOnClick, feedbackOnClickRelease, feedbackOnHoverEnter, feedbackOnHoverExit;

    private void Awake()
    {
        hapticFeedbackSelectables = GetComponent<HapticFeedbackSelectables>();
        selectable = GetComponent<HaptikosSelectable>();
    }

    private void OnEnable()
    {
        if (feedbackOnClick)
        {
            selectable.OnClick.AddListener(ClickFeedback);
        }

        if (feedbackOnClickRelease)
        {
            selectable.OnClickRelease.AddListener(ClickReleaseFeedback);
        }

        if (feedbackOnHoverEnter)
        {
            selectable.OnHoverEnter.AddListener(HoverEnterFeedback);
        }

        if (feedbackOnHoverExit)
        {
            selectable.OnHoverExit.AddListener(HoverExitFeedback);
        }
    }


    private void OnDisable()
    {
        selectable.OnClick.RemoveListener(ClickFeedback);
        selectable.OnClickRelease.RemoveListener(ClickReleaseFeedback);
        selectable.OnHoverEnter.RemoveListener(HoverEnterFeedback);
        selectable.OnHoverExit.RemoveListener(HoverExitFeedback);
    }

    void ClickFeedback(HaptikosExoskeleton hand)
    {
        hapticFeedbackSelectables.SetCollisionState(true, hand, true);
    }

    void ClickReleaseFeedback(HaptikosExoskeleton hand)
    {
        hapticFeedbackSelectables.SetCollisionState(true, hand, true);
    }

    void HoverEnterFeedback(HaptikosExoskeleton hand)
    {
        hapticFeedbackSelectables.SetCollisionState(true, hand, true);
    }

    void HoverExitFeedback(HaptikosExoskeleton hand)
    {
        hapticFeedbackSelectables.SetCollisionState(true, hand, true);
    }
}
