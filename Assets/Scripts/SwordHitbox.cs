using System.Collections.Generic;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public int damage = 1;

    // Simpan musuh yang sudah kena di serangan ini
    private List<Collider2D> alreadyHit = new List<Collider2D>();

    // Reset list saat hitbox diaktifkan (tiap serangan baru)
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

            enemy.TakeDamage(damage, hitDirection);
        }
    }
}