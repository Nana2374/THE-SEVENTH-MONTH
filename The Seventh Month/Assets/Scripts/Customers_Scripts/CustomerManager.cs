using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CustomerCase;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class CustomerCasePair
{
    public CustomerData customer;
    public CustomerCase customerCase;

    public CustomerCasePair(CustomerData c, CustomerCase cc)
    {
        customer = c;
        customerCase = cc;
    }
}

public class CustomerManager : MonoBehaviour
{
    public CustomerData[] customers;
    public Transform spawnPoint;
    public PhotoPanelManager photoPanelManager;
    public FailurePosterManager failurePosterManager;

    public ClockManager clockManager;
    public TMPro.TextMeshProUGUI dayText;
    public DialogueManager dialogueManager;
    public AudioSource audioSource;

    public TextMeshProUGUI nameText;
    private GameObject activeCustomer;
    private CustomerCasePair activePair; // store active customer + case

    private List<CustomerCasePair> availablePairs = new List<CustomerCasePair>();
    private Dictionary<CustomerData, int> failureCounts = new Dictionary<CustomerData, int>();
    private Dictionary<CustomerData, int> lastFailureDay = new Dictionary<CustomerData, int>();

    private int customersServed = 0;
    public int maxCustomers = 5;

    private int stalkerSpawned = 0;
    public float bufferTime = 2f;

    private int currentDay = 1;
    private int maxDays = 4;

    public FinalSummaryUI finalSummaryUI;
    private int customersDoomed = 0;

    void Start()
    {
        LoadProgress();
        StartDay();
    }

    // --- AUTOSAVE SYSTEM ---
    private void SaveProgress()
    {
        PlayerPrefs.SetInt("SavedDay", currentDay);

        // Save failure counts
        foreach (var kvp in failureCounts)
        {
            PlayerPrefs.SetInt($"FailureCount_{kvp.Key.customerName}", kvp.Value);
        }

        // Save last failure days
        foreach (var kvp in lastFailureDay)
        {
            PlayerPrefs.SetInt($"LastFailureDay_{kvp.Key.customerName}", kvp.Value);
        }

        PlayerPrefs.Save();
        Debug.Log($"[AutoSave] Progress saved for Day {currentDay} with {failureCounts.Count} failure entries");
    }

    private void LoadProgress()
    {
        if (PlayerPrefs.HasKey("SavedDay"))
        {
            currentDay = PlayerPrefs.GetInt("SavedDay");
            Debug.Log($"[AutoSave] Loaded saved day: {currentDay}");
        }
        else
        {
            currentDay = 1;
        }

        failureCounts.Clear();
        lastFailureDay.Clear();

        foreach (var customer in customers)
        {
            string name = customer.customerName;

            if (PlayerPrefs.HasKey($"FailureCount_{name}"))
                failureCounts[customer] = PlayerPrefs.GetInt($"FailureCount_{name}");

            if (PlayerPrefs.HasKey($"LastFailureDay_{name}"))
                lastFailureDay[customer] = PlayerPrefs.GetInt($"LastFailureDay_{name}");
        }

        Debug.Log($"[AutoSave] Loaded {failureCounts.Count} failure records");
    }

    private void ResetProgress()
    {
        PlayerPrefs.DeleteKey("SavedDay");
        Debug.Log("[AutoSave] Progress reset");
    }

    private void StartDay()
    {
        Debug.Log($"--- DAY {currentDay} START ---");

        customersServed = 0;
        stalkerSpawned = 0;

        FillAvailablePairs();

        if (clockManager != null)
            clockManager.ResetClock();

        if (dayText != null)
            dayText.text = $"Day {currentDay}";

        SpawnRandomCustomer();
    }

    private void FillAvailablePairs()
    {
        availablePairs.Clear();

        foreach (var customer in customers)
        {
            // Skip dead customers
            if (failureCounts.ContainsKey(customer) && failureCounts[customer] >= 2)
                continue;

            // Skip first-failure customers until the next day
            if (lastFailureDay.ContainsKey(customer) && lastFailureDay[customer] == currentDay)
                continue;

            // Pick ONE random case for this customer
            if (customer.possibleCases.Length > 0)
            {
                CustomerCase randomCase = customer.possibleCases[Random.Range(0, customer.possibleCases.Length)];
                availablePairs.Add(new CustomerCasePair(customer, randomCase));
            }
        }
    }

