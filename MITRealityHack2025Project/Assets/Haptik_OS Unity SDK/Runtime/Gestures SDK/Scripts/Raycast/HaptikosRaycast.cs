using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;
using Haptikos;

[RequireComponent(typeof(LineRenderer))]
public class HaptikosRaycast : MonoBehaviour
{
   
    LineRenderer line;

    public LineRenderer Line
    {
        get => line;
    }

    HaptikosSelectable clickSelected;
    public bool clickRay;
    HaptikosSelectable hoverSelected;
    public bool hoverRay;
    HaptikosSelectable currentSelected;

    private bool selectableHit;
    
    public bool SelectableHit
    {
        get => selectableHit;
    }

    public HandType hand;

    public HaptikosExoskeleton Hand
    {
        get => HaptikosPlayer.GetExoskeleton(hand);
    }

    public HaptikosGestureRecognizer clickRecognizer;
    public HaptikosGestureRecognizer hoverRecognizer;

    [SerializeField]
    private float width = 0.005f;
   
    public float rayRange = 100f;
 
    public bool drawCursor;
    public bool drawOnClick;
    public bool drawOnHover;
    [HideInInspector]
    public bool drawDefaultRay = true;

    bool prevClick = false;
    bool click;
    public bool ClickIsPressed
    {
        get => click;
    }

    bool hover;
    public bool Hover
    {
        get => hover && !click;
    }

    Vector3 hitPoint;
    public Vector3 HitPoint
    {
        get => hitPoint;
    }

    public LayerMask targetLayers;
    public LayerMask includeLayers;

    [HideInInspector]
    public Vector3 startingPoint;
    [HideInInspector]
    public Vector3 direction;
   
    bool validated;
    public bool Validated
    {
        get => validated;
    }

    bool warning;
    public bool Warning
    {
        get => warning;
    }

    public Color colour;
    [Range(0f,1f)]
    public float hoverTransparencyFactor;
    Color transparentColour;

    string status;
    public string getStatus
    {
        get => status;
    }
    Transform mainCamera;

    private void OnValidate()
    {
        validated = true;
        line = GetComponent<LineRenderer>();
        line.startWidth = width;
        line.endWidth = width;
        line.sortingOrder = 1;
        warning = false;

        if(clickRay && (clickRecognizer == null))
        {
            status = "Click Recognizer is missing";
            validated = false;
            warning = true;
        }

        if (hoverRay && (hoverRecognizer == null))
        {
            status = "Hover Recognizer is missing";
            validated = false;
            warning = true;
        }

        
        if (clickRecognizer != null)
        {
            clickRecognizer.hand = hand;
        }

        if (hoverRecognizer != null)
        {
            hoverRecognizer.hand = hand; 
        }
        
       
       
        if (clickRay && hoverRay && checkRecognizerOverlap(clickRecognizer, hoverRecognizer))
        {
            status = "Click and hover Recognizers should to differ";
            warning = true;
        }

        LayerMask layerMask = LayerMask.GetMask("Haptikos Hands");
        if((layerMask & includeLayers) != 0)
        {
            status = "Haptikos Hands layer should not be included, it can cause problems to raycasts";
            warning = true;
        }
        layerMask = LayerMask.GetMask("Ignore Raycast");
        if ((layerMask & includeLayers) != 0)
        {
            status = "Ignore Raycasts layer should not be included, it can cause problems to raycasts";
            warning = true;
        }
        includeLayers = targetLayers | includeLayers;
        clickRecognizer.enabled = false;
        hoverRecognizer.enabled = false;
    }

    void Awake()
    {
        OnValidate();
        line.material = HaptikosResources.Instance.raycastMaterial;
        transparentColour = colour;
        transparentColour.a *= hoverTransparencyFactor;
        mainCamera = GameObject.FindWithTag("MainCamera").transform;
    }

    private void OnEnable()
    {
        clickRecognizer.enabled = true;
        hoverRecognizer.enabled = true;
    }

    private void OnDisable()
    {
        if(hoverSelected != null)
        {
            hoverSelected.HoverExit?.Invoke(this, Hand);
            hoverSelected = null;
        }
        if(clickSelected != null)
        {
            clickSelected.ClickRelease?.Invoke(this, Hand);
            clickSelected = null;
        }
        line.enabled = false;
        clickRecognizer.enabled = false;
        hoverRecognizer.enabled = false;
    }

