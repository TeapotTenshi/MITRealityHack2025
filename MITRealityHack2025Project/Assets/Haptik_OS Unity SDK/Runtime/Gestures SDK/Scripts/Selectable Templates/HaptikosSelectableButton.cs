using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Haptikos.Gloves;
using UnityEngine.Events;
using Haptikos.Exoskeleton;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(RectTransform))]

public class HaptikosSelectableButton : HaptikosSimpleGloveSelectable
{
    public bool resizeCollider;

    [HideInInspector]
    public GameObject button;
    [HideInInspector]
    public List<PointerEvent> events = new();
    List<HaptikosSelectableButton> buttons;
    Collider buttonCollider;

    [HideInInspector]
    public float width = 1;

    HapticFeedbackUI hapticFeedback;

    private void Awake()
    {
        if (FindObjectOfType<HaptikosRaycastToMouseConverter>() == null)
        {
            if(FindObjectOfType<EventSystem>() != null)
            {
                Debug.LogWarning("You already have an event system. Use the Haptikos Event System Gameobject to support both the Unity UI and Haptikos UI");
            }

            Instantiate(HaptikosResources.Instance.haptikosEventSystem);
        }

        hapticFeedback = FindAnyObjectByType<HapticFeedbackUI>();
        if (hapticFeedback == null)
        {
            clickFeedback = false;
            hoverFeedback = false;
            Debug.Log("No Haptic feedback UI script found in the scene. It should be attached to the haptikos player");
        }
    }

    void OnValidate()
    {
        button = GetComponent<Button>().gameObject;
        if (button == null)
        {
            Debug.LogWarning("No button found");
        }
        buttonCollider = GetComponent<Collider>();
        buttonCollider.isTrigger = true;
        if (resizeCollider)
        {
            resizeColliderFunc();
        }
    }

    new void OnEnable()
    {
        base.OnEnable();
        OnClick.AddListener(OnClickUIEvent);
        OnClick.AddListener(InvokeClickFeedback);
        OnClickRelease.AddListener(OnClickReleaseUIEvent);
        OnHoverEnter.AddListener(OnHoverEnterUIEvent);
        OnHoverEnter.AddListener(InvokeHoverFeedback);
        OnHoverExit.AddListener(OnHoverExitUIEvent);
        buttons = FindAnyObjectByType<HaptikosRaycastToMouseConverter>().buttons;
        buttons.Add(this);
        if (resizeCollider)
        {
            resizeColliderFunc();
        }
    }

    public void resizeColliderFunc()
    {
        {
            BoxCollider buttonCollider = GetComponent<BoxCollider>();
            if (buttonCollider == null)
            {
                Debug.LogWarning("Button does not have a box collider, it cannot be resized automatically");
                resizeCollider = false;
            }
            else
            {
                RectTransform rect = GetComponent<RectTransform>();
                buttonCollider.size = new Vector3(rect.rect.width, rect.rect.height, width);
                buttonCollider.center = Vector3.zero;
            }
        }
    }

    new void OnDisable()
    {
        base.OnDisable();
        OnClick.RemoveListener(OnClickUIEvent);
        OnClick.RemoveListener(InvokeClickFeedback);
        OnClickRelease.RemoveListener(OnClickReleaseUIEvent);
        OnHoverEnter.RemoveListener(OnHoverEnterUIEvent);
        OnHoverEnter.RemoveListener(InvokeHoverFeedback);
        OnHoverExit.RemoveListener(OnHoverExitUIEvent);
    }

    void OnClickUIEvent(HaptikosExoskeleton hand)
    {
        events.Add(PointerEvent.pointerClick);
    }

    void OnClickReleaseUIEvent(HaptikosExoskeleton hand)
    {
        events.Add(PointerEvent.pointerUp);
    }

    void OnHoverEnterUIEvent(HaptikosExoskeleton hand)
    {
        events.Add(PointerEvent.pointerEnter);
    }

    void OnHoverExitUIEvent(HaptikosExoskeleton hand)
    {
        events.Add(PointerEvent.pointerExit);
    }
    
    void InvokeClickFeedback(HaptikosExoskeleton hand)
    {
        if (clickFeedback)
        {
            hapticFeedback.ButtonFeedback(hand, true);
        }
    }

    void InvokeHoverFeedback(HaptikosExoskeleton hand)
    {
        if (hoverFeedback)
        {
            hapticFeedback.ButtonFeedback(hand, false);
        }
    }
}

public enum PointerEvent
{
    pointerExit,
    pointerEnter,
    pointerUp,
    pointerClick
}
