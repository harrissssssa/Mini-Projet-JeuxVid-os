using UnityEngine;

// A placer sur le GameObject enfant "Hitbox" (avec un Collider2D en Is Trigger)
public class PlayerHitbox : MonoBehaviour
{
    [HideInInspector]
    public int damageAmount = 20;

    // Empeche de toucher plusieurs fois le meme ennemi pendant que le hitbox est actif
    private System.Collections.Generic.HashSet<Collider2D> alreadyHit = new System.Collections.Generic.HashSet<Collider2D>();

    void OnEnable()
    {
        alreadyHit.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hitbox a touche : " + other.name);

        if (alreadyHit.Contains(other)) return;

        EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageAmount);
            alreadyHit.Add(other);
        }
        else
        {
            Debug.Log("Aucun EnemyHealth trouve sur " + other.name + " ni ses parents.");
        }
    }
}