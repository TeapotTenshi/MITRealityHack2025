using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Haptikos.Gloves;
using Haptikos.Exoskeleton;

public class HapticInteraction : MonoBehaviour
{
    public CustomVibrationCurve curveData;      // Reference to the ScriptableObject containing the curve
    public CustomVibrationCurve curveDataEnter;
    public CustomVibrationCurve curveDataOnExit; // Curve for exit vibration
    public HaptikosExoskeleton leftGlove, rightGlove;

    private float axisLength;
    private float axisLengthEnter;
    private float axisLengthExit;
    private bool isColliding = false;
    private bool isExiting = false;

    // Start is called before the first frame update
    void Start()
    {
        axisLength = curveData.curve.keys[curveData.curve.keys.Length - 1].time;
        axisLengthEnter = curveDataEnter.curve.keys[curveDataEnter.curve.keys.Length - 1].time;
        axisLengthExit = curveDataOnExit.curve.keys[curveDataOnExit.curve.keys.Length - 1].time;
    }

    public void SetCollisionState(bool state)
    {
        if (state)
        {
            StartCoroutine(TriggerVibrationEnter());
        }
        else
        {
            StartCoroutine(TriggerVibrationExit());
        }
    }

    private IEnumerator TriggerVibrationEnter()
    {
        isColliding = true;
        float elapsedTime = 0f;

        while (elapsedTime < axisLengthEnter)
        {

            Debug.Log("enter");
            float normalizedTime = elapsedTime / axisLengthEnter;
            float intensity = curveDataEnter.curve.Evaluate(normalizedTime);

            // Send haptic feedback
            leftGlove.uDPReciever.SendHapticData("index3 on@" + intensity);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Ensure vibrations stop after the enter event is processed
        StopHaptics();
        isColliding = false;
    }

    private IEnumerator TriggerVibrationExit()
    {
        isExiting = true;
        float elapsedTime = 0f;

        while (elapsedTime < axisLengthExit)
        {
            Debug.Log("exit");
            float normalizedTime = elapsedTime / axisLengthExit;
            float intensity = curveDataOnExit.curve.Evaluate(normalizedTime);

            // Send haptic feedback
            leftGlove.uDPReciever.SendHapticData("index3 on@" + intensity);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Ensure vibrations stop after the exit event is processed
        StopHaptics();
        isExiting = false;
    }

    private void StopHaptics()
    {
        leftGlove.uDPReciever.SendHapticData("index3 off@0");
        leftGlove.uDPReciever.SendHapticData("middle3 off@0");
        leftGlove.uDPReciever.SendHapticData("ring3 off@0");
        leftGlove.uDPReciever.SendHapticData("pinky3 off@0");
        leftGlove.uDPReciever.SendHapticData("thumb3 off@0");
    }
}

