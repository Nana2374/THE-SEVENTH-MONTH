using UnityEngine;

public class CustomerAppearance : MonoBehaviour
{
    public enum Gender { Male, Female }

    [Header("SpriteRenderers")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer hairBackRenderer;
    public SpriteRenderer hairFrontRenderer;
    public SpriteRenderer clothesRenderer;
    public SpriteRenderer eyesRenderer;
    public SpriteRenderer noseRenderer;
    public SpriteRenderer lipsRenderer;
    public SpriteRenderer glassesRenderer;

    [Header("Female Sprites")]
    public Sprite[] femaleBodies;
    public Sprite[] femaleHairBack;
    public Sprite[] femaleHairFront;
    public Sprite[] femaleClothes;
    public Sprite[] femaleEyes;
    public Sprite[] femaleNoses;
    public Sprite[] femaleLips;

    [Header("Male Sprites")]
    public Sprite[] maleBodies;
    public Sprite[] maleHairBack;
    public Sprite[] maleHairFront;
    public Sprite[] maleClothes;
    public Sprite[] maleEyes;
    public Sprite[] maleNoses;
    public Sprite[] maleLips;

    [Header("Shared Sprites")]
    public Sprite[] glassesOptions;

    [Header("Settings")]
    [Range(0f, 1f)] public float glassesChance = 0.3f;

    private CustomerAppearanceData currentData;


    // Call this to randomize and apply a new look.

    public void RandomizeAppearance()
    {
        currentData = new CustomerAppearanceData();

        // Pick gender
        currentData.gender = (Random.value < 0.5f) ? Gender.Male : Gender.Female;

        // Pick feature indices
        if (currentData.gender == Gender.Female)
            PickSprites(currentData, femaleBodies, femaleHairBack, femaleHairFront, femaleClothes, femaleEyes, femaleNoses, femaleLips);
        else
            PickSprites(currentData, maleBodies, maleHairBack, maleHairFront, maleClothes, maleEyes, maleNoses, maleLips);

        // Glasses
        if (glassesOptions.Length > 0 && Random.value < glassesChance)
            currentData.glassesIndex = Random.Range(0, glassesOptions.Length);
        else
            currentData.glassesIndex = -1;

        ApplyAppearance(currentData);
    }


    // Applies a previously saved appearance.

    public void ApplyAppearance(CustomerAppearanceData data)
    {
        currentData = data;

        if (data.gender == Gender.Female)
            ApplySprites(data, femaleBodies, femaleHairBack, femaleHairFront, femaleClothes, femaleEyes, femaleNoses, femaleLips);
        else
            ApplySprites(data, maleBodies, maleHairBack, maleHairFront, maleClothes, maleEyes, maleNoses, maleLips);

        // Glasses
        if (data.glassesIndex >= 0 && data.glassesIndex < glassesOptions.Length)
        {
            glassesRenderer.enabled = true;
            glassesRenderer.sprite = glassesOptions[data.glassesIndex];
        }
        else
        {
            glassesRenderer.enabled = false;
        }

        // Skin
        bodyRenderer.color = CustomerColors.GetRandomSkin();

        // Hair
        hairBackRenderer.color = CustomerColors.GetRandomHair();
        hairFrontRenderer.color = CustomerColors.GetRandomHair();

        // Lips 
        lipsRenderer.color = CustomerColors.GetRandomLips();

        // Eyes remain white
        eyesRenderer.color = Color.white;

        // Clothes
        clothesRenderer.color = CustomerColors.GetRandomClothes();


    }

    private void PickSprites(CustomerAppearanceData data, Sprite[] body, Sprite[] hairBack, Sprite[] hairFront, Sprite[] clothes, Sprite[] eyes, Sprite[] noses, Sprite[] lips)
    {
        data.bodyIndex = Random.Range(0, body.Length);
        data.hairBackIndex = Random.Range(0, hairBack.Length);
        data.hairFrontIndex = Random.Range(0, hairFront.Length);
        data.clothesIndex = Random.Range(0, clothes.Length);
        data.eyesIndex = Random.Range(0, eyes.Length);
        data.noseIndex = Random.Range(0, noses.Length);
        data.lipsIndex = Random.Range(0, lips.Length);
    }

    private void ApplySprites(CustomerAppearanceData data, Sprite[] body, Sprite[] hairBack, Sprite[] hairFront, Sprite[] clothes, Sprite[] eyes, Sprite[] noses, Sprite[] lips)
    {
        if (body.Length > 0) bodyRenderer.sprite = body[Mathf.Clamp(data.bodyIndex, 0, body.Length - 1)];
        if (hairBack.Length > 0) hairBackRenderer.sprite = hairBack[Mathf.Clamp(data.hairBackIndex, 0, hairBack.Length - 1)];
        if (hairFront.Length > 0) hairFrontRenderer.sprite = hairFront[Mathf.Clamp(data.hairFrontIndex, 0, hairFront.Length - 1)];
        if (clothes.Length > 0) clothesRenderer.sprite = clothes[Mathf.Clamp(data.clothesIndex, 0, clothes.Length - 1)];
        if (eyes.Length > 0) eyesRenderer.sprite = eyes[Mathf.Clamp(data.eyesIndex, 0, eyes.Length - 1)];
        if (noses.Length > 0) noseRenderer.sprite = noses[Mathf.Clamp(data.noseIndex, 0, noses.Length - 1)];
        if (lips.Length > 0) lipsRenderer.sprite = lips[Mathf.Clamp(data.lipsIndex, 0, lips.Length - 1)];
    }


    public CustomerAppearanceData GetAppearanceData()
    {
        return currentData;
    }
}
