using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HaptikosRaycastToMouseConverter : BaseInputModule
{ 
    PointerEventData mouseData;
    public List<HaptikosSelectableButton> buttons;
    
    new void Start()
    {
        base.Start();
        mouseData = new PointerEventData(eventSystem);
    }

    public override void Process()
    {
        foreach (HaptikosSelectableButton button in buttons)
        {
            while (button.events.Count > 0)
            {
                PointerEvent pointerEvent = button.events[0];
                button.events.RemoveAt(0);
                switch (pointerEvent)
                {
                    case PointerEvent.pointerEnter:
                        ExecuteEvents.Execute(button.button, mouseData, ExecuteEvents.pointerEnterHandler);
                        break;
                    case PointerEvent.pointerExit:
                        ExecuteEvents.Execute(button.button, mouseData, ExecuteEvents.pointerExitHandler);
                        ExecuteEvents.Execute(button.button, mouseData, ExecuteEvents.deselectHandler);
                        break;
                    case PointerEvent.pointerUp:
                        ExecuteEvents.Execute(button.button, mouseData, ExecuteEvents.pointerUpHandler);
                        ExecuteEvents.Execute(button.button, mouseData, ExecuteEvents.deselectHandler);
                        break;
                    case PointerEvent.pointerClick:
                        //mouseData.button = PointerEventData.InputButton.Left;
                        ExecuteEvents.Execute(button.button, mouseData, ExecuteEvents.pointerClickHandler);
                        ExecuteEvents.Execute(button.button, mouseData, ExecuteEvents.pointerDownHandler);
                        break;
                    default:
                        break;
                }
            }
            if (!button.enabled)
            {
                buttons.Remove(button);
            }
        }
    }
}
