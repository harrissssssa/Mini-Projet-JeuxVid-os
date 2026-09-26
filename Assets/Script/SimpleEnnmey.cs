using System.Collections;
using UnityEngine;

public class SimpleEnemyAI : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 6f;
    public float attackRange = 0.5f;

    [Header("Mouvement")]
    public float moveSpeed = 2.5f;

    [Header("Attaque")]
    public float attackCooldown = 1.2f;
    public int attackDamage = 5;
    public float damageDelay = 0.3f;
    public float attackAnimationDuration = 0.6f;
    private float lastAttackTime = -999f;
    private bool isAttacking = false;

    [Header("Physique")]
    public bool preventPushingPlayer = true;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D myCollider;
    private Collider2D playerCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>();

        FindPlayer();
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerCollider = player.GetComponent<Collider2D>();
            ApplyCollisionSettings();
        }
    }

    void ApplyCollisionSettings()
    {
        if (preventPushingPlayer && myCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(myCollider, playerCollider, true);
        }
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float deltaX = player.position.x - transform.position.x;

        float edgeDistance = float.MaxValue;
        if (myCollider != null && playerCollider != null)
        {
            ColliderDistance2D colDist = myCollider.Distance(playerCollider);
            if (colDist.isValid)
            {
                edgeDistance = colDist.distance;
            }
        }

        bool inAttackRange = edgeDistance <= attackRange;
        bool inDetectionRange = edgeDistance <= detectionRange;

        // Tourner l'ennemi face au joueur
        if (deltaX > 0.05f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (deltaX < -0.05f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (inAttackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (animator != null)
            {
                animator.SetFloat("Speed", 0);
            }

            if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(AttackRoutine());
            }
        }
        else if (inDetectionRange)
        {
            float direction = deltaX > 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

            if (animator != null)
            {
                animator.SetFloat("Speed", 1);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (animator != null)
            {
                animator.SetFloat("Speed", 0);
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(damageDelay);
        DealDamageToPlayer();

        yield return new WaitForSeconds(attackAnimationDuration - damageDelay);

        lastAttackTime = Time.time;
        isAttacking = false;
    }

    void DealDamageToPlayer()
    {
        if (player == null) return;

        float edgeDistance = float.MaxValue;
        if (myCollider != null && playerCollider != null)
        {
            ColliderDistance2D colDist = myCollider.Distance(playerCollider);
            if (colDist.isValid)
            {
                edgeDistance = colDist.distance;
            }
        }

        if (edgeDistance > attackRange) return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}