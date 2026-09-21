using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Cibles")]
    public Transform player;

    [Header("Detection")]
    public float detectionRange = 12f;
    public float attackRange = 5f;

    [Header("Mouvement")]
    public float moveSpeed = 3.5f;

    [Header("Attaque")]
    public float attackCooldown = 1.5f;
    private float lastAttackTime = -999f;
    private bool isAttacking = false;

    [Header("Physique")]
    [Tooltip("Si actif, empeche le chevalier de pousser physiquement le joueur")]
    public bool preventPushingPlayer = true;

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D myCollider;
    private Collider2D playerCollider;
    private TealFalconEnemySeries.DarkKnightController darkKnight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        darkKnight = GetComponent<TealFalconEnemySeries.DarkKnightController>();

        // L'animator du Dark Knight se trouve sur l'enfant "Root"
        animator = GetComponentInChildren<Animator>();

        // Si le joueur n'est pas assigne dans l'inspecteur, recherche automatique par tag
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (player != null)
        {
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
        // Recherche continue si le joueur n'a pas encore ete trouve au demarrage
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerCollider = player.GetComponent<Collider2D>();
                ApplyCollisionSettings();
            }
            return;
        }

        if (playerCollider == null && player != null)
        {
            playerCollider = player.GetComponent<Collider2D>();
            ApplyCollisionSettings();
        }

        // Ne pas interrompre le mouvement ni recalculer si une attaque est en cours
        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float deltaX = player.position.x - transform.position.x;
        float horizontalDistance = Mathf.Abs(deltaX);
        float verticalDistance = Mathf.Abs(player.position.y - transform.position.y);
        float distance2D = Vector2.Distance(transform.position, player.position);

        // Distance physique bord-a-bord entre les colliders
        float edgeDistance = float.MaxValue;
        if (myCollider != null && playerCollider != null)
        {
            ColliderDistance2D colDist = myCollider.Distance(playerCollider);
            if (colDist.isValid)
            {
                edgeDistance = colDist.distance;
            }
        }

        // Le chevalier s'arrete et attaque s'il est a portee ou tout proche du joueur
        bool inAttackRange = (horizontalDistance <= attackRange || edgeDistance <= 0.5f) && verticalDistance <= 3.5f;
        bool inDetectionRange = distance2D <= detectionRange || horizontalDistance <= detectionRange;

        // Tourner le chevalier face au joueur
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
            // S'arreter net pour ne pas pousser le joueur
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
            // Joueur detecte : poursuite
            float direction = deltaX > 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

            if (animator != null)
            {
                animator.SetFloat("Speed", 1);
            }
        }
        else
        {
            // Joueur hors de portee : repos
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
            animator.SetFloat("Speed", 0);
        }

        if (darkKnight != null)
        {
            darkKnight.ActivateAttack();
            // Attendre la fin de l'attaque (duree animation = 0.75s)
            yield return new WaitForSeconds(0.8f);
        }
        else if (animator != null)
        {
            animator.Play("Attack", 0, 0f);
            yield return new WaitForSeconds(0.75f);
            animator.CrossFade("Idle", 0.15f, 0);
        }

        lastAttackTime = Time.time;
        isAttacking = false;
    }

    // Visualisation des portees dans la vue Scene de l'editeur Unity
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}