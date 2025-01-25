using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;
using Haptikos;

public class HaptikosGestureRecognizer : MonoBehaviour
{
    
    HaptikosHandPoseRecognizer poseRecognizer;
    
    public HaptikosHandPoseRecognizer PoseRecognizer
    {
        get => poseRecognizer;
    }

    HaptikosHandPoseTransformRecognizer transformRecognizer;

    public HaptikosHandPoseTransformRecognizer TransformRecognizer
    {
        get => transformRecognizer;
    }

    public HaptikosExoskeleton Hand
    {
        get => HaptikosPlayer.GetExoskeleton(hand);
    }
    public HandType hand;
    public HaptikosHandposeCalculator handposeCalculator;

    public bool poseRecognition = true;
    public bool reversePoseRecognition;
    public HaptikosHandposeShape targetHandPose;
    public bool transformRecognition = true;
    public bool reverseTransformRecognition;
    public HaptikosTransformPreset transformPreset;
    public float timeToActivate = 0.05f;
    public float timeToDeactivate = 0.05f;
    public bool visualizeTimeToActivate;
    public bool visualizeTimeToDeactivate;
    [SerializeField]
    private Sprite icon;
    public Sprite Icon
    {
        get => Icon;
        set
        {
            icon = value;
            if (iconImage != null)
            {
                iconImage.sprite = icon;
            }
        }
    }

    GameObject visualization;
    Slider slider;
    Image iconImage;
    Image fillImage;
    [SerializeField]
    bool activated;
    public bool Activated
    {
        get => activated;
    }
    bool recognized;
    public bool Recognized
    {
        get => recognized;
    }
  
    public UnityEvent<HaptikosExoskeleton> OnActivate;
    public UnityEvent<HaptikosExoskeleton> OnDeactivate;

    float inspectorTimer;

    string status;

    public string Status
    {
        get => status;
    }

    bool prevActivated;

    float timer;
    public float Timer
    {
        get => timer;
    }

    
    
    bool validated = false;
    public bool Validated
    {
        get => validated;
    }

    GameObject mainCamera;
    Transform middleBase;
    bool started = false;
    
    public void OnValidate()
    {
        validated = false;

        if (poseRecognition)
        {
            if(targetHandPose == null)
            {
                status = "Pose recognition is active, but no target handpose is provided";
                return;
            }
            if (started)
            {
                handposeCalculator = Hand.GetComponent<HaptikosHandposeCalculator>();
                poseRecognizer = new HaptikosHandPoseRecognizer(targetHandPose, handposeCalculator);
            }
            
        }

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera == null)
        {
            status = "Transform preset requires a main camera in the scene";
            return;
        }

        if (transformRecognition)
        {
            if(transformPreset == null)
            {
                status = "Transform Recognintion is active, but no transform preset is provided";
                return;
            }
            if (started)
            {
                transformRecognizer = new HaptikosHandPoseTransformRecognizer(transformPreset, mainCamera.transform, Hand, hand);
            }

        }

