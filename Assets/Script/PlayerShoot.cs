using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Transform target; // glisse le Boss ici dans l'Inspector

    public void Shoot()
    {
        Vector2 dir;

        if (target != null)
        {
            dir = (target.position - firePoint.position).normalized;
        }
        else
        {
            dir = Vector2.right; // fallback si pas de cible
        }

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
        {
            p.SetDirection(dir);
        }
    }
}