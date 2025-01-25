using UnityEngine;
using Haptikos.Gloves;


public class HaptikosHandPoseRecognizer
{
    HaptikosHandposeShape targetHandpose;
    HaptikosHandposeCalculator hand;
    HaptikosHandpose currentHandpose;
    float[] values;
    public bool[] recognizedValues = new bool[18];
    

    public HaptikosHandPoseRecognizer(HaptikosHandposeShape _targetHandpose, HaptikosHandposeCalculator _hand)
    {
       
        targetHandpose = _targetHandpose;
        currentHandpose = _hand.currentHandpose;
        hand = _hand;
    }

    public bool Check()
    {
        values = currentHandpose.values;
        if (values.Length != 18)
        {
            currentHandpose = hand.currentHandpose;
            return false;
        }
        return CheckHandpose();
    }

    private bool CheckHandpose()
    {
        
        bool result = true;
        for(int i=0; i<18; i++)
        {
            recognizedValues[i] = !((values[i] > targetHandpose.maxValues[i] || values[i] < targetHandpose.minValues[i]) && targetHandpose.includeValues[i] && targetHandpose.includeFingers[(i + 1)/4]);
            result = result && recognizedValues[i];
            
        }
        return result;
    }
}


//[CustomEditor(typeof(HaptikosHandPoseRecognizer))]
//public class HaptikHandPoseRecognizerInspector : Editor
//{
//    HaptikosHandPoseRecognizer recognizer;
//    bool currentHandpose = false;
//    bool detailedValues = false;
//    string[] fingers = { "Thumb", "Index", "Middle", "Ring", "Pinky" };
//    string[] values = { "Curl", "Flexion", "Abduction", "Opposition" };
//    float width = 100f;
//    HandType selectedType = HandType.RightHand;

//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();
//        recognizer = (HaptikosHandPoseRecognizer)target;

//        if (!recognizer.handSelected)
//        {
//            EditorGUILayout.BeginHorizontal();
//            bool select = GUILayout.Button("Select Hand", GUILayout.Width(width));
//            selectedType = (HandType)EditorGUILayout.EnumPopup(selectedType,GUILayout.Width(width));
//            EditorGUILayout.EndHorizontal();
//            if (select)
//            {
//                if(selectedType == HandType.LeftHand)
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
            
//        DrawCurrentHandpose();
//        DrawDetailedValues();
//    }

//    void DrawCurrentHandpose()
//    {
//        if(recognizer.currentHandpose.values == null)
//        {
//            return;
//        }
//        if (recognizer.currentHandpose.values.Length! == 18)
//        {
//            currentHandpose = EditorGUILayout.Foldout(currentHandpose, "Current Handpose");
//            if (currentHandpose)
//            {
//                GUIStyle style = GUIStyle.none;
//                style.richText = true;
//                int count = 0;

//                for (int i = 0; i < 5; i++)
//                {

//                    EditorGUILayout.LabelField("<b><color=white><size=15>" + fingers[i] + "</size></color></b>", style);

//                    for (int j = 0; j < 4; j++)
//                    {
//                        if ((j == 3 && i == 0) || (j == 2 && i == 4))
//                        {
//                            continue;
//                        }
//                        EditorGUILayout.BeginHorizontal();

//                        EditorGUILayout.LabelField(values[j], GUILayout.Width(width));
//                        EditorGUILayout.LabelField(recognizer.currentHandpose.values[count].ToString(), GUILayout.Width(width));

//                        EditorGUILayout.EndHorizontal();

//                        count++;
//                    }

//                    EditorGUILayout.Space();
//                }
//            }
//        } 
//    }

//    void DrawDetailedValues()
//    {
        
//        detailedValues = EditorGUILayout.Foldout(detailedValues, "Show Details");
        
//        if (detailedValues)
//        {
//            GUIStyle style = GUIStyle.none;
//            style.richText = true;
//            int count = 0;

//            for (int i = 0; i < 5; i++)
//            {

//                EditorGUILayout.LabelField("<b><color=white><size=15>" + fingers[i] + "</size></color></b>", style);

//                for (int j = 0; j < 4; j++)
//                {
//                    if ((j == 3 && i == 0) || (j == 2 && i == 4))
//                    {
//                        continue;
//                    }
//                    EditorGUILayout.BeginHorizontal();
                 
//                    EditorGUILayout.LabelField(values[j], GUILayout.Width(width));
//                    GUI.enabled = false;
//                    EditorGUILayout.Toggle(recognizer.recognizedValues[count], GUILayout.Width(width));
//                    GUI.enabled = true;

//                    EditorGUILayout.EndHorizontal();

//                    count++;
//                }

//                EditorGUILayout.Space();
//            }
//        }
        

//    }
//}