        if (visualization!=null)
        {
            if(icon == null)
            {
                iconImage.sprite = HaptikosResources.Instance.circle;
            }
            else
            {
                iconImage.sprite = icon;
            }  
        }
        status = "OK";
        validated = true;
        timer = 0;
        prevActivated = false;
        activated = false;
    }

    void OnEnable()
    {
        OnValidate();
    }

    private void Start()
    {
        middleBase = Hand.transform.GetChild(0).GetChild(0).GetChild(2);
        started = true;
        OnValidate();
    }

    // Update is called once per frame
    void Update()
    {
        bool poseRecognized = false;
        bool transformRecongnized = false;

        if (!validated)
        {
            if (activated)
            {
                OnDeactivate?.Invoke(Hand);
            }
            activated = false;
            return;
        }
        if (transformRecognition)
        {
            transformRecongnized = transformRecognizer.Check();
            if (reverseTransformRecognition)
            {
                transformRecongnized = !transformRecongnized;
            }
        }

        if (poseRecognition)
        {
            poseRecognized = poseRecognizer.Check();
            if (reversePoseRecognition)
            {
                poseRecognized = !poseRecognized;
            }
        }

        recognized = (transformRecognition || poseRecognition) && (transformRecongnized || !transformRecognition) && (poseRecognized || !poseRecognition);

        if (recognized != activated)
        {
            if (activated)
            {
                if(timer >= timeToDeactivate)
                {
                    timer = 0;
                    activated = recognized;
                }
                else
                {
                    timer += Time.deltaTime;
                }
            }
            else
            {
                if (timer >= timeToActivate)
                {
                    timer = 0;
                    activated = recognized;
                }
                else
                {
                    timer += Time.deltaTime;
                }
            }
        }
        else
        {
            timer = 0;
        }
        
        if(activated && !prevActivated)
        {
            OnActivate?.Invoke(Hand);
        }

        if(!activated && prevActivated)
        {
            OnDeactivate?.Invoke(Hand);
        }

        prevActivated = activated;

        if(inspectorTimer < 0)
        {
            EditorUtility.SetDirty(this);
            inspectorTimer = 0.2f;
        }
        else
        {
            inspectorTimer -= Time.deltaTime;
        }
        if(visualizeTimeToDeactivate || visualizeTimeToActivate)
        {
            if (visualization==null)
            {
                visualization = Instantiate(HaptikosResources.Instance.slider);
                visualization.transform.parent = transform;
                slider = visualization.GetComponentInChildren<Slider>();
                iconImage = visualization.transform.GetChild(0).GetChild(2).GetComponent<Image>();
                if(icon != null)
                {
                    iconImage.sprite = icon;
                }
                else
                {
                    iconImage.sprite = HaptikosResources.Instance.circle;
                }
                fillImage = visualization.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
            }
            HandleProgressSlider();
        }
         
    }
     
    private void OnDisable()
    {
        if (activated)
        {
            OnDeactivate?.Invoke(Hand);
        }
        activated = false;
        prevActivated = false;
    }

    void HandleProgressSlider()
    {
        if (timer == 0)
        {
            visualization.SetActive(false);
            return;
        }

        if (!activated && visualizeTimeToActivate)
        {
            visualization.SetActive(true);
            visualization.transform.SetPositionAndRotation(middleBase.position + Vector3.up * 0.15f, mainCamera.transform.rotation);
           
            fillImage.color = Color.green;
            slider.value = timer / timeToActivate;
        }
        else if(activated && visualizeTimeToDeactivate)
        {
            visualization.SetActive(true);
            visualization.transform.SetPositionAndRotation(middleBase.position + Vector3.up * 0.15f, mainCamera.transform.rotation);
            fillImage.color = Color.red;
            slider.value = timer / timeToActivate;
        }
    }

    void MoveProgressSlider(Vector3 targetPosition)
    {
        float maxSpeed = 0.1f;
        Vector3 currentPosition = visualization.transform.position;
        Vector3 direction = targetPosition - currentPosition;
        if (direction.magnitude <= maxSpeed * Time.deltaTime || direction.magnitude > 0.5f)
        {
            visualization.transform.SetPositionAndRotation(targetPosition, mainCamera.transform.rotation);
        }
        else
        {
            //visualization.transform.SetPositionAndRotation(targetPosition, mainCamera.transform.rotation);
            visualization.transform.rotation = mainCamera.transform.rotation;
            visualization.transform.position += direction * (maxSpeed * Time.deltaTime / direction.magnitude);
        }
    }
}



[CustomEditor(typeof(HaptikosGestureRecognizer))]

public class HaptikGestureRecognizerEditor : Editor
{
    HaptikosGestureRecognizer recognizer;
    HaptikosHandposeCalculator handposeCalucalator;
    HaptikosHandPoseTransformRecognizer transformRecognizer;
    
    SerializedProperty Hand;
    SerializedProperty PoseRecognition;
    SerializedProperty ReversePoseRecognition;
    SerializedProperty TargetHandPose;
    SerializedProperty ReverseTransformRecognition;
    SerializedProperty TransformRecognition;
    SerializedProperty TransformPreset;
    SerializedProperty TimeToActivate;
    SerializedProperty TimeToDeactivate;
    SerializedProperty VisualizeTimeToActivate;
    SerializedProperty VisualizeTimetoDeactivate;
    SerializedProperty Icon;
    SerializedProperty Activated;
    //SerializedProperty Recognized;
    SerializedProperty OnActivate;
    SerializedProperty OnDeactivate;
    
