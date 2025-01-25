using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomVibrationCurve))]
public class CustomVibrationCurveEditor : Editor
{
    private float newKeyframeTime = 0f;
    private float newKeyframeValue = 0f;
    private float newKeyframeInTangent = 0f;
    private float newKeyframeOutTangent = 0f;

    // Foldout controls
    private bool showCurveOperations = true;
    private bool showPatternOperations = true;
    private bool showPatternGeneration = true;
    private bool showPatternModifications = true;
    private bool showAudioOperations = true;
    private bool showAdvancedSettings = false;
    private bool showEditKeyframes = false;
    private bool showAddKeyframe = true;
    private bool showRepetitions = true;
    private bool showRescale = true;

    Color customColor = new Color(0f, 0f, 0f, 1f);
    
    // Variable for rescaling duration
    private float targetDuration = 1f;
    private int repetitions = 0;
    private int sampleCount = 1024;

    private float minAmplitude = 0f;
    private float maxAmplitude = 15f;
    private float keyframeInterval = 0.01f;
    private float startDuration = 0f;
    private float endDuration = 1f;


    private float startingValue = 0f;
    private float endingValue = 15f;
    private float durationValue = 1f;

    private AnimationCurve curve;
    private float curveStart;
    private float curveEnd;
    private float curveDuration;
    private AudioClip audioClip;


    private SerializedProperty curveProperty;
    private SerializedProperty audioclipProperty;

