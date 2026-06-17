using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    public Rigidbody2D rb;
    public Animator animator;
    public ParticleSystem walkDust;
    public GameObject slashEffect;
    public Animator slashAnimator;

    [Header("Attack")]
    public GameObject swordHitbox;
    public float comboCooldown = 0.8f;

    private bool isAttacking = false;
    private int comboStep = 0;
    private bool comboQueued = false;
    private float cooldownTimer = 0f;
    private bool onCooldown = false;
    private Vector3 originalSlashScale;

    public void EnableSlash()
    {
        slashEffect.SetActive(true);
        slashAnimator.Play("Slash", 0, 0f);
    }

    public void EnableHitbox()
    {
        swordHitbox.SetActive(true);
    }

    public void DisableHitbox()
    {
        swordHitbox.SetActive(false);
    }

    public void DisableSlash()
    {
        slashEffect.SetActive(false);
    }

    void Start()
    {
        originalSlashScale = slashEffect.transform.localScale;
    }

    private Vector2 moveInput;

    void Update()
    {
        // ===== COOLDOWN TIMER =====
        if (onCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                onCooldown = false;
                comboStep = 0;
            }
        }

        // ===== INPUT =====
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // ===== ATTACK INPUT =====
        if (Input.GetMouseButtonDown(0))
        {
            if (onCooldown) return;

            if (!isAttacking && comboStep == 0)
            {
                comboStep = 1;
                StartAttack(1);
            }
            else if (isAttacking && comboStep == 1)
            {
                comboQueued = true;
            }
        }

        // ===== FLIP CHARACTER =====
        if (!isAttacking)
        {
            if (moveInput.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (moveInput.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        // ===== WALK ANIMATION =====
        animator.SetBool("isWalking",
            moveInput != Vector2.zero && !isAttacking);

        // ===== WALK DUST =====
        if (moveInput != Vector2.zero && !isAttacking)
        {
            if (!walkDust.isPlaying) walkDust.Play();
        }
        else
        {
            if (walkDust.isPlaying) walkDust.Stop();
        }
    }

    void FixedUpdate()
    {
        if (!isAttacking)
            rb.linearVelocity = moveInput * speed;
        else
            rb.linearVelocity = Vector2.zero;
    }

    void StartAttack(int step)
    {
        isAttacking = true;
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        if (step == 1)
        {
            slashEffect.transform.localScale = originalSlashScale;
            animator.SetTrigger("Attack");
        }
        else
        {
            slashEffect.transform.localScale = new Vector3(
                originalSlashScale.x,
                -originalSlashScale.y,
                originalSlashScale.z);
            animator.SetTrigger("Attack2");
        }
    }

    // Dipanggil Animation Event di akhir Attack1
    public void EndAttack1()
    {
        if (comboQueued)
        {
            comboQueued = false;
            comboStep = 2;
            StartAttack(2);
        }
        else
        {
            EndCombo();
        }
    }

    // Dipanggil Animation Event di akhir Attack2
    public void EndAttack2()
    {
        EndCombo();
    }

    void EndCombo()
    {
        isAttacking = false;
        comboStep = 0;
        comboQueued = false;
        onCooldown = true;
        cooldownTimer = comboCooldown;
    }
}