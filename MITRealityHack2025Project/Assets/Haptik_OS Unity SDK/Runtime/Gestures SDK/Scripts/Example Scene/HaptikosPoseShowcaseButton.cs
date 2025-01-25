using Haptikos.Exoskeleton;
using Haptikos.Gloves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HaptikosGestureRecognizer))]
[RequireComponent(typeof(HaptikosSelectableButton))]
public class HaptikosPoseShowcaseButton : MonoBehaviour
{
    HaptikosGestureRecognizer recognizer;
    HaptikosSelectableButton selectableButton;
    Button button;
    Image image;
    // Start is called before the first frame update
    void Awake()
    {
        recognizer = GetComponent<HaptikosGestureRecognizer>();
        selectableButton = GetComponent<HaptikosSelectableButton>();
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        selectableButton.OnClick.AddListener(Click);
        recognizer.OnActivate.AddListener(PoseActive);
        recognizer.OnDeactivate.AddListener(PoseInsactive);
        image.color = (recognizer.Activated) ? Color.green : Color.red;
    }

    private void OnDisable()
    {
        selectableButton.OnClick.RemoveListener(Click);
        recognizer.OnActivate.RemoveListener(PoseActive);
        recognizer.OnDeactivate.RemoveListener(PoseInsactive);
        image.color = Color.gray;
    }
    void Click(HaptikosExoskeleton hand)
    {
        recognizer.enabled = !recognizer.enabled;
        image.color = (recognizer.enabled) ? Color.red : Color.gray;
    }
    
    void PoseActive(HaptikosExoskeleton hand)
    {
        image.color = Color.green;
    }

    void PoseInsactive(HaptikosExoskeleton hand)
    {
        image.color = Color.red;
    }
}
