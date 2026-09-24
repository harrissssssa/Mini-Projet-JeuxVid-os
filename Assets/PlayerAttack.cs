using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attaque")]
    public int damageAmount = 20;
    public KeyCode attackKey = KeyCode.Mouse0; // clic gauche par defaut
    public float attackCooldown = 0.5f;
    private float lastAttackTime = -999f;

    [Header("Hitbox")]
    [Tooltip("GameObject enfant avec un Collider2D (Is Trigger coche) place devant le joueur")]
    public GameObject hitboxObject;
    [Tooltip("Duree pendant laquelle le hitbox reste actif")]
    public float hitboxActiveDuration = 0.2f;

    [Header("Animator (optionnel)")]
    public Animator animator;

    private PlayerHitbox hitboxScript;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (hitboxObject != null)
        {
            hitboxScript = hitboxObject.GetComponent<PlayerHitbox>();
            if (hitboxScript == null)
            {
                hitboxScript = hitboxObject.AddComponent<PlayerHitbox>();
            }
            hitboxObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("PlayerAttack : aucun hitboxObject assigne dans l'inspecteur.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(attackKey) && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        if (hitboxObject != null && hitboxScript != null)
        {
            hitboxScript.damageAmount = damageAmount;
            hitboxObject.SetActive(true);
            yield return new WaitForSeconds(hitboxActiveDuration);
            hitboxObject.SetActive(false);
        }
    }
}