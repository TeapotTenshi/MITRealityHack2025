using UnityEngine;
using UnityEditor;
using Haptikos.Gloves;
using System.Collections;
using System.Collections.Generic;
using Haptikos.Exoskeleton;
using Haptikos;
using TMPro;

public class CurveCursorVisualizer : MonoBehaviour
{
    public CustomVibrationCurve curveData; // Reference to the ScriptableObject containing the curve
    private float elapsedTime = 0f; // Tracks elapsed time

    private float maxBackgroundWidth = 10f; // Fixed width for the background box
    private float maxBackgroundHeight = 5f; // Fixed height for the background box
    public Color backgroundColor = new Color(0.9f, 0.9f, 0.9f, 0.5f); // Background color
    private Color gridColor = new Color(0.7f, 0.7f, 0.7f, 0.2f);  // Grid line color
    private int gridXLines = 10; // Number of vertical grid lines
    private int gridYLines = 5; // Number of horizontal grid lines

    private float xScaleFactor; // Scale factor for X-axis to fit within the background
    private float yScaleFactor; // Scale factor for Y-axis to fit within the background

    private float axisLength; // Length of the X-axis based on curve duration
    private float yAxisMax; // Maximum Y-axis value based on the curve

    private bool isPlaying = false;
    private bool isLooping = false;

    [SerializeField] private TMP_Dropdown dropdown;
    private int gloveindex = 0;

    public HaptikosExoskeleton leftGlove, rightGlove;

    [SerializeField] Camera visualizationCamera;

    private void OnValidate()
    {
        UpdateScaleFactors();
    }