    bool poseDebug = false;
    bool transformDebug = false;
    bool currentHandpose = false;
    bool detailedValues = false;

    string[] fingers = { "Thumb", "Index", "Middle", "Ring", "Pinky" };
    string[] values = { "Curl", "Flexion", "Abduction", "Opposition" };
    float width = 100f;

    private void OnEnable()
    {
        Hand = serializedObject.FindProperty("hand");
        PoseRecognition = serializedObject.FindProperty("poseRecognition");
        ReversePoseRecognition = serializedObject.FindProperty("reversePoseRecognition");
        TargetHandPose = serializedObject.FindProperty("targetHandPose");
        ReverseTransformRecognition = serializedObject.FindProperty("reverseTransformRecognition");
        TransformRecognition = serializedObject.FindProperty("transformRecognition");
        TransformPreset = serializedObject.FindProperty("transformPreset");
        TimeToActivate = serializedObject.FindProperty("timeToActivate");
        TimeToDeactivate = serializedObject.FindProperty("timeToDeactivate");
        VisualizeTimeToActivate = serializedObject.FindProperty("visualizeTimeToActivate");
        VisualizeTimetoDeactivate = serializedObject.FindProperty("visualizeTimeToDeactivate");
        Icon = serializedObject.FindProperty("icon");
        Activated = serializedObject.FindProperty("activated");
        //Recognized = serializedObject.FindProperty("recognized");
        OnActivate = serializedObject.FindProperty("OnActivate");
        OnDeactivate = serializedObject.FindProperty("OnDeactivate");
    }

