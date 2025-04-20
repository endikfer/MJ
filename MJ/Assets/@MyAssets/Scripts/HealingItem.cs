using UnityEngine;

public class HealingItem : MonoBehaviour
{
    public int healAmount = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerLIfe vida = other.GetComponent<PlayerLIfe>();
            if (vida != null)
            {
                vida.Heal(healAmount);
            }

            HealingSpawner.Instance.OnHealingItemCollected();
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.Rotate(0f, 90f * Time.deltaTime, 0f);
    }
}
