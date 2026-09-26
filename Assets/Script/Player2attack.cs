using UnityEngine;

public class Player2attack : MonoBehaviour
{
    public int attackDamage = 10;
    private bool isAttacking = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttacking) return;

        EnemyHealth boss = other.GetComponent<EnemyHealth>();
        if (boss != null)
        {
            boss.TakeDamage(attackDamage);
        }
    }

    // Called via Animation Event at the start of the attack animation
    public void StartAttack()
    {
        isAttacking = true;
    }

    // Called via Animation Event at the end of the attack animation
    public void EndAttack()
    {
        isAttacking = false;
    }
}