    public override void OnInspectorGUI()
    {
        recognizer = (HaptikosGestureRecognizer)target;
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script: ",MonoScript.FromMonoBehaviour(recognizer), typeof(HaptikosGestureRecognizer), false);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(Hand);
        //if (recognizer.hand == null)
        //{
        //    EditorGUILayout.BeginHorizontal();
        //    bool select = GUILayout.Button("Select Hand", GUILayout.Width(width));
        //    selectedType = (HandType)EditorGUILayout.EnumPopup(selectedType, GUILayout.Width(width));
        //    EditorGUILayout.EndHorizontal();
        //    if (select)
        //    {
        //        if (selectedType == HandType.LeftHand)
        //        {
        //            recognizer.hand = GameObject.FindGameObjectWithTag("Left Hand").GetComponent<HaptikosExoskeleton>();
        //        }
        //        else if (selectedType == HandType.RightHand)
        //        {
        //            recognizer.hand = GameObject.FindGameObjectWithTag("Right Hand").GetComponent<HaptikosExoskeleton>();
        //        }
        //        if (recognizer.hand == null)
        //        {
        //            Debug.Log("Insert haptik player with a " + selectedType.ToString());
        //        }
        //        recognizer.OnValidate();
        //    }
        //}
        EditorGUILayout.PropertyField(PoseRecognition);
        EditorGUILayout.PropertyField(ReversePoseRecognition);
        EditorGUILayout.PropertyField(TargetHandPose);
        EditorGUILayout.PropertyField(TransformRecognition);
        EditorGUILayout.PropertyField(ReverseTransformRecognition);
        EditorGUILayout.PropertyField(TransformPreset);
        EditorGUILayout.PropertyField(TimeToActivate);
        EditorGUILayout.PropertyField(TimeToDeactivate);
        EditorGUILayout.PropertyField(VisualizeTimeToActivate);
        EditorGUILayout.PropertyField(VisualizeTimetoDeactivate);
        if (recognizer.visualizeTimeToActivate || recognizer.visualizeTimeToDeactivate)
        {
            EditorGUILayout.PropertyField(Icon);
        }
        EditorGUILayout.PropertyField(Activated);
        //EditorGUILayout.PropertyField(Recognized);
        EditorGUILayout.PropertyField(OnActivate);
        EditorGUILayout.PropertyField(OnDeactivate);

        serializedObject.ApplyModifiedProperties();

        if (!recognizer.Validated)
        {
            EditorGUILayout.HelpBox(recognizer.Status, MessageType.Warning);
        }

        if (recognizer.Validated)
        {
            if (recognizer.poseRecognition && Application.isPlaying)
            {
                handposeCalucalator = recognizer.handposeCalculator;
                if (handposeCalucalator.currentHandpose.values != null)
                {
                    poseDebug = EditorGUILayout.Foldout(poseDebug, "Debug Hand Pose", true);
                    if (poseDebug)
                    {
                        EditorGUI.indentLevel++;
                        DrawCurrentHandpose();
                        DrawDetailedValues();
                        EditorGUI.indentLevel--;
                    }
                }
            }
            if (recognizer.transformRecognition && Application.isPlaying)
            {
                transformRecognizer = recognizer.TransformRecognizer;
                transformDebug = EditorGUILayout.Foldout(transformDebug, "Transform Debug", true);
                if (transformDebug)
                {
                    EditorGUI.indentLevel++;
                    if (recognizer.transformPreset.checkAgaintsWorldUp)
                    {
                        EditorGUILayout.LabelField("Angle from World Up " + transformRecognizer.angleFromWorldAxis.ToString());
                        EditorGUILayout.LabelField("Minimum Accepted Value " + recognizer.transformPreset.minWorldAngle.ToString());
                        EditorGUILayout.LabelField("Maximum Accepted Value " + recognizer.transformPreset.maxWorldAngle.ToString());
                    }
                    if (recognizer.transformPreset.checkAgaintsCamera)
                    {
                        EditorGUILayout.LabelField("Angle from Camera " + transformRecognizer.angleFromCameraAxis.ToString());
                        EditorGUILayout.LabelField("Minimum Accepted Value " + recognizer.transformPreset.minCameraAngle.ToString());
                        EditorGUILayout.LabelField("Maximum Accepted Value " + recognizer.transformPreset.maxCameraAngle.ToString());
                    }
                    if (recognizer.transformPreset.checkFov)
                    {
                        EditorGUILayout.LabelField("Horizontal Angle*2 " + (transformRecognizer.xAngle*2).ToString());
                        EditorGUILayout.LabelField("Horizontal FoV " + recognizer.transformPreset.fovHorizontal.ToString());
                        EditorGUILayout.LabelField("Vertical Angle*2 " + (transformRecognizer.yAngle*2).ToString());
                        EditorGUILayout.LabelField("Vertical FoV " + recognizer.transformPreset.fovVertical.ToString());
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }
    }

    void DrawCurrentHandpose()
    {
        if (handposeCalucalator.currentHandpose.values.Length! == 18)
        {
            currentHandpose = EditorGUILayout.Foldout(currentHandpose, "Current Hand Pose");
            if (currentHandpose)
            {
                GUIStyle style = GUIStyle.none;
                style.richText = true;
                int count = 0;

                for (int i = 0; i < 5; i++)
                {

                    EditorGUILayout.LabelField("<b><color=white><size=15>" + fingers[i] + "</size></color></b>", style);

                    for (int j = 0; j < 4; j++)
                    {
                        if ((j == 3 && i == 0) || (j == 2 && i == 4))
                        {
                            continue;
                        }
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField(values[j], GUILayout.Width(width));
                        EditorGUILayout.LabelField(handposeCalucalator.currentHandpose.values[count].ToString(), GUILayout.Width(width));

                        EditorGUILayout.EndHorizontal();

                        count++;
                    }

                    EditorGUILayout.Space();
                }
            }
        }
    }

    void DrawDetailedValues()
    {
        detailedValues = EditorGUILayout.Foldout(detailedValues, "Show Details");

        if (detailedValues)
        {
            GUIStyle style = GUIStyle.none;
            style.richText = true;
            int count = 0;

            for (int i = 0; i < 5; i++)
            {

                EditorGUILayout.LabelField("<b><color=white><size=15>" + fingers[i] + "</size></color></b>", style);

                for (int j = 0; j < 4; j++)
                {
                    if ((j == 3 && i == 0) || (j == 2 && i == 4))
                    {
                        continue;
                    }
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(values[j], GUILayout.Width(width));
                    GUI.enabled = false;
                    EditorGUILayout.Toggle(recognizer.PoseRecognizer.recognizedValues[count], GUILayout.Width(width));
                    GUI.enabled = true;

                    EditorGUILayout.EndHorizontal();

                    count++;
                }

                EditorGUILayout.Space();
            }
        }
    }
}
