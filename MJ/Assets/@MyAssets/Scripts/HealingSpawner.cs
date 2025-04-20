using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class HealingSpawner : NetworkBehaviour
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

    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn() - ¿Es server?: " + IsServer);

        if (IsServer)
        {
            Debug.Log("Soy el host, arranco el spawn");
            StartCoroutine(SpawnFirstAfterDelay(5f));
        }
    }

    IEnumerator SpawnFirstAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnHealingItem();
    }

    void SpawnHealingItem()
    {
        int index = Random.Range(0, spawnPoints.Length);
        Debug.Log("Medkit instanciado en: " + spawnPoints[index].position + " por " + (IsServer ? "host" : "cliente"));

        GameObject item = Instantiate(healingPrefab, spawnPoints[index].position, Quaternion.identity);

        NetworkObject netObj = item.GetComponent<NetworkObject>();
        netObj.Spawn();

        currentHealingItem = item;
    }

    public void OnHealingItemCollected()
    {
        if (!firstCollected)
        {
            firstCollected = true;
        }

        StartCoroutine(WaitAndSpawnNext(15f));
    }

    IEnumerator WaitAndSpawnNext(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnHealingItem();
    }
}
