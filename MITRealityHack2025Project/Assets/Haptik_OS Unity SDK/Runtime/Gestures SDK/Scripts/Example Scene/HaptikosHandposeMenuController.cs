using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Haptikos.Gloves;
using UnityEditor;
using Haptikos.Exoskeleton;

public class HaptikosHandposeMenuController : MonoBehaviour
{
    public HaptikosHandposeShape[] handposes;
    public HandType hand;

    [ContextMenu("Update Menu")]
    void updateMenu()
    {
        for (int i = 0; i < handposes.Length; i++)
        {
            updateButton(transform.GetChild(i), i);
        }
    }

    void updateButton(Transform button, int index)
    {
        TMP_Text text = button.GetComponentInChildren<TMP_Text>();
        text.text = handposes[index].name;
        button.name = text.text;
        EditorUtility.SetDirty(text);
        HaptikosGestureRecognizer recognizer = button.GetComponent<HaptikosGestureRecognizer>();
        recognizer.hand = hand;
        recognizer.poseRecognition = true;
        recognizer.transformRecognition = recognizer.transformPreset != null;
        recognizer.reversePoseRecognition = false;
        recognizer.reverseTransformRecognition = false;
        recognizer.timeToActivate = 0.05f;
        recognizer.timeToDeactivate = 0.05f;
        recognizer.visualizeTimeToActivate = false;
        recognizer.visualizeTimeToDeactivate = false;
        recognizer.targetHandPose = handposes[index];
    }
}
