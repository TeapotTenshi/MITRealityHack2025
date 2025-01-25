using Haptikos.Exoskeleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStreamingEvents : MonoBehaviour
{
    public event Action<HaptikosExoskeleton> OnDataReceived;

    public event Action<HaptikosExoskeleton> OnDataStoppedReceiving;

    public void CallOnDataReceived(HaptikosExoskeleton hand)
    {
        OnDataReceived?.Invoke(hand);
    }

    public void CallOnDataStoppedReceiving(HaptikosExoskeleton hand)
    {
        OnDataStoppedReceiving?.Invoke(hand);
    }
}
