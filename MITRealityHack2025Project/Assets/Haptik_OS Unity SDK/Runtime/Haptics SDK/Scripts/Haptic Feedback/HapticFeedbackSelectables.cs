using UnityEngine;
using Haptikos.Gloves;
using System.Collections.Generic;
using Haptikos.UI;
using System.Collections;
using Haptikos;
using Haptikos.Exoskeleton;

public class HapticFeedbackSelectables : MonoBehaviour
{
    public CustomVibrationCurve curve;
    private float elapsedTime = 0f;

    private float axisLength => curve.curve.keys[curve.curve.keys.Length - 1].time;

    //private HashSet<string> activeFingers = new HashSet<string>();
    private HandType activeHandType; // Tracks which hand (left/right) is interacting
    public HaptikosExoskeleton hand; // Gloves for left and right hand

    [SerializeField] HaptikosSelectableButton hapticButton;

    bool isTriggered;
    bool isClick;

    public void SetCollisionState(bool state, HaptikosExoskeleton hand, bool stopAfterDelay = false)
    {
        StartCoroutine(SetCollisionStateCoroutine(state, hand, stopAfterDelay));
    }

    public IEnumerator SetCollisionStateCoroutine(bool state, HaptikosExoskeleton _hand, bool _isClick = false)
    {
        isTriggered = state;
        hand = _hand; // Set the interacting hand type (Left or Right)
        isClick = _isClick;

        
        yield return new WaitForSeconds(0.1f);

        ////activeFingers.Remove(mappedFinger);
        if (hand != null)
        {
            StopHapticFeedback(hand);
        }
        ResetState();
        
    }

    //private void RegisterHapticFeedbackListener()
    //{
    //    if (TryGetComponent<HaptikosSelectableButton>(out hapticButton))
    //    {

    //        this.hapticButton.OnHapticFeedback.AddListener((state, hand, isClick) =>
    //        {
    //            SetCollisionState(state, hand, isClick);
    //        });
    //    }
    //}

    //private void OnEnable()
    //{
    //    RegisterHapticFeedbackListener();
    //}

    //private void OnDisable()
    //{
    //    if (hapticButton != null)
    //    {
    //        this.hapticButton.OnHapticFeedback.RemoveAllListeners();
    //    }
    //}

    private void Awake()
    {
    }

    private void Update()
    {
        if (isTriggered)
        {
            if (elapsedTime < axisLength)
            { 
                float normalizedTime = elapsedTime / axisLength;
                float intensity = curve.curve.Evaluate(normalizedTime);
                if (!isClick)
                {
                    intensity /= 2;
                }
                SendHapticFeedback(hand, intensity);
                elapsedTime += Time.deltaTime;
            }
            else
            {
                ResetState();
            }
        }
        
    }


    private void SendHapticFeedback(HaptikosExoskeleton glove, float intensity)
    {
        if (glove == null)
        {
            Debug.Log("NULL");
            return;
        }
        glove.uDPReciever.SendHapticData($"index3 on@{intensity}");
        glove.uDPReciever.SendHapticData($"middle3 on@{intensity}");
        glove.uDPReciever.SendHapticData($"ring3 on@{intensity}");
        glove.uDPReciever.SendHapticData($"pinky3 on@{intensity}");
        glove.uDPReciever.SendHapticData($"thumb3 on@{intensity}");
    }

    private void StopHapticFeedback(HaptikosExoskeleton glove)
    {
        if (glove == null)
        {
            Debug.Log("NULL");
            return;
        }
        glove.uDPReciever.SendHapticData($"index3 on@0");
        glove.uDPReciever.SendHapticData($"middle3 on@0");
        glove.uDPReciever.SendHapticData($"ring3 on@0");
        glove.uDPReciever.SendHapticData($"pinky3 on@0");
        glove.uDPReciever.SendHapticData($"thumb3 on@0");

    }

    private void ResetState()
    {
        elapsedTime = 0f;
        isTriggered = false;
        StopHapticFeedback(hand);
    }

    private void OnApplicationQuit()
    {
        StartCoroutine(StopAllHaptics());
    }

    private IEnumerator StopAllHaptics()
    {
        yield return new WaitForSeconds(0.5f);

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
        StartCoroutine(StopAllHaptics());
    }
}