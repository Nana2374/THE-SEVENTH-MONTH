using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Force169Aspect : MonoBehaviour
{
    private Camera cam;
    private float targetAspect = 16f / 9f;

    void Awake()
    {
        cam = GetComponent<Camera>();
        UpdateViewport();
    }

    void Update()
    {
        // Recalculate if window size changes
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            UpdateViewport();
        }
    }

    int lastWidth = 0;
    int lastHeight = 0;

    void UpdateViewport()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;

        float windowAspect = (float)Screen.width / Screen.height;
        float scale = windowAspect / targetAspect;

        if (scale < 1f)
        {
            // Letterbox (bars top/bottom)
            cam.rect = new Rect(
                0,
                (1f - scale) / 2f,
                1f,
                scale
            );
        }
        else
        {
            // Pillarbox (bars left/right)
            float scaleWidth = 1f / scale;
            cam.rect = new Rect(
                (1f - scaleWidth) / 2f,
                0,
                scaleWidth,
                1f
            );
        }
    }
}