    void LateUpdate()
    {
        if (!validated)
        {
            if (hoverSelected != null)
            {
                hoverSelected.HoverExit?.Invoke(this, Hand);
                hoverSelected = null;
            }
            if (clickSelected != null)
            {
                clickSelected.ClickRelease?.Invoke(this, Hand);
                clickSelected = null;
            }
            line.enabled = false;
            return;
        }

        transparentColour = colour;
        transparentColour.a *= hoverTransparencyFactor;

        click = clickRay && clickRecognizer.Activated;
        hover = hoverRay && hoverRecognizer.Activated;

        HandleRay(hover, click);
        prevClick = click;
    }

    void HandleRay(bool hover, bool click)
    {
        if (!click && clickSelected!=null)
        {
            clickSelected.ClickRelease?.Invoke(this, Hand);
            clickSelected = null;
        }

        if(!click && !hover)
        {
            if (hoverSelected != null)
            {
                hoverSelected.HoverExit?.Invoke(this, Hand);
                hoverSelected = null; 
            }
            line.enabled = false;
            return;
        }

        line.enabled = true;
       
        Vector3[] positions = { startingPoint , startingPoint + direction * rayRange };

        selectableHit = CreateRay(positions, out positions, out RaycastHit hit);
        hitPoint = hit.point;

        if (SelectableHit)
        {
            currentSelected = hit.transform.gameObject.GetComponent<HaptikosSelectable>();
            selectableHit = selectableHit && (currentSelected != null);
        }

        if (SelectableHit) 
        { 
            SelectableDetected(hover, click, hit);
        }
        else if (hoverSelected != null)
        {
            hoverSelected.HoverExit?.Invoke(this, Hand);
            hoverSelected = null;
        }

        if (drawDefaultRay)
        {
            if (drawCursor)
            {
                line.enabled = false;
                float distance = (positions[positions.Length - 1] - mainCamera.position).magnitude;
            }
            line.positionCount = positions.Length;
            line.SetPositions(positions);
        }

        if(click && drawOnClick)
        {
            line.material.color = colour;
        }
        else if(hover && drawOnHover)
        {
            line.material.color = transparentColour;
        }
        else
        {
            line.enabled = false;
        }
    }

    void SelectableDetected(bool hover, bool click, RaycastHit hit)
    {
       
        if (click && !prevClick)
        {
            currentSelected.Click?.Invoke(this, Hand);
            clickSelected = currentSelected;
        }

        if ((hover || click) && currentSelected!=hoverSelected)
        {
            currentSelected.HoverEnter?.Invoke(this, Hand);
            if (hoverSelected != null)
            {
                hoverSelected.HoverExit?.Invoke(this, Hand);
            }
            hoverSelected = currentSelected;
        }
        
    }



    bool checkRecognizerOverlap(HaptikosGestureRecognizer recognizer1, HaptikosGestureRecognizer recognizer2)//TODO CHECK RECOGNITION OVERLAP
    {
        if(recognizer1 == null || recognizer2 == null)
        {
            return false;
        }
        HaptikosHandposeShape pose1 = recognizer1.targetHandPose;
        HaptikosHandposeShape pose2 = recognizer2.targetHandPose;
        HaptikosTransformPreset transform1 = recognizer1.transformPreset;
        HaptikosTransformPreset transform2 = recognizer2.transformPreset;
        return pose1 == pose2 && transform1 == transform2;
    }

    protected virtual bool CreateRay(Vector3[] originalPositions ,out Vector3[]  positions, out RaycastHit hit) //Returns true a selectable object was hit
    {
        positions = originalPositions;

        if (Physics.Raycast(startingPoint, direction, out hit, rayRange, includeLayers))
        {
            positions[1] = hit.point;

            if (((1 << hit.transform.gameObject.layer) & targetLayers) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false ;
        }
    }
   
}

[CustomEditor(typeof(HaptikosRaycast))]
public class HaptikRaycastEditor : Editor
{
    HaptikosRaycast raycast;
    public override void OnInspectorGUI()
    {
        raycast = (HaptikosRaycast)target;
        DrawDefaultInspector();
        if (raycast.Warning)
        {
            EditorGUILayout.HelpBox(raycast.getStatus, MessageType.Warning);
        }

    }
}