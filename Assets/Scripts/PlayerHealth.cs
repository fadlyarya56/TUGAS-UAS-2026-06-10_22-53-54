using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("References")]
    public Image healthFill;
    public Animator animator;

    [Header("Effects")]
    public GameObject deathParticle;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.15f;

    [Header("I Frame")]
    public float iFrameDuration = 0.5f;

    [Header("Regen")]
    public float regenDelay = 5f;     // detik tanpa kena damage sebelum regen mulai
    public float regenInterval = 0.5f; // tiap berapa detik nambah HP
    public int regenAmount = 1;       // HP per tick

    public bool isDead = false;

    private bool isInvincible = false;
    private float lastHitTime;
    private float regenTimer;

    private Rigidbody2D rb;
    private SpriteRenderer[] srs;
    private Color[] originalColors;
    private PlayerMovement playerMovement;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        srs = GetComponentsInChildren<SpriteRenderer>();

        originalColors = new Color[srs.Length];

        for(int i = 0; i < srs.Length; i++)
        {
            originalColors[i] = srs[i].color;
        }

        playerMovement = GetComponent<PlayerMovement>();

        currentHealth = maxHealth;
        lastHitTime = -regenDelay;

        UpdateHealthBar();
    }

    void Update()
    {
        if (isDead) return;

        // ===== HP REGEN =====
        if (currentHealth < maxHealth && Time.time - lastHitTime >= regenDelay)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= regenInterval)
            {
                regenTimer = 0f;
                currentHealth = Mathf.Min(currentHealth + regenAmount, maxHealth);
                UpdateHealthBar();
            }
        }
    }


    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isDead || isInvincible)
            return;

        lastHitTime = Time.time; // reset regen timer
        regenTimer = 0f;

        currentHealth -= damage;

        animator.SetTrigger("Hit");

        StartCoroutine(IFrame());

        StartCoroutine(Knockback(hitDirection));

        StartCoroutine(HitFlash());


        if (CameraImpulseManager.Instance != null)
        {
            CameraImpulseManager.Instance.Shake();
        }


        UpdateHealthBar();


        if (currentHealth <= 0)
        {
            Die();
        }
    }


    IEnumerator IFrame()
    {
        isInvincible = true;

        float timer = 0;

        while(timer < iFrameDuration)
        {
            foreach(SpriteRenderer sr in srs)
            {
                sr.color = new Color(1,1,1,0.3f);
            }

            yield return new WaitForSeconds(0.05f);


            for(int i = 0; i < srs.Length; i++)
            {
                srs[i].color = originalColors[i];
            }

            yield return new WaitForSeconds(0.05f);

            timer += 0.1f;
        }


        foreach(SpriteRenderer sr in srs)
        {
            sr.color = Color.white;
        }


        isInvincible = false;
    }



    IEnumerator HitFlash()
    {
        for(int i = 0; i < srs.Length; i++)
        {
            srs[i].color = Color.white;
        }


        yield return new WaitForSeconds(0.08f);


        for(int i = 0; i < srs.Length; i++)
        {
            srs[i].color = originalColors[i];
        }
    }



    IEnumerator Knockback(Vector2 direction)
    {

        playerMovement.isKnockedBack = true;


        rb.linearVelocity = direction * knockbackForce;


        yield return new WaitForSeconds(knockbackDuration);


        rb.linearVelocity = Vector2.zero;


        playerMovement.isKnockedBack = false;

    }




    void UpdateHealthBar()
    {
        healthFill.fillAmount =
        (float)currentHealth / maxHealth;
    }



    void Die()
    {

        if(isDead)
            return;


        isDead = true;


        if(playerMovement != null)
        {
            playerMovement.isDead = true;
        }


        rb.linearVelocity = Vector2.zero;



        SlimeAI[] slimes =
        FindObjectsByType<SlimeAI>(FindObjectsSortMode.None);


        foreach(SlimeAI slime in slimes)
        {
            slime.canMove = false;
        }



        if(deathParticle != null)
        {
            Instantiate(
                deathParticle,
                transform.position,
                Quaternion.identity
            );
        }



        animator.SetTrigger("Die");

        Invoke(nameof(RestartScene),2f);

    }



    void RestartScene()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

}