    public void SpawnRandomCustomer()
    {
        if (customersServed >= maxCustomers || availablePairs.Count == 0)
            return;

        CustomerCasePair pairToSpawn = PickCustomerCasePair();
        availablePairs.Remove(pairToSpawn);

        activePair = pairToSpawn;
        CustomerData customer = pairToSpawn.customer;
        StartCoroutine(SpawnCustomerAfterArrivalSound(customer));
    }

    private IEnumerator SpawnCustomerAfterArrivalSound(CustomerData customer)
    {
        if (customer.arrivalClip != null && audioSource != null)
        {
            audioSource.clip = customer.arrivalClip;
            audioSource.Play();
            yield return new WaitForSeconds(customer.arrivalClip.length);
        }

        if (activeCustomer != null) Destroy(activeCustomer);
        activeCustomer = new GameObject(customer.customerName);
        activeCustomer.transform.position = spawnPoint.position;
        activeCustomer.transform.rotation = spawnPoint.rotation;

        SpriteRenderer sr = activeCustomer.AddComponent<SpriteRenderer>();
        sr.sprite = (failureCounts.ContainsKey(customer) && failureCounts[customer] > 0)
            ? customer.failureSprite
            : customer.customerSprite;

        sr.sortingLayerName = "Characters";

        if (nameText != null)
        {
            nameText.text = customer.customerName;
        }

        Debug.Log($"[CustomerManager] Customer Spawned: {customer.customerName}");

        // Show photos and dialogue
        EvidencePhoto[] randomPhotos = GetRandomPhotos(activePair.customerCase.evidencePhotos, 3); // updated type
        if (photoPanelManager != null)
        {
            // Pass currentDay to the panel
            photoPanelManager.currentDay = currentDay;
            photoPanelManager.ShowEvidencePhotos(randomPhotos);
            StartCoroutine(ShowThumbnailNextFrame());
        }


        if (activePair.customerCase != null && dialogueManager != null)
        {
            bool hasFailedBefore = failureCounts.ContainsKey(customer) && failureCounts[customer] > 0;

            string dialogueLine = (hasFailedBefore && !string.IsNullOrEmpty(activePair.customer.failureDialogue))
                                  ? activePair.customer.failureDialogue
                                  : activePair.customer.caseDescription;

            dialogueManager.ShowDialogue(activePair.customerCase, dialogueLine);
        }

    }


    private CustomerCasePair PickCustomerCasePair()
    {
        int remaining = maxCustomers - customersServed;
        bool mustSpawnStalker = (stalkerSpawned == 0 && remaining == 1);

        CustomerCasePair pair = null;

        if (mustSpawnStalker)
        {
            var stalkers = availablePairs.FindAll(p => p.customerCase.caseType == CaseType.Stalker);
            if (stalkers.Count > 0)
                pair = stalkers[Random.Range(0, stalkers.Count)];
        }
        else
        {
            var ghosts = availablePairs.FindAll(p => p.customerCase.caseType == CaseType.Ghost);
            var stalkers = availablePairs.FindAll(p => p.customerCase.caseType == CaseType.Stalker);

            float stalkerWeight = 1f + (currentDay - 1) * 0.5f;
            float totalWeight = ghosts.Count * 4 + stalkers.Count * stalkerWeight;

            float r = Random.Range(0f, totalWeight);

            if (r < ghosts.Count * 4 && ghosts.Count > 0)
                pair = ghosts[Random.Range(0, ghosts.Count)];
            else if (stalkers.Count > 0)
                pair = stalkers[Random.Range(0, stalkers.Count)];
        }

        if (pair == null && availablePairs.Count > 0)
            pair = availablePairs[Random.Range(0, availablePairs.Count)];

        return pair;
    }

    private IEnumerator ShowThumbnailNextFrame()
    {
        yield return null;
        photoPanelManager.ShowThumbnail();
    }

    public void CustomerDone(float bufferTime)
    {
        StartCoroutine(CustomerLeaveAfterDelay(bufferTime));
    }

