using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Cibles")]
    public Transform player;

    [Header("Détection")]
    public float detectionRange = 8f;
    public float attackRange = 1.5f;

    [Header("Mouvement")]
    public float moveSpeed = 3f;

    [Header("Attaque")]
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("Sol")]
    public bool isGrounded = true;

    private Rigidbody2D rb;
    private Animator animator;
    private float distanceToPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        // Si pas assigné manuellement, cherche automatiquement l'objet avec le tag "Player"
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Flip vers le joueur
        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);

        if (distanceToPlayer <= attackRange)
        {
            // Assez proche : attaque
            Attack();
            animator.SetFloat("speed", 0);
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // Joueur détecté mais trop loin : poursuite
            Chase();
            animator.SetFloat("speed", 1);
        }
        else
        {
            // Joueur hors de portée : idle
            animator.SetFloat("speed", 0);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void Chase()
    {
        float direction = player.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    void Attack()
    {
        // Arrête le mouvement pendant l'attaque
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack1");

            // Optionnel : infliger des dégâts ici, ou via un Animation Event
            // player.GetComponent<PlayerHealth>().TakeDamage(10);
        }
    }

    // Visualise les portées dans l'éditeur (utile pour régler les distances)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}