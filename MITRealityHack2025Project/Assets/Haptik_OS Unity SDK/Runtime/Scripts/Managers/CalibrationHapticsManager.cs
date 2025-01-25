using Haptikos.Exoskeleton;
using Haptikos.Gloves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationHapticsManager : MonoBehaviour
{

    HapticFeedbackUI hapticFeedback;

    private void Awake()
    {
        hapticFeedback = FindAnyObjectByType<HapticFeedbackUI>();
        if (hapticFeedback == null)
        {
            Debug.Log("No Haptic feedback UI script found in the scene. It should be attached to the haptikos player");
        }
    }

    private void OnEnable()
    {
        CalibrationLoader.RecognizedThumbsUp.AddListener(CallOnThumbsUp);
        IMUCalibrationManager.StartedCalibration.AddListener(CallOnCalibration);
        ExampleSceneMenuController.OnRecognizedOpenHand.AddListener(CallOnOpenHand);
        ExampleSceneMenuController.OnMainMenuOpened.AddListener(CallOnMenuOpen);
    }


    private void OnDisable()
    {
        CalibrationLoader.RecognizedThumbsUp.RemoveListener(CallOnThumbsUp);
        IMUCalibrationManager.StartedCalibration.RemoveListener(CallOnCalibration);
        ExampleSceneMenuController.OnRecognizedOpenHand.RemoveListener(CallOnOpenHand);
        ExampleSceneMenuController.OnMainMenuOpened.RemoveListener(CallOnMenuOpen);
    }

    void CallOnThumbsUp(HaptikosExoskeleton hand)
    {
        hapticFeedback.ButtonFeedback(hand, true);
    }

    void CallOnCalibration(HaptikosExoskeleton hand)
    {
        hapticFeedback.ButtonFeedback(hand, true);
    }
    
    void CallOnOpenHand(HaptikosExoskeleton hand)
    {
        hapticFeedback.ButtonFeedback(hand, true);
    }

    void CallOnMenuOpen(HaptikosExoskeleton hand)
    {
        hapticFeedback.ButtonFeedback(hand, true);
    }
}
