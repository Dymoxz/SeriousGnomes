using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [Header("Cloud Setup")]
    public GameObject[] cloudPrefabs;

    [Header("Density Settings")]
    [Tooltip("Time in seconds between cloud spawns. Lower number = higher density.")]
    public float spawnInterval = 1.5f;

    [Tooltip("How many clouds should spawn every time the interval hits?")]
    public int cloudsPerSpawn = 1;

    [Header("Spawn Area Bounds")]
    public float minY = -3f;
    public float maxY = 5f;

    [Tooltip("Closest point toward the camera")]
    public float minZ = -5f;
    [Tooltip("Furthest point away from the camera")]
    public float maxZ = 5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        // When the timer hits the interval, spawn the clouds
        if (timer >= spawnInterval)
        {
            for (int i = 0; i < cloudsPerSpawn; i++)
            {
                SpawnCloud();
            }

            timer = 0f; // Reset timer
        }
    }

    void SpawnCloud()
    {
        // 1. Pick a random cloud
        int randomIndex = Random.Range(0, cloudPrefabs.Length);
        GameObject selectedCloud = cloudPrefabs[randomIndex];

        // 2. Pick a random Y (Height) and Z (Depth)
        float randomY = Random.Range(minY, maxY);
        float randomZ = Random.Range(minZ, maxZ);

        Vector3 spawnPosition = new Vector3(transform.position.x, randomY, randomZ);

        // 3. Spawn the cloud
        Instantiate(selectedCloud, spawnPosition, Quaternion.identity);
    }
}