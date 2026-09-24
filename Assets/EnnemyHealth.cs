using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vie")]
    public int maxHealth = 200;
    public int currentHealth;

    [Header("Invincibilite apres coup (optionnel)")]
    [Tooltip("Empeche de perdre de la vie en boucle si le hitbox du joueur reste actif plusieurs frames")]
    public float invincibilityDuration = 0.2f;
    private bool isInvincible = false;

    [Header("Feedback visuel (optionnel)")]
    public SpriteRenderer spriteRenderer;
    public float flashDuration = 0.05f;
    public Color hitFlashColor = Color.red;
    private Color originalColor;

    [Header("Animator (optionnel)")]
    public Animator animator;

    [Header("Mort")]
    [Tooltip("Delai avant de detruire le GameObject apres la mort (laisse le temps a une animation de mort si tu en as une)")]
    public float destroyDelay = 0f;
    public GameObject deathEffect; // prefab optionnel (particules, loot, etc.)
    private bool isDead = false;

    [Header("Niveau suivant")]
    [Tooltip("Coche pour charger un autre niveau quand ce boss meurt")]
    public bool loadNextLevelOnDeath = false;
    [Tooltip("Nom exact de la scene a charger (doit etre ajoutee dans Build Settings). Laisse vide pour utiliser Next Level Build Index a la place.")]
    public string nextLevelName = "";
    [Tooltip("Utilise seulement si Next Level Name est vide. Index de la scene dans Build Settings (File > Build Settings).")]
    public int nextLevelBuildIndex = -1;
    [Tooltip("Delai avant de changer de niveau, pour laisser jouer l'animation de mort")]
    public float levelLoadDelay = 2f;

    void Start()
    {
        currentHealth = maxHealth;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible || isDead || currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"{gameObject.name} touche ! Vie restante : {currentHealth}/{maxHealth}");

        if (spriteRenderer != null)
        {
            StartCoroutine(HitFlashRoutine());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (invincibilityDuration > 0f)
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    IEnumerator HitFlashRoutine()
    {
        spriteRenderer.color = hitFlashColor;
        yield return new WaitForSeconds(flashDuration);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} est mort.");

        // Desactive l'IA pour arreter tout mouvement/attaque
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // evite que le corps continue a etre pousse par la physique
        }

        // Desactive le collider pour que le corps ne bloque plus le joueur ni ne reçoive plus de coups
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        if (loadNextLevelOnDeath)
        {
            StartCoroutine(LoadNextLevelRoutine());
        }
        else
        {
            // Le GameObject reste actif pendant destroyDelay pour laisser jouer l'animation de mort
            Destroy(gameObject, destroyDelay);
        }
    }

    IEnumerator LoadNextLevelRoutine()
    {
        yield return new WaitForSeconds(levelLoadDelay);

        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else if (nextLevelBuildIndex >= 0)
        {
            SceneManager.LoadScene(nextLevelBuildIndex);
        }
        else
        {
            // Par defaut : charge la scene suivante dans l'ordre du Build Settings
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentIndex + 1);
        }
    }
}