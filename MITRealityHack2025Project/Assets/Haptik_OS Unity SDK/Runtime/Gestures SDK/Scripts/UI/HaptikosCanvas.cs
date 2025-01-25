using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HaptikosCanvas : MonoBehaviour
{
    public float width = 1;
    public bool enableCollider;
    public bool resizeCollider;
    BoxCollider canvasCollider;
    private void OnValidate()
    {
        canvasCollider = GetComponent<BoxCollider>();
        canvasCollider.isTrigger = true;
        if (!enableCollider)
        {
            canvasCollider.enabled = false;
        }
        else if (resizeCollider)
        {
            canvasCollider.enabled = true;
            if (resizeCollider)
            {
                
                if (canvasCollider == null)
                {
                    Debug.LogWarning("Canvas does not have a box collider, it cannot be resized automatically");
                    resizeCollider = false;
                }
                else
                {
                    RectTransform rect = GetComponent<RectTransform>();
                    canvasCollider.size = new Vector3(rect.rect.width, rect.rect.height, width/2);
                    canvasCollider.center = Vector3.zero;
                }
            }
        }
        HaptikosSelectableButton[] buttons = GetComponentsInChildren<HaptikosSelectableButton>(true);
        foreach(HaptikosSelectableButton button in buttons)
        {
            button.width = width;
        }
    }

    private void OnEnable()
    {
        OnValidate();
    }
}
