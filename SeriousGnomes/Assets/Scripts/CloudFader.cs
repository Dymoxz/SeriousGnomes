using UnityEngine;
using System.Collections;

public class CloudFader : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("How long it takes the cloud to fade out/in (in seconds)")]
    public float fadeDuration = 1.0f;

    private Renderer cloudRenderer;
    private Material cloudMaterial;
    private Coroutine currentFade;

    void Start()
    {
        cloudRenderer = GetComponent<Renderer>();

        // We use .material instead of .sharedMaterial so each cloud 
        // gets its own unique material instance. Otherwise, fading 
        // one cloud would fade EVERY cloud at the same time!
        cloudMaterial = cloudRenderer.material;
    }

    // Triggered when the cloud ENTERS the BoardZone
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CloudBlocker"))
        {
            if (currentFade != null) StopCoroutine(currentFade);
            currentFade = StartCoroutine(FadeTo(0f)); // Fade to 0 alpha (invisible)
        }
    }

    // Triggered when the cloud EXITS the BoardZone
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CloudBlocker"))
        {
            if (currentFade != null) StopCoroutine(currentFade);
            currentFade = StartCoroutine(FadeTo(0.50f)); // Fade back to 0.6 alpha (semi-transparent)
        }
    }

    // The Coroutine that smoothly animates the transparency
    IEnumerator FadeTo(float targetAlpha)
    {
        Color color = cloudMaterial.color;
        float startAlpha = color.a;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;

            // Lerp smoothly calculates the value between start and target
            color.a = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / fadeDuration);
            cloudMaterial.color = color;

            yield return null; // Wait for the next frame
        }

        // Ensure we hit the exact target alpha at the end
        color.a = targetAlpha;
        cloudMaterial.color = color;
    }
}