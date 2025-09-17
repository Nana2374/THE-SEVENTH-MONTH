using UnityEngine;

public static class CustomerColors
{
    // Skin tones (beige -> brown)
    public static Color GetRandomSkin()
    {
        return new Color(
            Random.Range(0.85f, 0.95f), // R
            Random.Range(0.65f, 0.8f),  // G
            Random.Range(0.55f, 0.7f)   // B
        );
    }

    // Hair tones (black -> brown)
    public static Color GetRandomHair()
    {
        return new Color(
            Random.Range(0.05f, 0.3f),  // R
            Random.Range(0.05f, 0.2f),  // G
            Random.Range(0.05f, 0.1f)   // B
        );
    }

    // Lips (pinks)
    public static Color GetRandomLips()
    {
        return new Color(
            Random.Range(0.9f, 1f),     // R
            Random.Range(0.4f, 0.7f),   // G
            Random.Range(0.5f, 0.8f)    // B
        );
    }

    // Clothes (any except skin tones)
    public static Color GetRandomClothes()
    {
        return new Color(
            Random.value,
            Random.value,
            Random.value
        );
    }
}
