using Haptikos.Exoskeleton;
using Haptikos.Gloves;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class HaptikosRaycastAwareSelectable : HaptikosSelectable 
{
    bool click = false;
    bool hover = false;
 
    protected HaptikosRaycast raycast;
    List<RaycastEvent> raycastEventsList = new();

    protected override void ClickHandler(HaptikosRaycast raycast, HaptikosExoskeleton hand)
    {
        RaycastEvent raycastInfo = new RaycastEvent(raycast, EventType.click);
        raycastEventsList.Add(raycastInfo);
    }

    protected override void ClickReleaseHandler(HaptikosRaycast raycast, HaptikosExoskeleton hand)
    {
        RaycastEvent raycastInfo = new RaycastEvent(raycast, EventType.clickRelease);
        raycastEventsList.Add(raycastInfo);
    }

    protected override void HoverEnterHandler(HaptikosRaycast raycast, HaptikosExoskeleton hand)
    {
        RaycastEvent raycastInfo = new RaycastEvent(raycast, EventType.hoverEnter);
        raycastEventsList.Add(raycastInfo);
    }


    protected override void HoverExitHandler(HaptikosRaycast raycast, HaptikosExoskeleton hand)
    {
        RaycastEvent raycastInfo = new RaycastEvent(raycast, EventType.hoverExit);
        raycastEventsList.Add(raycastInfo);
    }

    new protected void OnEnable()
    {
        base.OnEnable();
        click = false;
        hover = false;
        raycastEventsList = new();
        raycast = null;
    }

    void LateUpdate()
    {
        List<HaptikosRaycast> clickReleased = new();
        List<HaptikosRaycast> hoverExited = new();
        List<HaptikosRaycast> clickEntered = new();
        List<HaptikosRaycast> hoverEntered = new();

        int counter = raycastEventsList.Count;
        while(counter > 0)//Handle events that cancel each other(e.g clickRelease before click from the same Raycaster)
        {
            counter--;
            HaptikosRaycast currentRaycast = raycastEventsList[counter].raycast;
            EventType currentType = raycastEventsList[counter].type;
            if(currentType == EventType.clickRelease)
            {
                if (clickEntered.Contains(currentRaycast))
                {
                    raycastEventsList.RemoveAt(counter);
                }
                else
                {
                    clickReleased.Add(currentRaycast);
                }
            }
            if (currentType == EventType.click)
            {
                if (clickReleased.Contains(currentRaycast))
                {
                    raycastEventsList.RemoveAt(counter);
                }
                else
                {
                    clickEntered.Add(currentRaycast);
                }
            }
            else if (currentType == EventType.hoverExit)
            {
                if (hoverEntered.Contains(currentRaycast))
                {
                    raycastEventsList.RemoveAt(counter);
                }
                else
                {
                    hoverExited.Add(currentRaycast);
                }
            }
            else if (currentType == EventType.hoverEnter)
            {
                if (hoverExited.Contains(currentRaycast))
                {
                    raycastEventsList.RemoveAt(counter);
                }
                else
                {
                    hoverEntered.Add(currentRaycast);
                }
            }
        }

        if (raycast != null) //Handle events from the controller raycast
        {
            if (clickReleased.Contains(raycast))
            {
                if (click)
                {
                    OnClickRelease?.Invoke(raycast.Hand);
                }
                click = false;
                
            }
            else if (clickEntered.Contains(raycast))
            {
                if (!click)
                {
                    OnClick?.Invoke(raycast.Hand);
                }
                click = true;
            }

            if (hoverExited.Contains(raycast))
            {
                if (hover)
                {
                    OnHoverExit.Invoke(raycast.Hand);
                }
                hover = false;
            }
            else if (hoverEntered.Contains(raycast))
            {
                if (!hover)
                {
                    OnHoverEnter.Invoke(raycast.Hand);
                }
                hover = true;
            }

            if (!click && !hover)
            {
                raycast = null;
            }
        }


        if(raycast == null)//If the previoues controller is finished(exited and released) try to find a new raycast
        {
            counter = 0;
            while(counter < raycastEventsList.Count)
            {
                HaptikosRaycast currentRaycast = raycastEventsList[counter].raycast;
                EventType currentType = raycastEventsList[counter].type;
                if(raycast == null)
                {
                    if(currentType == EventType.click)
                    {
                        raycast = currentRaycast;
                        click = true;
                        OnClick?.Invoke(raycast.Hand);
                    }
                    else if(currentType == EventType.hoverEnter)
                    {
                        raycast = currentRaycast;
                        hover = true;
                        OnHoverEnter?.Invoke(raycast.Hand);
                    }

                    raycastEventsList.RemoveAt(counter);
                }
                else
                {
                    if(currentType == EventType.clickRelease || currentType == EventType.hoverExit)
                    {
                        raycastEventsList.RemoveAt(counter);
                    }
                    else if(currentType == EventType.click)
                    {
                        if(raycast == currentRaycast)
                        {
                            click = true;
                            OnClick?.Invoke(raycast.Hand);
                            raycastEventsList.RemoveAt(counter);
                        }
                        else
                        {
                            counter++;
                        }
                    }
                    else if(currentType == EventType.hoverEnter)
                    {
                        if(raycast == currentRaycast)
                        {
                            hover = true;
                            OnHoverEnter?.Invoke(raycast.Hand);
                            raycastEventsList.RemoveAt(counter);
                        }
                        else
                        {
                            counter++;
                        }
                    }
                }
            }

        }
    }
    
    

    private struct RaycastEvent
    {
        public HaptikosRaycast raycast;
        public EventType type;
        public RaycastEvent(HaptikosRaycast _raycast, EventType _type)
        {
            raycast = _raycast;
            type = _type;
        }
    }

    private enum EventType
    {
        click,
        clickRelease,
        hoverEnter,
        hoverExit
    }
}
