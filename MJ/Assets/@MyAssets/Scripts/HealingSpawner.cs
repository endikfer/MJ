using UnityEngine;
using System.Collections;

public class HealingSpawner : MonoBehaviour
{
    public static HealingSpawner Instance;

    public GameObject healingPrefab;
    public Transform[] spawnPoints;

    private GameObject currentHealingItem;
    private bool firstCollected = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(SpawnFirstAfterDelay(5f));
    }

    IEnumerator SpawnFirstAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnHealingItem();
    }

    void SpawnHealingItem()
    {
        int index = Random.Range(0, spawnPoints.Length);
        currentHealingItem = Instantiate(healingPrefab, spawnPoints[index].position, Quaternion.identity);
    }

    public void OnHealingItemCollected()
    {
        if (!firstCollected)
        {
            firstCollected = true;
        }

        // Espera 15 segundos antes de volver a instanciar
        StartCoroutine(WaitAndSpawnNext(15f));
    }

    IEnumerator WaitAndSpawnNext(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnHealingItem();
    }
}
