using UnityEngine;

public class SlimeAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Jump, Attack, Recover }

    [Header("Target")]
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float chaseRange = 5f;
    public float attackRange = 2f; // jarak trigger lompat, dalam satuan unit (1 tile = 1 unit jika PPU sesuai)

    [Header("Line of Sight")]
    public LayerMask obstacleLayer;
    public float sideStepDistance = 1f;

    [Header("Patrol")]
    public float patrolRadius = 3f;
    public float patrolWaitTime = 1f; // dikit aja biar ga banyak diem
    private Vector2 startPos;
    private Vector2 patrolTarget;
    private float patrolWaitTimer;

    [Header("Jump Attack")]
    public float jumpDuration = 0.3f;
    public float jumpDistanceMultiplier = 1.1f; // jarak lompat relatif ke posisi player saat take-off
    public float attackDuration = 0.3f;   // durasi fase attack (hitbox aktif di sini)
    public float recoverDuration = 0.3f;
    public float attackCooldown = 1.2f;

    public GameObject attackHitbox;

    [HideInInspector]
    public bool canMove = true;

    private State currentState = State.Patrol;
    private float originalScaleX;
    private float attackTimer;
    private Vector2 jumpTarget;
    private Vector2 jumpStartPos;
    private float jumpTimer;
    private float attackTimerInternal;
    private float recoverTimer;

    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        originalScaleX = Mathf.Abs(transform.localScale.x);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPos = transform.position;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        PickNewPatrolPoint();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        if (!canMove)
        {
            animator.SetBool("isWalking", false);
            return;
        }

        switch (currentState)
        {
            case State.Patrol:
                DoPatrol();
                break;
            case State.Chase:
                DoChase();
                break;
            case State.Jump:
                DoJump();
                break;
            case State.Attack:
                DoAttack();
                break;
            case State.Recover:
                DoRecover();
                break;
        }
    }

    // ================= PATROL =================
    void PickNewPatrolPoint()
    {
        Vector2 randomOffset = Random.insideUnitCircle * patrolRadius;
        patrolTarget = startPos + randomOffset;
        patrolWaitTimer = patrolWaitTime;
    }

    void DoPatrol()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) <= chaseRange)
        {
            currentState = State.Chase;
            return;
        }

        float distToTarget = Vector2.Distance(transform.position, patrolTarget);

        if (distToTarget < 0.2f)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);

            patrolWaitTimer -= Time.deltaTime;
            if (patrolWaitTimer <= 0)
            {
                PickNewPatrolPoint();
            }
        }
        else
        {
            Vector2 dir = (patrolTarget - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * (moveSpeed * 0.5f);
            animator.SetBool("isWalking", true);
            FaceDirection(dir.x);
        }
    }

    // ================= CHASE (dengan LOS) =================
    void DoChase()
    {
        if (player == null)
        {
            currentState = State.Patrol;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > chaseRange * 1.3f)
        {
            currentState = State.Patrol;
            PickNewPatrolPoint();
            return;
        }

        if (distance <= attackRange && HasLineOfSight())
        {
            if (attackTimer <= 0)
            {
                // Langsung lompat, tanpa windup
                jumpStartPos = transform.position;
                Vector2 dirToPlayer = ((Vector2)player.position - jumpStartPos);
                jumpTarget = jumpStartPos + dirToPlayer * jumpDistanceMultiplier;

                currentState = State.Jump;
                jumpTimer = 0f;
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("isWalking", false);
                animator.SetTrigger("Jump");
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("isWalking", false);
            }
            return;
        }

        Vector2 moveDir = GetChaseDirection();
        rb.linearVelocity = moveDir * moveSpeed;
        animator.SetBool("isWalking", true);
        FaceDirection(moveDir.x);
    }

    bool HasLineOfSight()
    {
        Vector2 dirToPlayer = (player.position - transform.position);
        float dist = dirToPlayer.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer.normalized, dist, obstacleLayer);
        return hit.collider == null;
    }

    Vector2 GetChaseDirection()
    {
        Vector2 directDir = (player.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directDir, dist, obstacleLayer);

        if (hit.collider == null)
        {
            return directDir;
        }

        Vector2 perpendicular = new Vector2(-directDir.y, directDir.x);

        Vector2[] candidates = new Vector2[]
        {
            (directDir + perpendicular).normalized,
            (directDir - perpendicular).normalized,
            perpendicular,
            -perpendicular
        };

        foreach (Vector2 candidate in candidates)
        {
            RaycastHit2D sideHit = Physics2D.Raycast(transform.position, candidate, sideStepDistance, obstacleLayer);
            if (sideHit.collider == null)
            {
                return candidate;
            }
        }

        return directDir;
    }

    // ================= JUMP =================
    void DoJump()
    {
        jumpTimer += Time.deltaTime;
        float t = Mathf.Clamp01(jumpTimer / jumpDuration);

        Vector2 newPos = Vector2.Lerp(jumpStartPos, jumpTarget, t);
        rb.MovePosition(newPos);

        if (t >= 1f)
        {
            currentState = State.Attack;
            attackTimerInternal = 0f;
            animator.SetTrigger("Attack");
            if (attackHitbox != null) attackHitbox.SetActive(true);
        }
    }

    // ================= ATTACK =================
    void DoAttack()
    {
        rb.linearVelocity = Vector2.zero;
        attackTimerInternal += Time.deltaTime;

        if (attackTimerInternal >= attackDuration)
        {
            if (attackHitbox != null) attackHitbox.SetActive(false);

            attackTimer = attackCooldown;
            currentState = State.Recover;
            recoverTimer = recoverDuration;
        }
    }

    // ================= RECOVER =================
    void DoRecover()
    {
        rb.linearVelocity = Vector2.zero;
        recoverTimer -= Time.deltaTime;

        if (recoverTimer <= 0)
        {
            currentState = State.Chase;
        }
    }

    // ================= HELPER =================
    void FaceDirection(float xDir)
    {
        if (xDir > 0.05f)
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        else if (xDir < -0.05f)
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);
    }

    // ================= EXTERNAL CALLS =================
    public void EnableHitbox() => attackHitbox.SetActive(true);
    public void DisableHitbox() => attackHitbox.SetActive(false);

    public void EndAttack()
    {
        if (attackHitbox != null) attackHitbox.SetActive(false);
    }

    // Dipanggil dari EnemyHealth saat slime kena hit
    public void InterruptAttack()
    {
        if (attackHitbox != null) attackHitbox.SetActive(false);
        attackTimer = attackCooldown * 0.5f;
        currentState = State.Chase;
    }
}