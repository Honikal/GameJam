using UnityEngine;
using System.Collections.Generic;

public class MedkitSpawner : MonoBehaviour
{
    public GameObject medkitPrefab;
    public LayerMask obstacleLayers; // Assign both walls and medkit layers here
    public int maxMedkits = 5;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;
    public float minDistanceBetween = 2f; // Minimum space between medkits

    private List<GameObject> activeMedkits = new List<GameObject>();

    void Start()
    {
        SpawnInitialMedkits();
    }

    void SpawnInitialMedkits()
    {
        for (int i = 0; i < maxMedkits; i++)
        {
            TrySpawnMedkit();
        }
    }

    void TrySpawnMedkit()
    {
        Vector3 spawnPosition;
        bool positionFound = false;
        int attempts = 0;
        int maxAttempts = 50; // Increased attempts for better placement

        while (!positionFound && attempts < maxAttempts)
        {
            attempts++;
            
            spawnPosition = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y),
                0f
            );

            // Check if position is clear of both walls AND other medkits
            if (IsPositionValid(spawnPosition))
            {
                GameObject newMedkit = Instantiate(medkitPrefab, spawnPosition, Quaternion.identity);
                activeMedkits.Add(newMedkit);
                positionFound = true;
                Debug.Log($"Spawned medkit at {spawnPosition}");
            }
        }

        if (!positionFound)
        {
            Debug.LogWarning("Failed to find valid spawn position");
        }
    }

    bool IsPositionValid(Vector3 position)
    {
        // Check for walls/obstacles
        if (Physics2D.OverlapCircle(position, 0.5f, obstacleLayers))
        {
            return false;
        }

        // Check distance to other medkits
        foreach (var medkit in activeMedkits)
        {
            if (medkit != null && Vector2.Distance(position, medkit.transform.position) < minDistanceBetween)
            {
                return false;
            }
        }

        return true;
    }

    public void MedkitPickedUp(GameObject medkit)
    {
        if (activeMedkits.Contains(medkit))
        {
            activeMedkits.Remove(medkit);
        }
        Destroy(medkit);
        
        // Respawn a new one to maintain count
        TrySpawnMedkit();
    }
}