using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject fishPrefab;
    public int initialFishCount = 5;
    public float spawnRadius = 20f;

    void Start()
    {
        for (int i = 0; i < initialFishCount; i++)
        {
            SpawnFish();
        }
    }

    void SpawnFish()
    {
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
        spawnPosition.y = 0f; // Keep fish at water level
        Instantiate(fishPrefab, spawnPosition, Quaternion.identity);
    }
}
