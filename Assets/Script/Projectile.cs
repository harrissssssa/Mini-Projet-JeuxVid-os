using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public float lifeTime = 5f; // détruit le projectile après X secondes si rien n'est touché

    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        // Calcule l'angle selon la direction et applique la rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Ajuste l'offset selon l'orientation de base de ton sprite de poing :
        transform.rotation = Quaternion.Euler(0f, 0f, angle); // si le poing pointe vers la DROITE à la base
        // transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f); // si le poing pointe vers le HAUT à la base
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth boss = other.GetComponent<EnemyHealth>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}