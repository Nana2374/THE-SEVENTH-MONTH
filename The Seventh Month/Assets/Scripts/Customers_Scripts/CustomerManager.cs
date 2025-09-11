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

    //public FailurePosterManager failurePosterManager;
    [SerializeField] private FailurePosterManager failurePosterManager;


    public ClockManager clockManager;
    public DialogueManager dialogueManager;

    private GameObject activeCustomer;
    private CustomerCase activeCase;
    private CustomerData activeCustomerData;  // <-- Track current customer
    private List<CustomerCasePair> availablePairs = new List<CustomerCasePair>();
    private Dictionary<CustomerData, int> failureCounts = new Dictionary<CustomerData, int>();

    private int customersServed = 0;
    public int maxCustomers = 5;
    private int stalkerSpawned = 0;
    public float bufferTime = 2f;

    private int currentDay = 1;
    private int maxDays = 4;

    void Start()
    {
        FillAvailablePairs();

        if (clockManager != null)
            clockManager.ResetClock();

        SpawnRandomCustomer();
    }

    // -----------------------
    private void FillAvailablePairs()
    {
        availablePairs.Clear();

        foreach (var customer in customers)
        {
            // Skip customers who died twice
            if (failureCounts.ContainsKey(customer) && failureCounts[customer] >= 2)
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

        CustomerCasePair pairToSpawn = null;

        // Stalker logic
        int remainingCustomers = maxCustomers - customersServed;
        bool mustSpawnStalker = (stalkerSpawned == 0 && remainingCustomers == 1);

        if (mustSpawnStalker)
        {
            List<CustomerCasePair> stalkerPairs = availablePairs.FindAll(p => p.customerCase.caseType == CaseType.Stalker);
            if (stalkerPairs.Count > 0)
            {
                int index = Random.Range(0, stalkerPairs.Count);
                pairToSpawn = stalkerPairs[index];
            }
        }
        else
        {
            List<CustomerCasePair> ghostPairs = availablePairs.FindAll(p => p.customerCase.caseType == CaseType.Ghost);
            List<CustomerCasePair> stalkerPairs = availablePairs.FindAll(p => p.customerCase.caseType == CaseType.Stalker);

            float stalkerWeight = 1f + (currentDay - 1) * 0.5f;
            float totalWeight = ghostPairs.Count * 4 + stalkerPairs.Count * stalkerWeight;
            float r = Random.Range(0f, totalWeight);

            if (r < ghostPairs.Count * 4)
            {
                int index = Random.Range(0, ghostPairs.Count);
                pairToSpawn = ghostPairs[index];
            }
            else if (stalkerPairs.Count > 0)
            {
                int index = Random.Range(0, stalkerPairs.Count);
                pairToSpawn = stalkerPairs[index];
            }
        }

        if (pairToSpawn == null)
        {
            int index = Random.Range(0, availablePairs.Count);
            pairToSpawn = availablePairs[index];
        }

        availablePairs.Remove(pairToSpawn);

        activeCustomerData = pairToSpawn.customer; // track current customer
        activeCase = pairToSpawn.customerCase;

        if (activeCustomer != null)
            Destroy(activeCustomer);

        activeCustomer = Instantiate(activeCustomerData.customerPrefab, spawnPoint.position, spawnPoint.rotation);

        if (activeCase.caseType == CaseType.Stalker)
            stalkerSpawned++;

        Debug.Log($"Customer: {activeCustomerData.customerName}, Case: {activeCase.caseName} ({activeCase.caseType})");

        PhotoEvidence[] randomPhotos = GetRandomPhotos(activeCase.evidencePhotos, 3);
        if (photoPanelManager != null)
        {
            photoPanelManager.ShowEvidencePhotos(randomPhotos);
            StartCoroutine(ShowThumbnailNextFrame());
        }

        if (activeCase != null && dialogueManager != null)
            dialogueManager.ShowDialogue(activeCase.description);
    }

    private IEnumerator ShowThumbnailNextFrame()
    {
        yield return null;
        photoPanelManager.ShowThumbnail();
    }

    public void CustomerDone(float bufferTime)
    {
        if (activeCustomer != null)
            Destroy(activeCustomer);

        activeCustomer = null;
        customersServed++;

        if (clockManager != null)
            clockManager.AdvanceHour();

        if (photoPanelManager != null)
            photoPanelManager.HideThumbnail();

        if (dialogueManager != null)
            dialogueManager.HideDialogue();

        if (customersServed < maxCustomers)
        {
            StartCoroutine(SpawnNextCustomerAfterDelay(bufferTime));
        }
        else
        {
            Debug.Log("End of day reached.");
            EndDay();
        }
    }

    private IEnumerator SpawnNextCustomerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnRandomCustomer();
    }

    private void EndDay()
    {
        if (currentDay >= maxDays)
        {
            Debug.Log("Game Over: All days completed.");
            return;
        }

        currentDay++;
        customersServed = 0;
        stalkerSpawned = 0;

        if (clockManager != null)
            clockManager.ResetClock();

        FillAvailablePairs();
        SpawnRandomCustomer();

        Debug.Log($"--- DAY {currentDay} START ---");
    }

    // ------------------------
    // Registers a failure for a customer
    public void RegisterFailure(CustomerData failedCustomer)
    {
        if (failedCustomer == null)
        {
            Debug.LogWarning("RegisterFailure called with null customer!");
            return;
        }

        if (!failureCounts.ContainsKey(failedCustomer))
            failureCounts[failedCustomer] = 0;

        failureCounts[failedCustomer]++;

        if (failureCounts[failedCustomer] >= 2)
        {
            Debug.Log($"{failedCustomer.customerName} has died.");

            if (failurePosterManager != null && failedCustomer.posterSprite != null)
                failurePosterManager.QueuePoster(failedCustomer.posterSprite);
        }
        else
        {
            Debug.Log($"{failedCustomer.customerName} failed once, will return later.");
        }
    }

    // ------------------------
    public CustomerCase GetActiveCase() => activeCase;
    public CustomerData GetActiveCustomerData() => activeCustomerData;

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
