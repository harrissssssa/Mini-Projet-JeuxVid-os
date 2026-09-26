using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vie")]
    public int maxHealth = 30;
    public int currentHealth;

    [Header("Invincibilite apres coup (optionnel)")]
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
    public float destroyDelay = 1f;
    public GameObject deathEffect;
    private bool isDead = false;

    [Header("Niveau suivant (coche seulement pour le boss)")]
    [Tooltip("Coche UNIQUEMENT sur le boss final. Laisse decoche pour les ennemis normaux.")]
    public bool loadNextLevelOnDeath = false;
    [Tooltip("Nom exact de la scene a charger. Laisse vide pour utiliser l'index a la place.")]
    public string nextLevelName = "";
    [Tooltip("Utilise seulement si Next Level Name est vide.")]
    public int nextLevelBuildIndex = -1;
    public float levelLoadDelay = 2f;

    void Start()
    {
        currentHealth = maxHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (animator == null)
            animator = GetComponent<Animator>();
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

        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.enabled = false;
        }

        SimpleEnemyAI simpleAi = GetComponent<SimpleEnemyAI>();
        if (simpleAi != null)
        {
            simpleAi.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

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
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentIndex + 1);
        }
    }
}