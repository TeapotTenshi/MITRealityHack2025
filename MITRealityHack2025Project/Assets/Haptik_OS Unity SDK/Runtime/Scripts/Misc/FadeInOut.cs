using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    public float fadeDuration = 0.5f; // Time for one fade in or out
    public Renderer objectRenderer;  // Assign the object's Renderer in the Inspector

    private Material objectMaterial;
    private Color originalColor;
    private bool isFadingIn = true;
    private float fadeTimer = 0f;

    private float minAlpha = 0.1f;
    private float maxAlpha = 0.4f;

    void Start()
    {
        if (objectRenderer == null)
        {
            objectRenderer = GetComponent<Renderer>();
        }

        // Get the material and its original color
        objectMaterial = objectRenderer.material;
        originalColor = objectMaterial.color;
    }

    void Update()
    {
        // Update the fade timer
        fadeTimer += Time.deltaTime;

        float t = fadeTimer / fadeDuration;

        // Calculate the alpha value based on fade duration
        float alpha = isFadingIn ? Mathf.Lerp(minAlpha,maxAlpha,t) : Mathf.Lerp(maxAlpha, minAlpha, t);

        // Set the material's color with the updated alpha
        Color newColor = originalColor;
        newColor.a = alpha;
        objectMaterial.color = newColor;

        // Switch fade direction if a cycle is completed
        if (fadeTimer >= fadeDuration)
        {
            fadeTimer = 0f;
            isFadingIn = !isFadingIn;
        }
    }
}