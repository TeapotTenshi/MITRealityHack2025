using Haptikos.Exoskeleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExoskeletonConnectionController : MonoBehaviour
{
    [Space(5)]
    [Header("Hand Model References")]
    [SerializeField] GameObject leftHandMesh;
    [SerializeField] GameObject rightHandMesh;
    static bool rightGloveConnected = false;
    static public bool RightGloveConnetected
    {
        get => rightGloveConnected;
    }
    static bool leftGloveConnected = false;
    static public bool LeftGloveConnected
    {
        get => leftGloveConnected;
    }


    DataStreamingEvents dataStreamingEvents;
    // Start is called before the first frame update
    void Awake()
    {
        dataStreamingEvents = GetComponent<DataStreamingEvents>();

        HideHands();
    }

    private void HideHands()
    {
        leftHandMesh.SetActive(false);
        rightHandMesh.SetActive(false);
    }

    private void OnEnable()
    {
        dataStreamingEvents.OnDataReceived += DataStreamingEvents_OnDataReceived;
        dataStreamingEvents.OnDataStoppedReceiving += DataStreamingEvents_OnDataStoppedReceiving;
    }

    private void DataStreamingEvents_OnDataStoppedReceiving(HaptikosExoskeleton hand) 
    {
        if (hand.hand.HandType == HandType.LeftHand)
        {
            leftHandMesh.SetActive(false);
            leftGloveConnected = false;
        }
        else if (hand.hand.HandType == HandType.RightHand)
        {
            rightHandMesh.SetActive(false);
            rightGloveConnected = false;
        }
    }

    private void DataStreamingEvents_OnDataReceived(HaptikosExoskeleton hand)
    {
        if (hand.hand.HandType == HandType.LeftHand)
        {
            leftHandMesh.SetActive(true);
            leftGloveConnected = true;
        }
        else if (hand.hand.HandType == HandType.RightHand)
        {
            rightHandMesh.SetActive(true);
            rightGloveConnected = true;
        }
    }

    private void OnDisable()
    {
        dataStreamingEvents.OnDataReceived -= DataStreamingEvents_OnDataReceived;
        dataStreamingEvents.OnDataStoppedReceiving -= DataStreamingEvents_OnDataStoppedReceiving;
    }
}
