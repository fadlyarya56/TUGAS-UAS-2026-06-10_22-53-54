using UnityEngine;

public class SlimeJellyPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (QuestManager.Instance != null)
                QuestManager.Instance.OnJellyCollected();

            Destroy(gameObject);
        }
    }
}
