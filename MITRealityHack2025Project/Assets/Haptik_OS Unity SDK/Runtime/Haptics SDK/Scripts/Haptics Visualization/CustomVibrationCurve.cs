using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

[CreateAssetMenu(fileName = "CustomVibrationCurve", menuName = "ScriptableObjects/CustomVibrationCurve", order = 1)]


public class CustomVibrationCurve : ScriptableObject
{
    public AnimationCurve curve = new AnimationCurve();
    public float startValue = 0;
    public float endValue = 15;
    public float duration = 1;

    public float defaultduration = 1;

    public AnimationCurve resetcurve = new AnimationCurve();
    public AnimationCurve startingCurve = null;
    public int times = 0;
    public AnimationCurve originalCurve = new AnimationCurve();
    public AnimationCurve editedCurve = new AnimationCurve();

    public Keyframe[] customKeyframes;
    public List<Keyframe> editableKeyframes = new List<Keyframe>();
    public AudioClip audioClip;
    public void ResetCurve()
    {
        // Clear the curve and reset it to an empty AnimationCurve
        curve = new AnimationCurve();
        editedCurve = new AnimationCurve();
        originalCurve = new AnimationCurve();
        // Clear the editable keyframes list
        editableKeyframes.Clear();
    }

    public void ConvertAudioToCurve(int sampleCount)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("No audio clip assigned.");
            return;
        }

        float[] samples = new float[audioClip.samples];
        audioClip.GetData(samples, 0);

        originalCurve = new AnimationCurve();
        //editedCurve = new AnimationCurve();

        int step = Mathf.Max(1, audioClip.samples / sampleCount);

        float maxA = 0f;
        foreach (float sample in samples)
        {
            maxA = Mathf.Max(maxA, Mathf.Abs(sample));
        }

        if (maxA == 0f)
        {
            Debug.LogWarning("The audio clip has no audible signal (max amplitude is zero).");
            return;
        }

        for (int i = 0; i < sampleCount; i++)
        {
            float time = (float)i / sampleCount * audioClip.length;
            float amplitude = Mathf.Abs(samples[i * step]);
            //float amplitude = samples[i * step];

            // Scale amplitude to the range [minAmplitude, maxAmplitude]
            float scaledAmplitude = Mathf.Lerp(0, 15, amplitude / maxA);
            //float scaledAmplitude = amplitude;


            // Add the key to the original curve
            originalCurve.AddKey(new Keyframe(time, scaledAmplitude));
        }
        // Find the maximum amplitude in the clip for normalization
    }

    public void ApplyEditsToCurve(float minAmplitude, float maxAmplitude, float startDuration, float endDuration, float keyframeInterval)
    {
        if (originalCurve == null || originalCurve.keys.Length == 0)
        {
            Debug.Log("Original curve is empty.");
            return;
        }

        editedCurve = new AnimationCurve();

        foreach (Keyframe originalKey in originalCurve.keys)
        {
            if (originalKey.time > endDuration || originalKey.time < startDuration)
                continue;

            float newTime = originalKey.time - startDuration;
            //float adjustedValue = originalKey.time < startDuration ? 0f : Mathf.Lerp(minAmplitude, maxAmplitude, originalKey.value / 15f);
            float adjustedValue = Mathf.Lerp(minAmplitude, maxAmplitude, originalKey.value / 15f);


            if (editedCurve.keys.Length == 0 || (newTime - editedCurve.keys[editedCurve.keys.Length - 1].time) >= keyframeInterval)
            {
                editedCurve.AddKey(new Keyframe(newTime, adjustedValue));
            }
        }
    }
    public void PeaksOnlyCurve(float startDuration)
    {
        if (editedCurve == null || editedCurve.keys.Length < 3)
        {
            Debug.Log("Edited curve is empty.");
            return;
        }

        curve = new AnimationCurve();

        curve.AddKey(new Keyframe(0, 0));

        for (int i = 1; i < editedCurve.keys.Length - 1; i++)
        {
            Keyframe previousKey = editedCurve.keys[i - 1];
            Keyframe nextKey = editedCurve.keys[i + 1];
            Keyframe currentKey = editedCurve.keys[i];

            if (currentKey.value > previousKey.value && currentKey.value > nextKey.value)
            {
                if (currentKey.value < 5f)
                {
                    curve.AddKey(new Keyframe(currentKey.time, 0));
                }
                else
                {
                    //float adjustedValue = currentKey.time < startDuration ? 0f : currentKey.value;
                    //curve.AddKey(new Keyframe(currentKey.time, adjustedValue));
                    curve.AddKey(new Keyframe(currentKey.time, currentKey.value));
                }
            }
        }
        SyncKeyframesToEditableList();
    }


    public void RescaleCurveDuration(float targetDuration)
    {
        if (curve == null || curve.keys.Length == 0)
        {
            Debug.LogWarning("No curve available to modify.");
            return;
        }

        // Get the original start, end, and duration values
        float originalDuration = curve.keys[curve.keys.Length - 1].time;

        float timeScale = targetDuration / originalDuration;

        // Create a new curve with adjusted keyframes and preserved tangents
        AnimationCurve rescaledCurve = new AnimationCurve();

        foreach (var key in curve.keys)
        {
            // Adjust keyframe time and value based on new start, end, and duration
            float newTime = key.time * timeScale;

            Keyframe newKey = new Keyframe(newTime, key.value)
            {
                inTangent = key.inTangent / timeScale,
                outTangent = key.outTangent / timeScale
                //tangentMode = key.tangentMode
            };

            rescaledCurve.AddKey(newKey);
        }

        // Replace the existing curve with the modified version
        curve = rescaledCurve;
        SyncKeyframesToEditableList();
    }

    public void ResetRescale()
    {
        RescaleCurveDuration(defaultduration);
    }

    public void RepeatCurve(int repeatCount)
    {
        times++;

        if (curve == null || curve.keys.Length == 0)
        {
            Debug.LogWarning("No curve available to modify.");
            return;
        }

        if (startingCurve == null)
        {
            startingCurve = new AnimationCurve(curve.keys);
        }
        

        // Store the original curve for resetting later
        //resetcurve = new AnimationCurve(curve.keys);

        // Get the original curve's duration
        float originalDuration = curve.keys[curve.keys.Length - 1].time;

        // Create a new curve for the repeated segments
        AnimationCurve repeatedCurve = new AnimationCurve();

        // Loop through each repetition
        for (int i = 0; i <= repeatCount-1; i++)
        {
            foreach (var key in curve.keys)
            {
                // Calculate the new time for each keyframe in the repeated segment
                float repeatedTime = key.time + (i * originalDuration);

                // Apply a small offset (0.01) to the first key of each repeated segment (except the first one)
                if (i > 0 && key.time == 0)
                {
                    repeatedTime += 0.01f; // Add the offset to avoid overlapping with the previous segment's end
                }

                // Create a new keyframe with the adjusted time and the same value and tangents
                Keyframe newKey = new Keyframe(repeatedTime, key.value)
                {
                    inTangent = key.inTangent,
                    outTangent = key.outTangent
                };

                // Ensure continuity for linear curves
                if (i > 0 && key.time == 0)
                {
                    // Adjust tangents at the boundary
                    Keyframe lastKey = repeatedCurve.keys[repeatedCurve.keys.Length - 1];
                    lastKey.outTangent = 0;
                    newKey.inTangent = 0;

                    // Move the last key with the adjusted tangent
                    repeatedCurve.MoveKey(repeatedCurve.keys.Length - 1, lastKey);
                }

                // Add the new key to the repeated curve
                repeatedCurve.AddKey(newKey);
            }
        }

        if (times == 1)
        {
            startingCurve = curve;
        }
        // Replace the existing curve with the modified version
        curve = repeatedCurve;
        duration = originalDuration * (repeatCount + 1);
        SyncKeyframesToEditableList();
    }

    public void ResetRepeat()
    {
        times = 0;
        if (startingCurve != null && startingCurve.keys.Length > 0)
        {
            curve = new AnimationCurve(startingCurve.keys);
            duration = startingCurve.keys[startingCurve.keys.Length - 1].time;
            SyncKeyframesToEditableList();
        }
        else
        {
            Debug.Log("error");
        }
        // Restore the original curve and duration
        //curve = new AnimationCurve(resetcurve.keys);
        //duration = resetcurve.keys[resetcurve.keys.Length - 1].time;
    }


    public float Evaluate(float time)
    {
        return curve.Evaluate(time);
    }

    // Method to synchronize editableKeyframes with the actual curve's keyframes
    private void SyncKeyframesToEditableList()
    {
        editableKeyframes = new List<Keyframe>(curve.keys);
    }

    // Method to apply edited keyframes back to the curve
    public void ApplyEditableKeyframes()
    {
        curve = new AnimationCurve(editableKeyframes.ToArray());
    }

    // Method to add a new keyframe to the editable list
    public void AddKeyframe(float time, float value, float inTangent, float outTangent)
    {
        // Create a new keyframe with specified tangents
        Keyframe newKeyframe = new Keyframe(time, value, inTangent, outTangent);

        // Add it to the editable keyframes list
        editableKeyframes.Add(newKeyframe);

        // Apply changes immediately to the curve
        ApplyEditableKeyframes();
    }


    // Method to remove all keyframes
    public void RemoveAllKeyframes()
    {
        editableKeyframes.Clear();
        curve = new AnimationCurve(); // Clear the curve itself
    }

    /*----------------------------------------------------------------------------*/
    /*------------------------ GENERATE PATTERNS METHODS -------------------------*/
    /*----------------------------------------------------------------------------*/

    public void GenerateLinear()
    {
        startValue = 0f;
        endValue = 10f;
        duration = 1f;

        curve = AnimationCurve.Linear(0, startValue, duration, endValue);
        SyncKeyframesToEditableList();
    }

    public void GenerateInverseLinear()
    {
        startValue = 10f;
        endValue = 0f;
        duration = 1f;

        curve = AnimationCurve.Linear(0, startValue, duration, endValue);
        SyncKeyframesToEditableList();
    }

    public void GenerateSine()
    {

        startValue = 0f;
        endValue = 5f;
        duration = 1f;

        curve = new AnimationCurve();
        int numPoints = 10;
        for (int i = 0; i <= numPoints; i++)
        {
            float t = (float)i / numPoints * duration;
            float frequency = 1f / duration;
            float value = Mathf.Sin(t * Mathf.PI * 2 * frequency) * (endValue - startValue) / 2 + (endValue + startValue) / 2;

            curve.AddKey(t, value);
        }
        SyncKeyframesToEditableList();
    }

    public void GenerateExponential()
    {
        startValue = 0f;
        endValue = 10f;
        duration = 1f;

        curve = new AnimationCurve();
        int numPoints = 10;
        for (int i = 0; i <= numPoints; i++)
        {
            float t = (float)i / numPoints * duration;
            float value = startValue + (endValue - startValue) * Mathf.Pow(10, (t / duration) - 1);
            curve.AddKey(t, value);
        }
        SyncKeyframesToEditableList();
    }

    public void GenerateInverseExponential()
    {
        // Set default values for Inverse Exponential
        startValue = 10f;
        endValue = 0f;
        duration = 1f;

        curve = new AnimationCurve();
        int numPoints = 10;
        for (int i = 0; i <= numPoints; i++)
        {
            float t = (float)i / numPoints * duration;
            float value = endValue - (endValue - startValue) * Mathf.Pow(10, (t / duration) - 1);
            value = startValue - value;
            curve.AddKey(t, value);
        }
        SyncKeyframesToEditableList();
    }


    public void ApplyPatternModifications(float startingValue, float endingValue, float durationValue)
    {
        // Set the new values
        startValue = startingValue;
        endValue = endingValue;
        duration = durationValue;

        // Check if the curve has any keyframes to modify
        if (curve == null || curve.keys.Length == 0)
        {
            Debug.LogWarning("No curve available to modify.");
            return;
        }

        // Determine if the curve is inverted by comparing the first and last key values
        bool isInverted = curve.keys[0].value > curve.keys[curve.keys.Length - 1].value;

        // Calculate target values based on inversion
        float targetMinValue = isInverted ? endValue : startValue;
        float targetMaxValue = isInverted ? startValue : endValue;

        // Calculate the original amplitude and midpoint of the existing curve
        float originalMinValue = curve.keys.Select(key => key.value).Min();
        float originalMaxValue = curve.keys.Select(key => key.value).Max();
        float originalAmplitude = (originalMaxValue - originalMinValue) / 2f;
        float originalMidpoint = (originalMaxValue + originalMinValue) / 2f;

        // Calculate the target amplitude and midpoint based on inversion-aware min and max
        float newAmplitude = (targetMaxValue - targetMinValue) / 2f;
        float newMidpoint = (targetMaxValue + targetMinValue) / 2f;

        // Calculate scaling factors for both amplitude and time
        float valueScale = originalAmplitude == 0 ? 1 : newAmplitude / originalAmplitude;
        float timeScale = duration / curve.keys[curve.keys.Length - 1].time;

        // Create a new curve to hold the modified keyframes
        AnimationCurve modifiedCurve = new AnimationCurve();

        foreach (var key in curve.keys)
        {
            // Scale the keyframe's time and value to fit the new amplitude and duration
            float newTime = key.time * timeScale;
            float newValue = newMidpoint + (key.value - originalMidpoint) * valueScale;

            // Preserve tangents for smooth transitions
            Keyframe newKey = new Keyframe(newTime, newValue)
            {
                inTangent = key.inTangent * valueScale / timeScale,
                outTangent = key.outTangent * valueScale / timeScale
            };

            modifiedCurve.AddKey(newKey);
        }

        // Replace the original curve with the modified version
        curve = modifiedCurve;
        SyncKeyframesToEditableList();

        // Reset modification fields for a clean UI
        startValue = 0f;
        endValue = 0f;
        duration = 0f;
    }


}