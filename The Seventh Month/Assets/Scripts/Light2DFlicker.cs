using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light2DFlicker : MonoBehaviour
{
    [Header("Main Light")]
    public Light2D light2D;

    [Header("Global Light (optional)")]
    public Light2D globalLight2D;

    [Header("Normal Flicker Settings")]
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float normalIntervalMin = 0.05f;
    public float normalIntervalMax = 0.2f;

    [Header("Failure Flicker Settings")]
    public float failMinIntensity = 0f;
    public float failMaxIntensity = 2f;
    public float failIntervalMin = 0.01f;
    public float failIntervalMax = 0.05f;
    public float failDuration = 1f;

    [Header("Global Light Dim Settings")]
    public float globalNormalIntensity = 1f;
    public float globalFailMinIntensity = 0.4f;
    public float globalFailMaxIntensity = 0.8f;

    [Header("Transition Speed")]
    public float flickerSmoothness = 20f;

    private float targetIntensity;
    private float timer;
    private bool isFailFlicker = false;
    private float failTimer = 0f;

    void Start()
    {
        if (light2D == null)
            light2D = GetComponent<Light2D>();

        targetIntensity = light2D.intensity;
        ResetNormalTimer();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (isFailFlicker)
        {
            failTimer -= Time.deltaTime;

            if (timer <= 0f)
            {
                // Violent random flicker for main light
                targetIntensity = Random.Range(failMinIntensity, failMaxIntensity);
                ResetFailTimer();
            }

            // Violent random dimming for global light (if assigned)
            if (globalLight2D != null)
            {
                float globalTarget = Random.Range(globalFailMinIntensity, globalFailMaxIntensity);
                globalLight2D.intensity = Mathf.Lerp(globalLight2D.intensity, globalTarget, Time.deltaTime * flickerSmoothness / 2f);
            }

            if (failTimer <= 0f)
                isFailFlicker = false; // return to normal flicker
        }
        else
        {
            if (timer <= 0f)
            {
                // Subtle random flicker
                targetIntensity = Random.Range(minIntensity, maxIntensity);
                ResetNormalTimer();
            }

            // Normal global light intensity
            if (globalLight2D != null)
                globalLight2D.intensity = Mathf.Lerp(globalLight2D.intensity, globalNormalIntensity, Time.deltaTime * flickerSmoothness);
        }

        // Smoothly interpolate main light intensity
        light2D.intensity = Mathf.Lerp(light2D.intensity, targetIntensity, Time.deltaTime * flickerSmoothness);
    }

    /// <summary>
    /// Trigger violent flicker (call on customer failure)
    /// </summary>
    public void TriggerFailFlicker()
    {
        isFailFlicker = true;
        failTimer = failDuration;
        ResetFailTimer();
    }

    private void ResetNormalTimer()
    {
        timer = Random.Range(normalIntervalMin, normalIntervalMax);
    }

    private void ResetFailTimer()
    {
        timer = Random.Range(failIntervalMin, failIntervalMax);
    }
}
