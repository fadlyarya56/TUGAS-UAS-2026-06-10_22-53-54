using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.15f;
    public float hitStopDuration = 0.08f;

    [Header("Camera Shake")]
    public float shakeMultiplier = 1f;
    public float critShakeMultiplier = 2f;

    [Header("UI")]
    public EnemyHealthBar healthBar; // assign child object di Inspector

    [Header("Loot")]
    public GameObject lootPrefab;     // misal Slime Jelly
    public float lootDropChance = 0.35f; // ~3-4 dari 10

    [Header("Quest")]
    public bool countsForQuest = true; // slime = true

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator animator;
    private SlimeAI slimeAI;
    private Coroutine knockbackCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        slimeAI = GetComponent<SlimeAI>();

        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage, Vector2 hitDirection, bool isCrit = false)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (healthBar != null)
            healthBar.SetHealth(Mathf.Max(currentHealth, 0), maxHealth);

        if (DamageNumberSpawner.Instance != null)
            DamageNumberSpawner.Instance.Spawn(transform.position + Vector3.up * 0.5f, damage, isCrit);

        if (CameraImpulseManager.Instance != null)
            CameraImpulseManager.Instance.Shake(isCrit ? critShakeMultiplier : shakeMultiplier);

        if (currentHealth <= 0)
        {
            isDead = true;

            if (knockbackCoroutine != null)
                StopCoroutine(knockbackCoroutine);

            HitStopManager.Instance.Stop(hitStopDuration);
            StartCoroutine(FlashThenDie());
            return;
        }

        if (slimeAI != null)
            slimeAI.InterruptAttack();

        StartCoroutine(HitFlash());
        animator.SetTrigger("Hit");
        HitStopManager.Instance.Stop(hitStopDuration);

        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);
        knockbackCoroutine = StartCoroutine(KnockbackAfterHitStop(hitDirection, hitStopDuration));
    }

    IEnumerator FlashThenDie()
    {
        sr.color = Color.red;
        yield return new WaitForSecondsRealtime(0.1f);
        sr.color = Color.white;
        Die();
    }

    IEnumerator HitFlash()
    {
        sr.color = Color.red;
        yield return new WaitForSecondsRealtime(0.1f);
        sr.color = Color.white;
    }

    IEnumerator KnockbackAfterHitStop(Vector2 hitDirection, float stopDuration)
    {
        if (slimeAI != null) slimeAI.canMove = false;

        yield return new WaitForSecondsRealtime(stopDuration);

        if (isDead) yield break;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSecondsRealtime(knockbackDuration);

        if (!isDead && slimeAI != null)
        {
            slimeAI.canMove = true;
            knockbackCoroutine = null;
        }
    }

    void Die()
    {
        if (slimeAI != null)
        {
            slimeAI.canMove = false;
            slimeAI.InterruptAttack();
        }

        sr.color = Color.white;
        rb.linearVelocity = Vector2.zero;
        animator.ResetTrigger("Hit");
        animator.SetTrigger("Die");

        if (countsForQuest && QuestManager.Instance != null)
            QuestManager.Instance.OnSlimeKilled();

        if (lootPrefab != null && Random.value < lootDropChance)
        {
            Instantiate(lootPrefab, transform.position, Quaternion.identity);
        }

        StartCoroutine(DestroyAfterDeath());
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSecondsRealtime(0.8f);
        Destroy(gameObject);
    }
}
