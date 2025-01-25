using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;

public abstract class HaptikosSelectable : MonoBehaviour
{
    public Action<HaptikosRaycast, HaptikosExoskeleton> Click;
    public Action<HaptikosRaycast, HaptikosExoskeleton> ClickRelease;
    public Action<HaptikosRaycast, HaptikosExoskeleton> HoverEnter;
    public Action<HaptikosRaycast, HaptikosExoskeleton> HoverExit;
   

    public UnityEvent<HaptikosExoskeleton> OnClick;
    public UnityEvent<HaptikosExoskeleton> OnClickRelease;
    public UnityEvent<HaptikosExoskeleton> OnHoverEnter;
    public UnityEvent<HaptikosExoskeleton> OnHoverExit;

    public bool clickFeedback;
    public bool hoverFeedback;

    protected virtual void OnEnable()
    {
        Click += ClickHandler;
        ClickRelease += ClickReleaseHandler;
        HoverEnter += HoverEnterHandler;
        HoverExit += HoverExitHandler;
    }

    protected virtual void OnDisable()
    {
        Click -= ClickHandler;
        ClickRelease -= ClickReleaseHandler;
        HoverEnter -= HoverEnterHandler;
        HoverExit -= HoverExitHandler;
    }

    protected abstract void ClickHandler(HaptikosRaycast raycast, HaptikosExoskeleton hand);

    protected abstract void ClickReleaseHandler(HaptikosRaycast raycast, HaptikosExoskeleton hand);

    protected abstract void HoverEnterHandler(HaptikosRaycast raycast, HaptikosExoskeleton hand);

    protected abstract void HoverExitHandler(HaptikosRaycast raycast, HaptikosExoskeleton hand);

}
