using UnityEngine;
using Haptikos.Gloves;
using System.Collections.Generic;
using Haptikos.UI;
using System.Collections;
using Haptikos;
using Haptikos.Exoskeleton;

public class HapticFeedback : MonoBehaviour
{
    public CustomVibrationCurve curve;
    private float elapsedTime = 0f;

    private float axisLength => curve.curve.keys[curve.curve.keys.Length - 1].time;
    private bool isColliding = false;

    public bool isContinuous = false;
    private HashSet<string> activeFingers = new HashSet<string>();
    private HandType activeHandType; // Tracks which hand (left/right) is interacting

    [HideInInspector]
    public HaptikosExoskeleton leftGlove, rightGlove; // Gloves for left and right hand

    [SerializeField] HapticItem hapticItem;

    private Dictionary<string, string> fingerMappings = new Dictionary<string, string>
    {
        { "index1", "index3" },
        { "index2", "index3" },
        { "index3", "index3" },
        { "middle1", "middle3" },
        { "middle2", "middle3" },
        { "middle3", "middle3" },
        { "ring1", "ring3" },
        { "ring2", "ring3" },
        { "ring3", "ring3" },
        { "pinky1", "pinky3" },
        { "pinky2", "pinky3" },
        { "pinky3", "pinky3" },
        { "thumb1", "thumb3" },
        { "thumb2", "thumb3" },
        { "thumb3", "thumb3" },
        { "b_r_wrist", "b_l_wrist" },
        { "wrist",  "wrist" }
  };

    public void SetCollisionState(bool state, string finger, HandType type, bool stopAfterDelay = false)
    {
        StartCoroutine(SetCollisionStateCoroutine(state, finger, type, stopAfterDelay));
    }

    public IEnumerator SetCollisionStateCoroutine(bool state, string finger, HandType type, bool stopAfterDelay = false)
    {
        if (!fingerMappings.ContainsKey(finger))
        {
            Debug.LogWarning($"Finger '{finger}' is not in the mapping dictionary.");
            yield break;
        }

        string mappedFinger = fingerMappings[finger];

        activeHandType = type; // Set the interacting hand type (Left or Right)

        if (state)
        {
            activeFingers.Add(mappedFinger);
        }
        else
        {
            activeFingers.Remove(mappedFinger);
        }    

        isColliding = activeFingers.Count > 0;

        if (stopAfterDelay)
        {
            yield return new WaitForSeconds(axisLength);

            activeFingers.Remove(mappedFinger);

            HaptikosExoskeleton activeGlove = GetActiveGlove();
            if (activeGlove != null)
            {
                StopHapticFeedback(activeFingers, activeGlove);
            }
            ResetState();
        }
    }

    private void RegisterHapticFeedbackListener()
    {

        if (TryGetComponent<HapticItem>(out var hapticItemListener))
        {
            this.hapticItem = hapticItemListener;

            this.hapticItem.onHapticFeedbackStarted.AddListener((state, finger, type) =>
            {
                SetCollisionState(state, finger, type);
            });


            this.hapticItem.onHapticFeedbackStartAndEnd.AddListener((state, finger, type, end) =>
            {
                SetCollisionState(state, finger, type, end);
            });
        }
    }

    private void Start()
    {
        leftGlove = HaptikosPlayer.GetExoskeleton(HandType.LeftHand);
        rightGlove = HaptikosPlayer.GetExoskeleton(HandType.RightHand);
    }

    private void OnEnable()
    {
        RegisterHapticFeedbackListener();
    }

    private void OnDisable()
    {
        if (hapticItem != null)
        {
            this.hapticItem.onHapticFeedbackStarted.RemoveAllListeners();
            this.hapticItem.onHapticFeedbackStartAndEnd.RemoveAllListeners();
        }
    }

    private void FixedUpdate()
    {
        if (isColliding)
        {
            HaptikosExoskeleton activeGlove = GetActiveGlove(); // Determine which glove to send haptic feedback to

            if (isContinuous)
            {

                if (elapsedTime < axisLength)
                {
                    float normalizedTime = elapsedTime / axisLength;
                    float intensity = curve.curve.Evaluate(normalizedTime);

                    SendHapticFeedback(activeFingers, activeGlove, intensity);
                    elapsedTime += Time.deltaTime;
                }
                else
                {
                    elapsedTime = 0f;
                }
            }
            else
            {
                if (elapsedTime < axisLength)
                {
                    float normalizedTime = elapsedTime / axisLength;
                    float intensity = curve.curve.Evaluate(normalizedTime);
                    SendHapticFeedback(activeFingers, activeGlove, intensity);
                    elapsedTime += Time.deltaTime;
                }
            }
        }
        else
        {
            HaptikosExoskeleton activeGlove = GetActiveGlove();
            if (activeGlove != null)
            {
                StopHapticFeedback(activeFingers, activeGlove);
            }
            ResetState();
        }
    }

    private HaptikosExoskeleton GetActiveGlove()
    {
        return activeHandType == HandType.LeftHand ? leftGlove : rightGlove;
    }

    /// <summary>
    /// Adding wrist as input, vibrates the whole hand
    /// </summary>
    private void SendHapticFeedback(HashSet<string> fingers, HaptikosExoskeleton glove, float intensity)
    {
        foreach (var finger in fingers)
        {
            if (finger.Contains("wrist"))
            {
                glove.uDPReciever.SendHapticData($"index3 on@{intensity}");
                glove.uDPReciever.SendHapticData($"middle3 on@{intensity}");
                glove.uDPReciever.SendHapticData($"ring3 on@{intensity}");
                glove.uDPReciever.SendHapticData($"pinky3 on@{intensity}");
                glove.uDPReciever.SendHapticData($"thumb3 on@{intensity}");
            }
            else
            {
                glove.uDPReciever.SendHapticData($"{finger} on@{intensity}");
            }
        }
    }

    private void StopHapticFeedback(HashSet<string> fingers, HaptikosExoskeleton glove)
    {
        foreach (var finger in fingers)
        {
            if (finger.Contains("wrist"))
            {
                glove.uDPReciever.SendHapticData($"index3 on@0");
                glove.uDPReciever.SendHapticData($"middle3 on@0");
                glove.uDPReciever.SendHapticData($"ring3 on@0");
                glove.uDPReciever.SendHapticData($"pinky3 on@0");
                glove.uDPReciever.SendHapticData($"thumb3 on@0");
            }
            else
            {
                glove.uDPReciever.SendHapticData($"{finger} on@0");
            }
        }
        fingers.Clear();
    }

    private void ResetState()
    {
        elapsedTime = 0f;
        isColliding = false;
    }

    private void OnApplicationQuit()
    {
        StartCoroutine(StopAllHaptics(0.5f));
    }

    public static IEnumerator StopAllHaptics(float time)
    {
        yield return new WaitForSeconds(time);

        foreach (var glove in HaptikosPlayer.exoskeletonGloves)
        {
            yield return null;

            glove.uDPReciever.SendHapticData($"index3 off@0");
            glove.uDPReciever.SendHapticData($"middle3 off@0");
            glove.uDPReciever.SendHapticData($"ring3 off@0");
            glove.uDPReciever.SendHapticData($"pinky3 off@0");
            glove.uDPReciever.SendHapticData($"thumb3 off@0");
        }
    }

   

    private void OnApplicationPause(bool pause)
    {
        StartCoroutine(StopAllHaptics(0.5f));
    }
}