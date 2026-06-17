using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.15f;
    public float hitStopDuration = 0.08f;

    [Header("Camera Shake")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.08f;

    private int currentHealth;
    private bool isDead = false;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator animator;
    private SlimeAI slimeAI;

    // Untuk cancel coroutine knockback yang sedang jalan
    private Coroutine knockbackCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        slimeAI = GetComponent<SlimeAI>();
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            isDead = true;

            // Cancel knockback yang mungkin masih jalan
            if (knockbackCoroutine != null)
                StopCoroutine(knockbackCoroutine);
                HitStopManager.Instance.Stop(hitStopDuration); 
                StartCoroutine(FlashThenDie());
            return;
        }

        IEnumerator FlashThenDie()
        {
            sr.color = Color.red;
            yield return new WaitForSecondsRealtime(0.1f);
            sr.color = Color.white;
            Die();
        }

        // Interrupt attack slime jika sedang menyerang
        if (slimeAI != null)
            slimeAI.InterruptAttack();

        // Flash merah
        StartCoroutine(HitFlash());

        // Trigger animasi hit
        animator.SetTrigger("Hit");

        // HitStop
        HitStopManager.Instance.Stop(hitStopDuration);

        // Cancel knockback sebelumnya jika masih jalan, lalu start yang baru
        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);
        knockbackCoroutine = StartCoroutine(KnockbackAfterHitStop(hitDirection, hitStopDuration));
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

        sr.color = Color.white; // reset warna kalau masih merah
        rb.linearVelocity = Vector2.zero;
        animator.ResetTrigger("Hit"); // buang trigger hit yang mungkin pending
        animator.SetTrigger("Die");

        StartCoroutine(DestroyAfterDeath());
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSecondsRealtime(0.8f);
        Destroy(gameObject);
    }
}