    private void OnEnable()
    {
        curveProperty = serializedObject.FindProperty("curve");
        audioclipProperty = serializedObject.FindProperty("audioClip");
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        serializedObject.Update();
        EditorGUILayout.PropertyField(curveProperty, new GUIContent("Curve Display"));
        //EditorGUILayout.PropertyField(audioclipProperty, new GUIContent("Audio Clip"));


        CustomVibrationCurve curveData = (CustomVibrationCurve)target;

        //curve = EditorGUILayout.CurveField("Curve", curveData.curve);
        //curveStart = EditorGUILayout.FloatField("Curve Starting Keyframe", curveData.startValue);
        //curveEnd = EditorGUILayout.FloatField("Curve Ending Keyframe", curveData.endValue);
        //curveDuration = EditorGUILayout.FloatField("Curve Duration", curveData.duration);
        //audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", curveData.audioClip, typeof(AudioClip),false);

        EditorGUILayout.Space();

        // Curve Operations Section
        showCurveOperations = EditorGUILayout.Foldout(showCurveOperations, "Curve Operations", true);
        if (showCurveOperations)
        {

            EditorGUILayout.LabelField("Restart Curve", EditorStyles.boldLabel);

            GUI.backgroundColor = Color.grey;
            if (GUILayout.Button("Restart Curve"))
            {
                if (EditorUtility.DisplayDialog("Confirm restarting curve", "Are you sure you want to restart the curve?", "Yes", "No"))
                {
                    curveData.ResetCurve();
                    EditorUtility.SetDirty(curveData);
                    targetDuration = 1f;
                    repetitions = 1;
                    sampleCount = 1024;
                    minAmplitude = 0f;
                    maxAmplitude = 15f;
                    keyframeInterval = 0.01f;
                    startDuration = 0f;
                    endDuration = 1f;
                    startingValue = 0f;
                    endingValue = 15f;
                    durationValue = 1f;
}
            }
            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.Space();

        // Pattern Generation Section
        showPatternOperations = EditorGUILayout.Foldout(showPatternOperations, "Pattern Operations", true);
        if (showPatternOperations)
        {
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;
            showPatternGeneration = EditorGUILayout.Foldout(showPatternGeneration,"Pattern Generation", true);
            if (showPatternGeneration)
            {
                EditorGUI.indentLevel++;
                if (GUILayout.Button("Generate Linear Curve"))
                {
                    curveData.GenerateLinear();
                    EditorUtility.SetDirty(curveData);
                }

                if (GUILayout.Button("Generate Inverse Linear Curve"))
                {
                    curveData.GenerateInverseLinear();
                    EditorUtility.SetDirty(curveData);
                }

                if (GUILayout.Button("Generate Sinusoidal Curve"))
                {
                    curveData.GenerateSine();
                    EditorUtility.SetDirty(curveData);
                }

                if (GUILayout.Button("Generate Exponential Curve"))
                {
                    curveData.GenerateExponential();
                    EditorUtility.SetDirty(curveData);
                }

                if (GUILayout.Button("Generate Inverse Exponential Curve"))
                {
                    curveData.GenerateInverseExponential();
                    EditorUtility.SetDirty(curveData);
                }
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            showPatternModifications = EditorGUILayout.Foldout(showPatternModifications, "Pattern Modifications", true);
            
            if (showPatternModifications)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical("box");
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                startingValue = EditorGUILayout.FloatField("Starting Curve Value", startingValue);
                endingValue = EditorGUILayout.FloatField("Ending Curve Value", endingValue);
                durationValue = EditorGUILayout.FloatField("Curve Duration Value", durationValue);
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                EditorGUILayout.EndVertical();

                if (GUILayout.Button("Apply Pattern Modifications"))
                {
                    curveData.ApplyPatternModifications(startingValue, endingValue, durationValue);
                    EditorUtility.SetDirty(curveData);
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUI.indentLevel--;
        }


        EditorGUILayout.Space();

        showAudioOperations = EditorGUILayout.Foldout(showAudioOperations, "Audio to Curve Operations", true);
        if (showAudioOperations)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(audioclipProperty, new GUIContent("AudioClip"));
            EditorGUILayout.CurveField("Original WAV File", curveData.originalCurve);
            sampleCount = EditorGUILayout.IntField("Sample Count", sampleCount);

            EditorGUI.indentLevel++;
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
            if (GUILayout.Button("Convert Audio to Curve"))
            {
                curveData.ConvertAudioToCurve(sampleCount);
                
                EditorUtility.SetDirty(curveData);
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel--;
            EditorGUILayout.CurveField("Edited WAV File", curveData.editedCurve);
            minAmplitude = EditorGUILayout.FloatField("Minimum Amplitude", minAmplitude);
            maxAmplitude = EditorGUILayout.FloatField("Maximum Amplitude", maxAmplitude);
            keyframeInterval = EditorGUILayout.FloatField("Keyframes Time Interval", keyframeInterval);
            startDuration = EditorGUILayout.FloatField("Start Duration", startDuration);
            endDuration = EditorGUILayout.FloatField("End Duration", endDuration);
            EditorGUI.indentLevel++;
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Apply Curve Edits"))
            {
                curveData.ApplyEditsToCurve(minAmplitude, maxAmplitude, startDuration, endDuration, keyframeInterval);
            }

            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel--;

            EditorGUILayout.CurveField("Peaks WAV File", curveData.curve);

            EditorGUI.indentLevel++;
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Apply Only Peaks"))
            {
                curveData.PeaksOnlyCurve(startDuration);
            }

            GUI.backgroundColor = Color.grey;
            if (GUILayout.Button("Reset to default values"))
            {
                if (EditorUtility.DisplayDialog("Confirm Reset", "Are you sure you want to reset the curve to default values?", "Yes", "No"))
                {
                    curveData.ApplyEditsToCurve(0, 15, 0, 1, 0.01f);
                    minAmplitude = 0;
                    maxAmplitude = 15;
                    keyframeInterval = 0.01f;
                    startDuration = 0;
                    endDuration = 1;
                    curveData.PeaksOnlyCurve(0);
                }
            }
            GUI.backgroundColor = Color.white;
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Curve Operations", true);
        if (showAdvancedSettings)
        {
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;
            // Edit Keyframes Section
            showEditKeyframes = EditorGUILayout.Foldout(showEditKeyframes, "Edit Keyframes", true);
            if (showEditKeyframes)
            {
                EditorGUI.indentLevel--;
                for (int i = 0; i < curveData.editableKeyframes.Count; i++)
                {
                    Keyframe keyframe = curveData.editableKeyframes[i];

                    EditorGUILayout.BeginVertical("box");
                    // Time and Value fields
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Time", GUILayout.Width(80));
                    keyframe.time = EditorGUILayout.FloatField(keyframe.time);

                    EditorGUILayout.LabelField("Value", GUILayout.Width(80));
                    keyframe.value = EditorGUILayout.FloatField(keyframe.value);
                    EditorGUILayout.EndHorizontal();

                    // In Tangent and Out Tangent fields
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("In Tangent", GUILayout.Width(80));
                    keyframe.inTangent = EditorGUILayout.FloatField(keyframe.inTangent);

                    EditorGUILayout.LabelField("Out Tangent", GUILayout.Width(80));
                    keyframe.outTangent = EditorGUILayout.FloatField(keyframe.outTangent);
                    EditorGUILayout.EndHorizontal();

                    GUILayout.FlexibleSpace();

                    // Remove button
                    if (GUILayout.Button("Remove Keyframe"))
                    {
                        curveData.editableKeyframes.RemoveAt(i);
                        i--;
                        EditorUtility.SetDirty(curveData);
                        continue;
                    }

                    EditorGUILayout.EndVertical();

                    // Update the keyframe in the list
                    curveData.editableKeyframes[i] = keyframe;
                    curveData.ApplyEditableKeyframes();
                    EditorUtility.SetDirty(curveData);
                }

                EditorGUILayout.Space();
                GUI.backgroundColor = Color.grey;
                if (GUILayout.Button("Remove All Keyframes"))
                {
                    if (EditorUtility.DisplayDialog("Confirm Delete", "Are you sure you want to remove all keyframes?", "Yes", "No"))
                    {
                        curveData.RemoveAllKeyframes();
                        EditorUtility.SetDirty(curveData);
                    }
                }
                GUI.backgroundColor = Color.white;
                EditorGUI.indentLevel++;
            }

            EditorGUILayout.Space();

            // Add New Keyframe Section
            showAddKeyframe = EditorGUILayout.Foldout(showAddKeyframe, "Add New Keyframe", true);
            if (showAddKeyframe)
            {
                EditorGUI.indentLevel=0;

                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical("box");
                EditorGUI.indentLevel--;
                newKeyframeTime = EditorGUILayout.FloatField("Time", newKeyframeTime);
                newKeyframeValue = EditorGUILayout.FloatField("Value", newKeyframeValue);
                newKeyframeInTangent = EditorGUILayout.FloatField("In Tangent", newKeyframeInTangent);
                newKeyframeOutTangent = EditorGUILayout.FloatField("Out Tangent", newKeyframeOutTangent);
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndVertical();
                //EditorGUI.indentLevel++;
                if (GUILayout.Button("Add Keyframe"))
                {

                    curveData.AddKeyframe(newKeyframeTime, newKeyframeValue, newKeyframeInTangent, newKeyframeOutTangent);
                    EditorUtility.SetDirty(curveData);
                }
                EditorGUI.indentLevel++;
            }

            // Rescale Curve Duration Section
            EditorGUILayout.Space();

            showRescale = EditorGUILayout.Foldout(showRescale, "Rescale Curve Duration", true);
            if (showRescale)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUI.indentLevel--;
                targetDuration = EditorGUILayout.FloatField("Target Duration", targetDuration);
                EditorGUI.indentLevel++;
                EditorGUILayout.EndVertical();
                if (GUILayout.Button("Apply Curve Duration Scaling"))
                {
                    curveData.RescaleCurveDuration(targetDuration);
                    EditorUtility.SetDirty(curveData);
                }
                GUI.backgroundColor = Color.grey;
                if (GUILayout.Button("Reset Curve Duration Scaling"))
                {
                    if (EditorUtility.DisplayDialog("Confirm reset curve duration scaling", "Are you sure you want to reset the scaling?", "Yes", "No"))
                    {
                        curveData.ResetRescale();
                        EditorUtility.SetDirty(curveData);
                        targetDuration = 1;
                    }
                }
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.Space();
            showRepetitions = EditorGUILayout.Foldout(showRepetitions, "Curve Cycles", true);
            if (showRepetitions)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUI.indentLevel--;
                repetitions = EditorGUILayout.IntField("Number of cycles", repetitions);
                EditorGUI.indentLevel++;
                EditorGUILayout.EndVertical();
                if (GUILayout.Button("Apply number of cycles"))
                {
                    curveData.RepeatCurve(repetitions);
                    EditorUtility.SetDirty(curveData);
                }
                GUI.backgroundColor = Color.grey;
                if (GUILayout.Button("Reset curve cycles to default"))
                {
                    if (EditorUtility.DisplayDialog("Confirm reset curve cycles to default(1)", "Are you sure you want to reset the curve cycles to default(1)?", "Yes", "No"))
                    {
                        curveData.ResetRepeat();
                        EditorUtility.SetDirty(curveData);
                        repetitions = 1;
                    }
                }
                GUI.backgroundColor = Color.white;
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}