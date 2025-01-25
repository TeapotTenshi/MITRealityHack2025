using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;


[CreateAssetMenu(menuName = "ScriptableObjects/HandPoses Shape")]
public class HaptikosHandposeShape : ScriptableObject
{
    public float[] minValues;
    public float[] maxValues;
    public bool[] includeFingers;
    public bool[] includeValues;
    public bool final = false;


    public void Reset()
    {
        minValues = new float[18];
        maxValues = new float[18];
        includeFingers = new bool[5];
        includeValues = new bool[18];
        final = false;
    }
}

[CustomEditor(typeof(HaptikosHandposeShape))]
public class HaptikHandposeEditor : Editor
{
    HaptikosHandposeShape shape;
    string[] fingers = { "Thumb", "Index", "Middle", "Ring", "Pinky" };
    string[] values = { "Curl", "Flexion", "Abduction", "Opposition" };
    float width = 100f;
    float titleWidth = 306f;

    public override void OnInspectorGUI()
    {
        shape = (HaptikosHandposeShape)target;
        GUI.enabled = false;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Script", GUILayout.Width(width));
        EditorGUILayout.ObjectField( MonoScript.FromScriptableObject(shape), typeof(HaptikosHandposeShape),false,GUILayout.Width(titleWidth - width + 18));
        EditorGUILayout.EndHorizontal();
        GUI.enabled = !shape.final;
        DrawMinMaxList();
        
        GUI.enabled = true;
        if (!shape.final)
        {
            if (GUILayout.Button("Save", GUILayout.Width(titleWidth + 18)))
            {
                shape.final = true;
                EditorUtility.SetDirty(shape);
            }
        }
        else
        {
            if (GUILayout.Button("Edit", GUILayout.Width(titleWidth + 18)))
            {
                shape.final = false;
            }

        }
    }

    void DrawMinMaxList()
    { 
        if (shape.minValues.Length != 18 || shape.maxValues.Length!=18)
        {
            shape.Reset();
        }

        GUIStyle style = GUIStyle.none;
        style.richText = true;
        int count = 0;

        
        
            for (int i = 0; i < 5; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("<b><color=white><size=15>" + fingers[i] + "</size></color></b>", style, GUILayout.Width(titleWidth));
                shape.includeFingers[i] = EditorGUILayout.Toggle(shape.includeFingers[i]);
                EditorGUILayout.EndHorizontal();
                if (shape.includeFingers[i])
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if((j==3 && i==0) || (j==2 && i == 4)){
                            continue;
                        }
                        EditorGUILayout.BeginHorizontal();
                        if (shape.includeValues[count])
                        {
                            EditorGUILayout.LabelField(values[j],GUILayout.Width(width));
                            shape.minValues[count] = EditorGUILayout.FloatField(shape.minValues[count], GUILayout.Width(width));
                            shape.maxValues[count] = EditorGUILayout.FloatField(shape.maxValues[count], GUILayout.Width(width));
                            shape.includeValues[count] = EditorGUILayout.Toggle(shape.includeValues[count]);
                        }
                        else
                        {
                            if (GUI.enabled)
                            {
                                EditorGUILayout.LabelField(values[j], GUILayout.Width(width));
                                GUI.enabled = false;
                                shape.minValues[count] = EditorGUILayout.FloatField(shape.minValues[count], GUILayout.Width(width));
                                shape.maxValues[count] = EditorGUILayout.FloatField(shape.maxValues[count], GUILayout.Width(width));
                                GUI.enabled = true;
                            }
                            else
                            {
                            EditorGUILayout.LabelField(values[j], GUILayout.Width(titleWidth));
                            }
                            shape.includeValues[count] = EditorGUILayout.Toggle(shape.includeValues[count]);
                        }
                        EditorGUILayout.EndHorizontal();

                        count++;
                    }
                }
                else
                {
                    count = (i+1) * 4 - 1;
                }
                EditorGUILayout.Space();
            }
    }
    
}
