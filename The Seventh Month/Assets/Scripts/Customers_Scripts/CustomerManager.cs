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


    public ClockManager clockManager; // Assign in inspector

    private GameObject activeCustomer;
    private CustomerCase activeCase;
    public DialogueManager dialogueManager;

    private List<CustomerCasePair> availablePairs = new List<CustomerCasePair>();


    private int customersServed = 0; // track number of customers
    public int maxCustomers = 5;     // stop after 5

    private int stalkerSpawned = 0;

    public float bufferTime = 2f; // default = 2 seconds


    void Start()
    {
        // Fill availablePairs with all possible customer-case combinations
        foreach (var customer in customers)
        {
            foreach (var c in customer.possibleCases)
            {
                availablePairs.Add(new CustomerCasePair(customer, c));
            }

            // 👇 Reset clock to 1 PM at game start
            if (clockManager != null)
            {
                clockManager.ResetClock();
            }
        }
        // Start the cycle automatically
        SpawnRandomCustomer();
    }

    public void SpawnRandomCustomer()
    {
        if (customersServed >= maxCustomers)
        {
            Debug.Log("All customers have been served!");
            return;
        }

        if (availablePairs.Count == 0)
        {
            Debug.Log("All customer-case pairs have been spawned!");
            return;
        }

        CustomerCasePair pairToSpawn = null;

        // Calculate remaining customers and ensure at least 1 stalker
        int remainingCustomers = maxCustomers - customersServed;
        bool mustSpawnStalker = (stalkerSpawned == 0 && remainingCustomers == 1);

        if (mustSpawnStalker)
        {
            // Pick a stalker case
            List<CustomerCasePair> stalkerPairs = availablePairs.FindAll(p => p.customerCase.caseType == CaseType.Stalker);
            if (stalkerPairs.Count > 0)
            {
                int index = Random.Range(0, stalkerPairs.Count);
                pairToSpawn = stalkerPairs[index];
            }
        }
        else
        {
            // Normal random weighted spawn (4 Ghost : 1 Stalker)
            List<CustomerCasePair> ghostPairs = availablePairs.FindAll(p => p.customerCase.caseType == CaseType.Ghost);
            List<CustomerCasePair> stalkerPairs = availablePairs.FindAll(p => p.customerCase.caseType == CaseType.Stalker);

            float totalWeight = ghostPairs.Count * 4 + stalkerPairs.Count * 1;
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
            // fallback: just pick any
            int index = Random.Range(0, availablePairs.Count);
            pairToSpawn = availablePairs[index];
        }


        //// Pick a random pair
        availablePairs.Remove(pairToSpawn);

        activeCase = pairToSpawn.customerCase;

        if (activeCustomer != null) Destroy(activeCustomer);
        activeCustomer = Instantiate(pairToSpawn.customer.customerPrefab, spawnPoint.position, spawnPoint.rotation);

        if (activeCase.caseType == CaseType.Stalker)
            stalkerSpawned++;

        Debug.Log($"Customer: {activeCustomer.name}, Case: {activeCase.caseName} ({activeCase.caseType})");


        Sprite[] randomPhotos = GetRandomPhotos(activeCase.evidencePhotos, 3);

        if (photoPanelManager != null)
        {
            photoPanelManager.ShowEvidencePhotos(randomPhotos);

            StartCoroutine(ShowThumbnailNextFrame());
        }

        // Show the dialogue box with typing effect
        if (activeCase != null && dialogueManager != null)
        {
            dialogueManager.ShowDialogue(activeCase.description);
        }
    }
    // 👇 Place the coroutine here (still inside the class)
    private IEnumerator ShowThumbnailNextFrame()
    {
        yield return null; // wait for next frame
        photoPanelManager.ShowThumbnail();
    }

    public void CustomerDone(float bufferTime)
    {
        if (activeCustomer != null) Destroy(activeCustomer);
        activeCustomer = null;
        customersServed++;

        // Advance clock by 1 hour
        if (clockManager != null)
        {
            clockManager.AdvanceHour();
        }

        if (photoPanelManager != null)
        {
            photoPanelManager.HideThumbnail();
        }

        if (dialogueManager != null)
        {
            dialogueManager.HideDialogue();
        }


        if (customersServed < maxCustomers)
        {
            StartCoroutine(SpawnNextCustomerAfterDelay(bufferTime));
        }
        else
        {
            Debug.Log("All customers served for today.");
        }



    }

    private IEnumerator SpawnNextCustomerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnRandomCustomer();
    }


    // -----------------------------
    // Public getter for the current active case
    public CustomerCase GetActiveCase()
    {
        return activeCase;
    }

    private Sprite[] GetRandomPhotos(Sprite[] pool, int count)
    {
        List<Sprite> shuffled = new List<Sprite>(pool);

        // Fisher-Yates shuffle
        for (int i = 0; i < shuffled.Count; i++)
        {
            int randIndex = Random.Range(i, shuffled.Count);
            Sprite temp = shuffled[i];
            shuffled[i] = shuffled[randIndex];
            shuffled[randIndex] = temp;
        }

        // Take first N
        int finalCount = Mathf.Min(count, shuffled.Count);
        return shuffled.GetRange(0, finalCount).ToArray();
    }
}



