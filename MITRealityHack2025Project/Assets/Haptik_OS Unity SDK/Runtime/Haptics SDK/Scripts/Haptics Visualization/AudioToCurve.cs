using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioToCurveEditor : MonoBehaviour
{
    public AnimationCurve preCalculatedIntensityCurve; // Pre-calculated curve to use during playback
    public int sampleSize = 1024; // Number of samples per RMS calculation (adjust as needed)

    private AudioSource audioSource;
    private float[] samples;

    void OnValidate()
    {
        // Generate the curve only in the editor
        audioSource = GetComponent<AudioSource>();

        if (audioSource.clip != null)
        {
            // Generate and store the curve when the audio clip is assigned or properties change
            GenerateIntensityCurve(audioSource.clip);
        }
    }

    private void GenerateIntensityCurve(AudioClip audioClip)
    {
        preCalculatedIntensityCurve = new AnimationCurve();
        int totalSamples = audioClip.samples;
        int sampleRate = audioClip.frequency;

        // Prepare the samples array
        samples = new float[sampleSize];

        // Loop over audio data in sample-sized chunks to calculate RMS
        for (int i = 0; i < totalSamples; i += sampleSize)
        {
            // Read audio samples
            audioClip.GetData(samples, i);

            // Calculate RMS for the current sample window
            float sum = 0f;
            for (int j = 0; j < sampleSize; j++)
            {
                sum += samples[j] * samples[j];
            }
            float rms = Mathf.Sqrt(sum / sampleSize);

            // Calculate the time for this keyframe
            float time = (float)i / sampleRate;

            // Add the RMS value to the pre-calculated curve as a keyframe
            preCalculatedIntensityCurve.AddKey(new Keyframe(time, rms));
        }
    }

    void Update()
    {
        if (preCalculatedIntensityCurve != null && audioSource.isPlaying)
        {
            // Use the pre-calculated intensity curve for animation during playback
            float currentTime = audioSource.time;
            float scale = preCalculatedIntensityCurve.Evaluate(currentTime);
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}