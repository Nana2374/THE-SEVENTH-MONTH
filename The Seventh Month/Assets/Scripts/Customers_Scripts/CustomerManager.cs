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
    [Header("ARG Spawn System")]
    public GameObject[] argPrefabs;          // Prefabs to spawn
    public Transform[] argSpawnPoints;       // Locations to spawn ARGs
    //private int successfulCount = 0;         // Tracks total successful customers

    private bool dayHadFailures = false;

    public AudioClip argSpawnSound;   // the sound that plays when an ARG spawns

    private List<CustomerCasePair> failedTodayTemp = new List<CustomerCasePair>();

    private List<(int prefabIndex, int spawnIndex)> spawnedARGs = new List<(int, int)>();

    private Dictionary<CustomerData, int> lastSuccessDay = new Dictionary<CustomerData, int>();


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

    private List<CustomerCasePair> failedCustomersQueue = new List<CustomerCasePair>();

    private Dictionary<CustomerData, CustomerCase> activeAssignedCases = new Dictionary<CustomerData, CustomerCase>();

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
        LoadARGProgress();  // loads ARGs from previous session

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
            Debug.Log("[AutoSave] No saved day found. Starting at Day 1.");
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

        dayHadFailures = false;

        // >>> Add THIS block <<<
        if (failedTodayTemp.Count > 0)
        {
            failedCustomersQueue.AddRange(failedTodayTemp);
            failedTodayTemp.Clear();
        }
        // >>> END <<<

        FolderManager folder = FindObjectOfType<FolderManager>();
        if (folder != null)
        {
            folder.InitializeFolder(currentDay); // use saved/current day
        }

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

        FolderManager folder = FindObjectOfType<FolderManager>();
        if (folder == null)
            return;

        foreach (var customer in customers)
        {
            // Skip dead
            if (failureCounts.ContainsKey(customer) && failureCounts[customer] >= 2)
                continue;

            // Skip failed today
            if (lastFailureDay.ContainsKey(customer) && lastFailureDay[customer] == currentDay)
                continue;

            // Skip customers that were already successfully served on earlier days
            if (lastSuccessDay.ContainsKey(customer) && lastSuccessDay[customer] < currentDay)
                continue;

            foreach (var custCase in customer.possibleCases)
            {
                if (folder.IsCaseUnlocked(custCase))
                {
                    availablePairs.Add(new CustomerCasePair(customer, custCase));
                }
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


        if (activePair.customer != null && dialogueManager != null)
        {
            bool hasFailedBefore = failureCounts.ContainsKey(customer) && failureCounts[customer] > 0;

            string dialogueLine = (hasFailedBefore && !string.IsNullOrEmpty(activePair.customer.failureDialogue))
                                  ? activePair.customer.failureDialogue
                                  : activePair.customer.caseDescription;

            // Use CustomerData for correct voice gender
            dialogueManager.ShowDialogue(activePair.customer, dialogueLine);
        }

    }


    private CustomerCasePair PickCustomerCasePair()
    {
        // Step 0: Return failed customers first
        if (failedCustomersQueue.Count > 0)
        {
            CustomerCasePair nextFailed = failedCustomersQueue[0];
            failedCustomersQueue.RemoveAt(0);

            // Ensure the failed customer is still available (not dead)
            if (failureCounts.ContainsKey(nextFailed.customer) && failureCounts[nextFailed.customer] >= 2)
                return PickCustomerCasePair(); // skip if doomed

            // Remove this pair from availablePairs so it doesn’t spawn again
            availablePairs.RemoveAll(p => p.customer == nextFailed.customer && p.customerCase == nextFailed.customerCase);

            // Remember assigned case
            activeAssignedCases[nextFailed.customer] = nextFailed.customerCase;

            return nextFailed;
        }

        // Step 1: Return customers who already have assigned cases (continuing cases)
        foreach (var kvp in activeAssignedCases)
        {
            CustomerData c = kvp.Key;
            CustomerCase cc = kvp.Value;

            if (failureCounts.ContainsKey(c) && failureCounts[c] >= 2) continue; // dead
            if (lastFailureDay.ContainsKey(c) && lastFailureDay[c] == currentDay) continue; // failed today

            if (availablePairs.Exists(p => p.customer == c && p.customerCase == cc))
            {
                availablePairs.RemoveAll(p => p.customer == c); // remove other cases
                return new CustomerCasePair(c, cc);
            }
        }

        // Step 2: Pick a new random customer
        int remaining = maxCustomers - customersServed;
        bool mustSpawnStalker = (stalkerSpawned == 0 && remaining == 1);

        CustomerCasePair pair = null;

        if (mustSpawnStalker)
        {
            var stalkers = availablePairs.FindAll(p => p.customerCase.caseType == CaseType.Stalker);
            if (stalkers.Count > 0) pair = stalkers[Random.Range(0, stalkers.Count)];
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

        // Step 3: Remember assigned case
        if (pair != null && !activeAssignedCases.ContainsKey(pair.customer))
            activeAssignedCases[pair.customer] = pair.customerCase;

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
            dialogueManager.ShowDialogue(activePair.customer, "Thanks, I'll try it out!");


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

        // <<< INSERT THIS BLOCK HERE >>>
        if (activePair != null)
        {
            // Remove successfully served customer so they won't spawn again
            availablePairs.RemoveAll(p => p.customer == activePair.customer);
            activeAssignedCases.Remove(activePair.customer);


            // Mark customer as successfully served today
            if (!lastSuccessDay.ContainsKey(activePair.customer))
                lastSuccessDay.Add(activePair.customer, currentDay);
            else
                lastSuccessDay[activePair.customer] = currentDay;
        }

        //// Count as a successful customer (no failures)
        //successfulCount++;

        //if (successfulCount % 5 == 0)
        //{
        //    SpawnARGObject();
        //}

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

        // Only spawn ARGs if the day had no failures
        if (!dayHadFailures)
        {
            Debug.Log("[ARG] No failures today, spawning ARGs...");
            SpawnARGObject();
        }
        else
        {
            Debug.Log("[ARG] Failures occurred today, no ARGs spawned.");
        }

        if (currentDay >= maxDays)
        {
            Debug.Log("Game Over: all days completed!");
            if (finalSummaryUI != null)
                finalSummaryUI.ShowSummary(customersDoomed);

            StopAllCoroutines();
            if (activeCustomer != null)
                Destroy(activeCustomer);

            return;
        }

        currentDay++;
        SaveProgress();
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
        dayHadFailures = true;

        if (!failureCounts.ContainsKey(failedCustomer))
            failureCounts[failedCustomer] = 0;

        failureCounts[failedCustomer]++;
        int failures = failureCounts[failedCustomer];

        if (failures == 1)
        {
            lastFailureDay[failedCustomer] = currentDay;
            Debug.Log($"[CustomerManager] {failedCustomer.customerName} failed once, will return next day.");

            // Store for tomorrow
            foreach (var custCase in failedCustomer.possibleCases)
            {
                failedTodayTemp.Add(new CustomerCasePair(failedCustomer, custCase));
            }
        }
        else if (failures >= 2)
        {
            customersDoomed++;
            Debug.Log($"[CustomerManager] {failedCustomer.customerName} has died after 2 failures.");

            if (failurePosterManager != null && failedCustomer.failurePoster != null)
                failurePosterManager.QueuePoster(failedCustomer.failurePoster);

            availablePairs.RemoveAll(pair => pair.customer == failedCustomer);

            // Remove from active case memory since they’re gone
            if (activeAssignedCases.ContainsKey(failedCustomer))
                activeAssignedCases.Remove(failedCustomer);
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

    private int GetTotalFailures()
    {
        int total = 0;
        foreach (var kvp in failureCounts)
            total += kvp.Value; // sum of all failures (1+ or 2+)
        return total;
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

    private void SpawnARGObject()
    {
        int index = spawnedARGs.Count; // next ARG to unlock

        // Safety check
        if (index >= argPrefabs.Length || index >= argSpawnPoints.Length)
        {
            Debug.LogWarning("[ARG] No more ARGs to spawn.");
            return;
        }

        // Spawn ARG i at spawnpoint i
        GameObject spawnedARG = Instantiate(
            argPrefabs[index],
            argSpawnPoints[index].position,
            argSpawnPoints[index].rotation
        );

        if (audioSource != null && argSpawnSound != null)
            audioSource.PlayOneShot(argSpawnSound);

        Debug.Log($"[ARG] Spawned ARG index {index} at spawn point {index}");

        // Save this ARG as unlocked
        spawnedARGs.Add((index, index));
        SaveARGProgress();

    }
    // SAVE ARG DATA
    // -----------------------------------------
    private void SaveARGProgress()
    {
        PlayerPrefs.SetInt("ARG_Count", spawnedARGs.Count);

        for (int i = 0; i < spawnedARGs.Count; i++)
        {
            PlayerPrefs.SetInt($"ARG_Prefab_{i}", spawnedARGs[i].prefabIndex);
            PlayerPrefs.SetInt($"ARG_Spawn_{i}", spawnedARGs[i].spawnIndex);
        }

        PlayerPrefs.Save();
        Debug.Log("[ARG] Saved ARG progress");
    }

    // -----------------------------------------
    // LOAD ARG DATA
    // -----------------------------------------
    private void LoadARGProgress()
    {
        spawnedARGs.Clear();

        int count = PlayerPrefs.GetInt("ARG_Count", 0);

        for (int i = 0; i < count; i++)
        {
            int index = i; // always match array index

            spawnedARGs.Add((index, index));

            Instantiate(
                argPrefabs[index],
                argSpawnPoints[index].position,
                argSpawnPoints[index].rotation
            );

            Debug.Log($"[ARG] Restored ARG index {index}");
        }
    }
}
