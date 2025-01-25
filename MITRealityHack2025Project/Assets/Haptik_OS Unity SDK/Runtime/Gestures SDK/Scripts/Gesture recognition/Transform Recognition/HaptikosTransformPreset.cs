using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(menuName ="ScriptableObjects/Transform Preset")]
public class HaptikosTransformPreset : ScriptableObject
{
    
    public bool checkAgaintsWorldUp;
    public HandAxis handWorldAxis;
    public float minWorldAngle;
    public float maxWorldAngle;
    public bool checkAgaintsCamera;
    public HandAxis handCameraAxis;
    public float minCameraAngle;
    public float maxCameraAngle;
    public bool checkFov;
    [HideInInspector]
    public float fovHorizontal;
    [HideInInspector]
    public float fovVertical;
    
}


public enum HandAxis
{
    wrist,
    fingers,    
    palm
} 

[CustomEditor(typeof(HaptikosTransformPreset))]
public class HaptikHandposeTransformEditor : Editor
{
    HaptikosTransformPreset poseTransform;

    private void OnEnable()
    {
        poseTransform = (HaptikosTransformPreset)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.labelWidth = 150f;
        poseTransform.checkAgaintsWorldUp = EditorGUILayout.Toggle("Check Against World Up", poseTransform.checkAgaintsWorldUp);
        if(poseTransform.checkAgaintsWorldUp)
        {
            poseTransform.handWorldAxis = (HandAxis)EditorGUILayout.EnumPopup("HandAxis",poseTransform.handWorldAxis);
            poseTransform.minWorldAngle = EditorGUILayout.FloatField("Minimum Angle", poseTransform.minWorldAngle,GUILayout.ExpandWidth(false));
            poseTransform.maxWorldAngle = EditorGUILayout.FloatField("Maximum Angle", poseTransform.maxWorldAngle, GUILayout.ExpandWidth(false));
        }

        poseTransform.checkAgaintsCamera = EditorGUILayout.Toggle("Check Against Camera", poseTransform.checkAgaintsCamera);
        if (poseTransform.checkAgaintsCamera)
        {
            poseTransform.handCameraAxis = (HandAxis)EditorGUILayout.EnumPopup("HandAxis", poseTransform.handCameraAxis);
            poseTransform.minCameraAngle = EditorGUILayout.FloatField("Minimum Angle", poseTransform.minCameraAngle, GUILayout.ExpandWidth(false));
            poseTransform.maxCameraAngle = EditorGUILayout.FloatField("Maximum Angle", poseTransform.maxCameraAngle, GUILayout.ExpandWidth(false));
        }
        
        poseTransform.checkFov = EditorGUILayout.Toggle("Check FoV", poseTransform.checkFov);
        if (poseTransform.checkFov)
        {
            poseTransform.fovHorizontal = EditorGUILayout.FloatField("FoV Horizontal", poseTransform.fovHorizontal, GUILayout.ExpandWidth(false));
            poseTransform.fovVertical = EditorGUILayout.FloatField("FoV Vertical", poseTransform.fovVertical, GUILayout.ExpandWidth(false));
        }
        EditorUtility.SetDirty(poseTransform);
    }
}