    private void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropDownValueChanged);
        leftGlove = HaptikosPlayer.GetExoskeleton(HandType.LeftHand);
        rightGlove = HaptikosPlayer.GetExoskeleton(HandType.RightHand);

        visualizationCamera.enabled = true;
    }

    void OnDropDownValueChanged(int index)
    {
        if (index == 0)
        {
            gloveindex = 0;
            //Debug.Log("Left Glove");
        }
        else if (index == 1)
        {
            gloveindex = 1;
            //Debug.Log("Right Glove");
        }
    }


    private void FixedUpdate()
    {
        if (isPlaying)
        {

            if (elapsedTime < axisLength)
            {

                float normalizedTime = elapsedTime / axisLength;
                float intensity = curveData.curve.Evaluate(normalizedTime);
                if (gloveindex == 0)
                {
                    leftGlove.uDPReciever.SendHapticData("index3 on@" + intensity);
                    leftGlove.uDPReciever.SendHapticData("middle3 on@" + intensity);
                    leftGlove.uDPReciever.SendHapticData("ring3 on@" + intensity);
                    leftGlove.uDPReciever.SendHapticData("pinky3 on@" + intensity);
                    leftGlove.uDPReciever.SendHapticData("thumb3 on@" + intensity);
                    elapsedTime += Time.fixedDeltaTime;
                }
                else
                {
                    rightGlove.uDPReciever.SendHapticData("index3 on@" + intensity);
                    rightGlove.uDPReciever.SendHapticData("middle3 on@" + intensity);
                    rightGlove.uDPReciever.SendHapticData("ring3 on@" + intensity);
                    rightGlove.uDPReciever.SendHapticData("pinky3 on@" + intensity);
                    rightGlove.uDPReciever.SendHapticData("thumb3 on@" + intensity);
                    elapsedTime += Time.fixedDeltaTime;
                }
            }
            else
            {
                if (isLooping)
                {
                    elapsedTime = 0;
                }
                else
                {
                    elapsedTime = axisLength; // Reset to loop the animation
                    if (gloveindex == 0)
                    {
                        leftGlove.uDPReciever.SendHapticData("index3 off@0");
                        leftGlove.uDPReciever.SendHapticData("middle3 off@0");
                        leftGlove.uDPReciever.SendHapticData("ring3 off@0");
                        leftGlove.uDPReciever.SendHapticData("pinky3 off@0");
                        leftGlove.uDPReciever.SendHapticData("thumb3 off@0");
                    }
                    else
                    {
                        rightGlove.uDPReciever.SendHapticData("index3 off@0");
                        rightGlove.uDPReciever.SendHapticData("middle3 off@0");
                        rightGlove.uDPReciever.SendHapticData("ring3 off@0");
                        rightGlove.uDPReciever.SendHapticData("pinky3 off@0");
                        rightGlove.uDPReciever.SendHapticData("thumb3 off@0");
                    }
                    isPlaying = false;
                }
            }
        }
    }

    private void UpdateScaleFactors()
    {
        if (curveData != null && curveData.curve.keys.Length > 0)
        {
            // Calculate axisLength based on the last key's time in the curve
            axisLength = curveData.curve.keys[curveData.curve.keys.Length - 1].time;

            // Find maximum Y value across all keyframes in the curve
            yAxisMax = 0f;
            foreach (var key in curveData.curve.keys)
            {
                yAxisMax = Mathf.Max(yAxisMax, Mathf.Abs(key.value));
            }

            yAxisMax = Mathf.Ceil(yAxisMax / 5f) * 5f;

            // Calculate scale factors to fit the curve within the fixed background box
            xScaleFactor = maxBackgroundWidth / axisLength;
            yScaleFactor = maxBackgroundHeight / yAxisMax;
        }
    }

    private void OnDrawGizmos()
    {
        if (curveData == null || curveData.curve == null) return; // Exit if no curve data is assigned

        UpdateScaleFactors(); // Ensure scale factors are updated for editor view

        // Draw the background box with a grid
        DrawBackgroundWithGrid();

        // Draw the X and Y axes with labels
        DrawAxesWithLabels();

        // Draw the entire curve within the Scene view
        DrawCurveGizmos();

        // Draw a moving cursor that follows the curve
        DrawMovingCursor();
    }

    private void DrawBackgroundWithGrid()
    {
        Gizmos.color = backgroundColor;

        // Draw a fixed background box of size 10x5 units
        Vector3 backgroundCenter = transform.position + new Vector3(maxBackgroundWidth / 2, maxBackgroundHeight / 2, 0);
        Gizmos.DrawCube(backgroundCenter, new Vector3(maxBackgroundWidth, maxBackgroundHeight, 0.01f));

        // Draw grid lines
        Gizmos.color = gridColor;

        // Vertical grid lines
        for (int i = 1; i < gridXLines; i++)
        {
            float x = i * (maxBackgroundWidth / gridXLines);
            Vector3 start = transform.position + new Vector3(x, 0, 0);
            Vector3 end = transform.position + new Vector3(x, maxBackgroundHeight, 0);
            Gizmos.DrawLine(start, end);
        }

        float yTicks = 2.5f;
        for (float yValue = yTicks; yValue <= yAxisMax; yValue += yTicks)
        {
            float yPos = yValue * yScaleFactor;
            Vector3 start = transform.position + new Vector3(0f, yPos, 0);
            Vector3 end = transform.position + new Vector3(maxBackgroundWidth, yPos, 0);
            Gizmos.DrawLine(start, end);
        }

    }

    private void DrawAxesWithLabels()
    {
        Gizmos.color = Color.black; // Set axis color to black

        // Draw the X-axis
        Vector3 xAxisStart = transform.position;
        Vector3 xAxisEnd = transform.position + new Vector3(maxBackgroundWidth, 0, 0);
        Gizmos.DrawLine(xAxisStart, xAxisEnd);

        // Draw the Y-axis
        Vector3 yAxisStart = transform.position;
        Vector3 yAxisEnd = transform.position + new Vector3(0, maxBackgroundHeight, 0);
        Gizmos.DrawLine(yAxisStart, yAxisEnd);

        // Set label color to black
        Handles.color = Color.black;

        // Draw X-axis labels
        int xTicks = 5; // Number of ticks along the X-axis
        for (int i = 0; i <= xTicks; i++)
        {
            float t = i / (float)xTicks * axisLength;
            Vector3 tickPosition = transform.position + new Vector3(i / (float)xTicks * maxBackgroundWidth, 0f, 0);
            Gizmos.DrawSphere(tickPosition, 0.04f);
            Handles.Label(tickPosition + Vector3.down * 0.3f + Vector3.left * 0.15f, t.ToString("F1"));
        }

        float yTicks = 2.5f;
        for (float yValue = yTicks; yValue <= yAxisMax; yValue += yTicks)
        {
            float yPos = yValue * yScaleFactor;
            Vector3 tickPosition = transform.position + new Vector3(0f, yPos, 0);
            Gizmos.DrawSphere(tickPosition, 0.04f);
            Handles.Label(tickPosition + Vector3.left * 0.7f, yValue.ToString("F1"));
        }

    }

    private void DrawCurveGizmos()
    {
        Gizmos.color = Color.green;

        // Start with the first point on the curve
        float previousValue = curveData.curve.Evaluate(0) * yScaleFactor;
        Vector3 previousPosition = transform.position + new Vector3(0, previousValue, 0);

        // Iterate over the duration to draw the scaled curve
        for (float t = 0; t <= axisLength; t += 0.01f)
        {
            float scaledTime = t / axisLength * maxBackgroundWidth;
            float currentValue = curveData.curve.Evaluate(t) * yScaleFactor;

            Vector3 currentPosition = transform.position + new Vector3(scaledTime, currentValue, 0);

            Gizmos.DrawLine(previousPosition, currentPosition);

            previousPosition = currentPosition;
        }
    }

    private void DrawMovingCursor()
    {
        float scaledTime = elapsedTime * xScaleFactor;
        float curveValue = curveData.curve.Evaluate(elapsedTime) * yScaleFactor;

        Vector3 cursorPosition = transform.position + new Vector3(scaledTime, curveValue, 0);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(cursorPosition, 0.1f);
    }


    public void StartPlayback()
    {
        elapsedTime = 0f;
        isPlaying = true;
    }

    public void PausePlayback()
    {
        isPlaying = !isPlaying;
    }

    public void ResetPlayback()
    {
        elapsedTime = 0f;
        isPlaying = false;
    }

    public void LoopPlayback()
    {
        isLooping = !isLooping;
    }
}