using UnityEngine;
using System.Collections.Generic;

public class HealthPickupSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject healthPickupPrefab;
    [SerializeField] private int maxPickups = 5;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float minDistanceBetween = 3f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float spawnCheckRadius = 0.5f;

    [Header("Timing")]
    [SerializeField] private float initialSpawnDelay = 2f;
    [SerializeField] private float respawnDelay = 30f;

    private List<Vector2> spawnedPositions = new List<Vector2>();

    private void Start()
    {
        InvokeRepeating(nameof(SpawnPickups), initialSpawnDelay, respawnDelay);
    }

    private void SpawnPickups()
    {
        // Clear destroyed pickup positions
        spawnedPositions.RemoveAll(pos => Vector2.Distance(pos, transform.position) > spawnRadius * 2);

        while (spawnedPositions.Count < maxPickups)
        {
            Vector2 randomPos = GetValidSpawnPosition();
            if (randomPos != Vector2.negativeInfinity)
            {
                Instantiate(healthPickupPrefab, randomPos, Quaternion.identity);
                spawnedPositions.Add(randomPos);
            }
            else
            {
                Debug.LogWarning("Failed to find valid spawn position after attempts");
                break;
            }
        }
    }

    private Vector2 GetValidSpawnPosition()
    {
        for (int i = 0; i < 30; i++) // Try 30 times to find valid position
        {
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            
            // Check if position is valid
            if (IsPositionValid(randomPos))
            {
                return randomPos;
            }
        }
        return Vector2.negativeInfinity;
    }

    private bool IsPositionValid(Vector2 position)
    {
        // Check for obstacles (walls)
        if (Physics2D.OverlapCircle(position, spawnCheckRadius, obstacleMask))
        {
            return false;
        }

        // Check distance from other pickups
        foreach (Vector2 existingPos in spawnedPositions)
        {
            if (Vector2.Distance(position, existingPos) < minDistanceBetween)
            {
                return false;
            }
        }

        return true;
    }

    // Visualize spawn area in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}