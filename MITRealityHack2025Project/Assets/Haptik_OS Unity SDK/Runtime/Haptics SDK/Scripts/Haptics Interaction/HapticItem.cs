using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HapticItem : MonoBehaviour
{
    public UnityEvent<bool, string, HandType> onHapticFeedbackStarted = new UnityEvent<bool, string, HandType>();

    public UnityEvent<bool, string, HandType, bool> onHapticFeedbackStartAndEnd = new UnityEvent<bool, string, HandType, bool>();
}
