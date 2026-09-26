using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vie")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Invincibilite apres coup")]
    [Tooltip("Empeche de perdre de la vie en boucle pendant l'animation de degats")]
    public float invincibilityDuration = 1f;
    private bool isInvincible = false;

    [Header("Feedback visuel (optionnel)")]
    public SpriteRenderer spriteRenderer;
    public float flashDuration = 0.1f;

    [Header("Animator (optionnel)")]
    public Animator animator;

    [Header("Game Over")]
    [Tooltip("Le panel UI (dans le Canvas) a afficher quand le joueur meurt")]
    public GameObject gameOverPanel;
    [Tooltip("Met le jeu en pause (Time.timeScale = 0) quand le Game Over s'affiche")]
    public bool pauseOnDeath = true;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible || isDead || currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"Joueur touche ! Vie restante : {currentHealth}/{maxHealth}");

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        if (spriteRenderer != null)
        {
            // Petit clignotement pendant l'invincibilite
            float elapsed = 0f;
            while (elapsed < invincibilityDuration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(flashDuration);
                elapsed += flashDuration;
            }
            spriteRenderer.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(invincibilityDuration);
        }

        isInvincible = false;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Le joueur est mort.");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (pauseOnDeath)
        {
            Time.timeScale = 0f;
        }
    }

    // A appeler depuis le bouton "Rejouer" du panel Game Over
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    // A appeler depuis un bouton "Quitter" du panel Game Over (optionnel)
    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}