    private IEnumerator CustomerLeaveAfterDelay(float delay)
    {
        if (dialogueManager != null)
            dialogueManager.ShowDialogue(activePair.customerCase, "Thanks, I'll try it out!");


        yield return new WaitForSeconds(delay);

        if (dialogueManager != null)
            dialogueManager.HideDialogue();

        if (activeCustomer != null)
        {
            if (audioSource != null && activePair != null && activePair.customer.departureClip != null)
                audioSource.PlayOneShot(activePair.customer.departureClip);

            Destroy(activeCustomer);
            activeCustomer = null;
        }

        customersServed++;
        if (clockManager != null)
        {
            clockManager.AdvanceHour();
            if (customersServed >= maxCustomers)
            {
                EndDay();
                yield break;
            }
        }

        if (photoPanelManager != null)
            photoPanelManager.HideThumbnail();

        if (customersServed < maxCustomers)
            StartCoroutine(SpawnNextCustomerAfterDelay(bufferTime));
    }

    private IEnumerator SpawnNextCustomerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnRandomCustomer();
    }

    private void EndDay()
    {
        Debug.Log($"--- DAY {currentDay} END ---");

        // Autosave progress here
        SaveProgress();

        if (currentDay >= maxDays)
        {
            Debug.Log("Game Over: all days completed!");
            // Show final summary UI
            if (finalSummaryUI != null)
            {
                finalSummaryUI.ShowSummary(customersDoomed); // pass any stats you want
            }

            // Optionally, disable further spawning
            StopAllCoroutines();
            if (activeCustomer != null)
                Destroy(activeCustomer);

            // Or load a GameOver scene
            // SceneManager.LoadScene("GameOverScene");


            return;

        }

        currentDay++;
        StartCoroutine(PlayDayTransition(currentDay));
    }

    private IEnumerator PlayDayTransition(int day)
    {
        DayTransitionManager transition = FindObjectOfType<DayTransitionManager>();
        if (transition != null)
            yield return StartCoroutine(transition.PlayTransition(day));

        StartDay();
    }

    public void RegisterFailure(CustomerData failedCustomer)
    {
        if (!failureCounts.ContainsKey(failedCustomer))
            failureCounts[failedCustomer] = 0;

        failureCounts[failedCustomer]++;
        int failures = failureCounts[failedCustomer];

        if (failures == 1)
        {
            lastFailureDay[failedCustomer] = currentDay;
            Debug.Log($"[CustomerManager] {failedCustomer.customerName} failed once, will return next day.");
        }
        else if (failures >= 2)
        {
            customersDoomed++; //  Track doomed customers
            Debug.Log($"[CustomerManager] {failedCustomer.customerName} has died after 2 failures.");

            // Use the failure poster from the customer data now
            if (failurePosterManager != null && failedCustomer.failurePoster != null)
                failurePosterManager.QueuePoster(failedCustomer.failurePoster);

            availablePairs.RemoveAll(pair => pair.customer == failedCustomer);
        }
    }

    public CustomerData GetActiveCustomerData()
    {
        return activePair?.customer;
    }

    public CustomerCase GetActiveCase()
    {
        return activePair?.customerCase;
    }

    private EvidencePhoto[] GetRandomPhotos(EvidencePhoto[] pool, int count)
    {
        List<EvidencePhoto> shuffled = new List<EvidencePhoto>(pool);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int randIndex = Random.Range(i, shuffled.Count);
            var temp = shuffled[i];
            shuffled[i] = shuffled[randIndex];
            shuffled[randIndex] = temp;
        }
        int finalCount = Mathf.Min(count, shuffled.Count);
        return shuffled.GetRange(0, finalCount).ToArray();
    }


    public void FullResetProgress()
    {
        PlayerPrefs.DeleteKey("SaveData"); // clear all save data
        PlayerPrefs.DeleteKey("SavedDay"); // (in case you still store this somewhere)

        PlayerPrefs.Save();
        Debug.Log("[AutoSave] Full reset: cleared all save data");

        // Reset runtime data too, in case this object isn't reloaded immediately
        currentDay = 1;
        failureCounts.Clear();
        lastFailureDay.Clear();
        availablePairs.Clear();
    }

    // For testing: skip directly to a specific day
    public void SkipToDay(int targetDay)
    {
        if (targetDay < 1 || targetDay > maxDays)
        {
            Debug.LogWarning("Target day is out of range!");
            return;
        }

        // Stop any ongoing customer coroutines
        StopAllCoroutines();

        // Destroy the current active customer if any
        if (activeCustomer != null)
        {
            Destroy(activeCustomer);
            activeCustomer = null;
        }

        // Set the day
        currentDay = targetDay;
        Debug.Log($"[CustomerManager] Skipping to Day {currentDay}");

        // Update day text
        if (dayText != null)
            dayText.text = $"Day {currentDay}";

        // Start the day immediately
        StartDay();
    }

}
