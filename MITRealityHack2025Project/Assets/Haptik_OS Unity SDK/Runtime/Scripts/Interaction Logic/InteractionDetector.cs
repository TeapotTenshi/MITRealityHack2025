using Haptikos.Exoskeleton;
using Haptikos.Gloves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionDetector : MonoBehaviour
{
    Dictionary<string, HandPart> handPartsInsideObject = new Dictionary<string, HandPart>();

    public UnityEvent onObjectStoppedTouching;
    public UnityEvent onObjectStartTouching;

    [Space(5)]
    [Header("Parts to Interact with")]
    [SerializeField] Hand_Part_Type interactionPart;

    private HandPart lastTouchedFinger;

    public Hand_Part_Type InteractionPart { get => interactionPart; set => interactionPart = value; }
    public HandPart LastTouchedFinger { get => lastTouchedFinger; set => lastTouchedFinger = value; }

    Collider col;


    private void Start()
    {
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<HandPart>(out var collidingHandPart))
        {          
            if(InteractionPart == collidingHandPart.Type || InteractionPart == Hand_Part_Type.All)
            {
                if (!handPartsInsideObject.ContainsKey(collidingHandPart.Name))
                {
                    handPartsInsideObject.Add(collidingHandPart.Name, collidingHandPart);
                }

                lastTouchedFinger = other.GetComponent<HandPart>();
            }
        }

        if (handPartsInsideObject.Count == 1)
        {
            onObjectStartTouching?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<HandPart>(out var collidingHandPart))
        {
            if (InteractionPart == collidingHandPart.Type || InteractionPart == Hand_Part_Type.All)
            {
                if (handPartsInsideObject.ContainsKey(collidingHandPart.Name))
                {
                    handPartsInsideObject.Remove(collidingHandPart.Name);

                    lastTouchedFinger = other.GetComponent<HandPart>();

                }
            }
        }

        if (handPartsInsideObject.Count == 0)
        {
            onObjectStoppedTouching?.Invoke();
        }
    }
}
