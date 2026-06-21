using System.Collections.Generic;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public int damage = 1;

    [Header("Critical Hit")]
    public float critChance = 0.15f; // 15%
    public int critMultiplier = 3;

    private List<Collider2D> alreadyHit = new List<Collider2D>();

    private void OnEnable()
    {
        alreadyHit.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (alreadyHit.Contains(other)) return;

        alreadyHit.Add(other);

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            Vector2 hitDirection =
                (other.transform.position - transform.position).normalized;

            bool isCrit = Random.value < critChance;
            int finalDamage = isCrit ? damage * critMultiplier : damage;

            enemy.TakeDamage(finalDamage, hitDirection, isCrit);
        }
    }
}
