using UnityEngine;
using Haptikos.Gloves;
using System.Collections.Generic;
using Haptikos.UI;
using System.Collections;
using Haptikos;
using Haptikos.Exoskeleton;

public class HapticFeedbackUI : MonoBehaviour
{

    public void ButtonFeedback( HaptikosExoskeleton hand, bool isClick = false)
    {
        StartCoroutine(ButtonFeedbackEnum( hand, isClick));
    }

    public IEnumerator ButtonFeedbackEnum( HaptikosExoskeleton _hand, bool isClick = false)
    {
        HaptikosExoskeleton hand = _hand; // Set the interacting hand type (Left or Right)
      

        float intensity = (isClick) ? 15f : 5f;
        SendHapticFeedback(hand, intensity);
        yield return new WaitForSeconds(0.1f);
       
        if (hand != null)
        {
            StopHapticFeedback(hand);
        }
    }

    private void SendHapticFeedback(HaptikosExoskeleton glove, float intensity)
    {
        if (glove == null)
        {
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
            return;
        }
        glove.uDPReciever.SendHapticData($"index3 on@0");
        glove.uDPReciever.SendHapticData($"middle3 on@0");
        glove.uDPReciever.SendHapticData($"ring3 on@0");
        glove.uDPReciever.SendHapticData($"pinky3 on@0");
        glove.uDPReciever.SendHapticData($"thumb3 on@0");

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