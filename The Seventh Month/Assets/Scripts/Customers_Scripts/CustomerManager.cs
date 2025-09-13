using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CustomerCase;

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
    public TMPro.TextMeshProUGUI dayText; // in inspector


    public DialogueManager dialogueManager;

    private GameObject activeCustomer;
    private CustomerCase activeCase;

    private List<CustomerCasePair> availablePairs = new List<CustomerCasePair>();
    private Dictionary<CustomerData, int> failureCounts = new Dictionary<CustomerData, int>();
    private Dictionary<CustomerData, int> lastFailureDay = new Dictionary<CustomerData, int>();


    private int customersServed = 0;
    public int maxCustomers = 5;

    private int stalkerSpawned = 0;
    public float bufferTime = 2f;

    private int currentDay = 1;
    private int maxDays = 4;

    void Start()
    {
        StartDay();
    }

    private void StartDay()
    {
        Debug.Log($"--- DAY {currentDay} START ---");

        customersServed = 0;
        stalkerSpawned = 0;

        FillAvailablePairs();

        if (clockManager != null)
            clockManager.ResetClock();

        // Update day text
        if (dayText != null)
            dayText.text = $"Day {currentDay}";

        // Optional: animate day text
        //if (dayText != null)
        //    LeanTween.scale(dayText.gameObject, Vector3.one * 1.5f, 0.3f).setEasePunch().setOnComplete(() =>
        //        LeanTween.scale(dayText.gameObject, Vector3.one, 0.2f));

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


            foreach (var c in customer.possibleCases)
            {
                availablePairs.Add(new CustomerCasePair(customer, c));
            }
        }
    }


    public void SpawnRandomCustomer()
    {
        if (customersServed >= maxCustomers)
        {
            Debug.Log("All customers served for today.");
            return;
        }

        if (availablePairs.Count == 0)
        {
            Debug.Log("No available customers left to spawn.");
            return;
        }

        CustomerCasePair pairToSpawn = PickCustomerCasePair();

        availablePairs.Remove(pairToSpawn);
        activeCase = pairToSpawn.customerCase;

        if (activeCustomer != null) Destroy(activeCustomer);
        activeCustomer = Instantiate(pairToSpawn.customer.customerPrefab, spawnPoint.position, spawnPoint.rotation);

        if (activeCase.caseType == CaseType.Stalker)
            stalkerSpawned++;

        Debug.Log($"Customer spawned. Case: {activeCase.caseName} ({activeCase.caseType})");

        PhotoEvidence[] randomPhotos = GetRandomPhotos(activeCase.evidencePhotos, 3);

        if (photoPanelManager != null)
        {
            photoPanelManager.ShowEvidencePhotos(randomPhotos);
            StartCoroutine(ShowThumbnailNextFrame());
        }

        if (activeCase != null && dialogueManager != null)
            dialogueManager.ShowDialogue(activeCase.description);
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

        if (pair == null)
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
        // Start coroutine to keep customer for 2 seconds before leaving
        StartCoroutine(CustomerLeaveAfterDelay(3f));
    }

    private IEnumerator CustomerLeaveAfterDelay(float delay)
    {
        // Show "thanks" dialogue
        if (dialogueManager != null)
        {
            dialogueManager.ShowDialogue("Thanks, I'll try it out!");
        }

        // Wait for 2 seconds
        yield return new WaitForSeconds(delay);

        // Hide dialogue
        if (dialogueManager != null)
            dialogueManager.HideDialogue();

        // Destroy the active customer
        if (activeCustomer != null)
        {
            Destroy(activeCustomer);
            activeCustomer = null;
        }

        // Advance hour and check for end of day
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

        // Hide photo thumbnail
        if (photoPanelManager != null)
            photoPanelManager.HideThumbnail();

        // Spawn next customer if any remain
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

        currentDay++;
        if (currentDay > maxDays)
        {
            Debug.Log("Game Over: all days completed!");
            return;
        }

        StartDay();
    }

    public void RegisterFailure(CustomerData failedCustomer, Sprite failurePoster = null)
    {
        if (!failureCounts.ContainsKey(failedCustomer))
            failureCounts[failedCustomer] = 0;

        failureCounts[failedCustomer]++;

        int failures = failureCounts[failedCustomer];

        if (failures == 1)
        {
            Debug.Log($"[CustomerManager] {failedCustomer.customerName} failed once, will return next day.");

            // Record the day they failed
            lastFailureDay[failedCustomer] = currentDay;

            //  They will return tomorrow, do not remove from pool
        }
        else if (failures >= 2)
        {
            Debug.Log($"[CustomerManager] {failedCustomer.customerName} has died after 2 failures.");

            // Spawn failure poster
            if (failurePosterManager != null && failurePoster != null)
            {
                Debug.Log("[CustomerManager] Queuing poster for dead customer.");
                failurePosterManager.QueuePoster(failurePoster);
            }

            // Remove from pool permanently
            Debug.Log($"[CustomerManager] {failedCustomer.customerName} has been removed from list.");
            availablePairs.RemoveAll(pair => pair.customer == failedCustomer);
        }
    }

    public CustomerData GetActiveCustomerData()
    {
        // Loop through all customers
        foreach (var customer in customers)
        {
            // Check if this customer has the currently active case
            foreach (var c in customer.possibleCases)
            {
                if (c == activeCase)
                    return customer;
            }
        }

        Debug.LogWarning("[CustomerManager] GetActiveCustomerData: No matching customer found for active case.");
        return null;
    }


    public CustomerCase GetActiveCase()
    {
        return activeCase;
    }

    private PhotoEvidence[] GetRandomPhotos(PhotoEvidence[] pool, int count)
    {
        List<PhotoEvidence> shuffled = new List<PhotoEvidence>(pool);
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
}
