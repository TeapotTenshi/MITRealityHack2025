using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptikos.Gloves;
using UnityEditor;
using Unity.VisualScripting;
using Haptikos.Exoskeleton;

public class HaptikosHandPoseTransformRecognizer
{
    HaptikosTransformPreset preset;
    HandType handType;
    Transform handTransform;
    Transform handPositionTransform;
    Vector3 handCameraAxis;
    Vector3 handWorldAxis;
    public float angleFromWorldAxis;
    public float angleFromCameraAxis;
    public float xAngle;
    public float yAngle;
    bool recognized;
    Transform mainCamera;
    float factor;
    

    public HaptikosHandPoseTransformRecognizer(HaptikosTransformPreset _preset, Transform _camera, HaptikosExoskeleton _hand, HandType _handType)
    {
        preset = _preset;
        mainCamera = _camera;
        handTransform = _hand.transform;
        handPositionTransform = handTransform.GetChild(0).GetChild(0).GetChild(2);
        handType = _handType;
       
        if(handType  == HandType.RightHand)
        {
            factor = -1;
        } 
        else if(handType == HandType.LeftHand)
        {
            factor = 1;
        }
    }

  
    public bool Check()
    {
        SetHandAxes();
        CheckTransform();
        return recognized;
    }

    private void CheckTransform()
    {
        if (preset.checkAgaintsWorldUp)
        {
            angleFromWorldAxis = Vector3.Angle(handWorldAxis, Vector3.up);
        }

        if(preset.checkAgaintsCamera)
        {
           angleFromCameraAxis = Vector3.Angle(handCameraAxis, mainCamera.forward);
        }

       
        recognized = (!preset.checkAgaintsWorldUp || ((angleFromWorldAxis >= preset.minWorldAngle) && (angleFromWorldAxis <= preset.maxWorldAngle))) && (!preset.checkAgaintsCamera || ((angleFromCameraAxis >= preset.minCameraAngle) && (angleFromCameraAxis <= preset.maxCameraAngle)));
        if (!preset.checkFov)
        {
            return;
        } 
        
        Vector3 relativeHandPosition = handPositionTransform.position - mainCamera.position;
        Vector3 projectedHandPosition = Vector3.ProjectOnPlane(relativeHandPosition, mainCamera.forward);
        if(Vector3.Dot(mainCamera.forward,relativeHandPosition) <= 0)
        {
            recognized = false;
            return;
        }
        
        xAngle = Mathf.Asin(Vector3.Dot(projectedHandPosition, mainCamera.right)/relativeHandPosition.magnitude);
        xAngle *= Mathf.Rad2Deg;
        xAngle = Mathf.Abs(xAngle);
        yAngle = Mathf.Asin(Vector3.Dot(projectedHandPosition, mainCamera.up)/relativeHandPosition.magnitude);
        yAngle *= Mathf.Rad2Deg;
        yAngle = Mathf.Abs(yAngle);
        recognized = recognized && yAngle <= preset.fovVertical / 2 && xAngle <= preset.fovHorizontal / 2;
    }


    void SetHandAxes()
    {
        switch (preset.handWorldAxis)
        {
            case HandAxis.wrist:
                handWorldAxis = factor * handTransform.right;
                break;

            case HandAxis.fingers:
                handWorldAxis = handTransform.forward;
                break;
            case HandAxis.palm:
                handWorldAxis = -handTransform.up;
                break;
        }

        switch (preset.handCameraAxis)
        {
            case HandAxis.wrist:
                handCameraAxis = factor * handTransform.right;
                break;

            case HandAxis.fingers:
                handCameraAxis = handTransform.forward;
                break;
            case HandAxis.palm:
                handCameraAxis = -handTransform.up;
                break;
        }
    }

}

//[CustomEditor(typeof(HaptikosHandPoseTransformRecognizer))]

//public class HaptikHandPoseTransformRecognizerEditor : Editor
//{
//    HaptikosHandPoseTransformRecognizer recognizer;
//    HandType selectedType = HandType.RightHand;
//    float width = 100f;

//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();
//        recognizer = (HaptikosHandPoseTransformRecognizer)target;

//        if (!recognizer.handSelected)
//        {
//            EditorGUILayout.BeginHorizontal();
//            bool select = GUILayout.Button("Select Hand", GUILayout.Width(width));
//            selectedType = (HandType)EditorGUILayout.EnumPopup(selectedType, GUILayout.Width(width));
//            EditorGUILayout.EndHorizontal();
//            if (select)
//            {
//                if (selectedType == HandType.LeftHand)
//                {
//                    recognizer.hand = GameObject.FindGameObjectWithTag("Left Hand");
//                }
//                else if (selectedType == HandType.RightHand)
//                {
//                    recognizer.hand = GameObject.FindGameObjectWithTag("Right Hand");
//                }
//                if (recognizer.hand == null)
//                {
//                    Debug.Log("Insert haptik player with a " + selectedType.ToString());
//                }
//                recognizer.OnValidate();
//            }
//        }
//    }
//}
