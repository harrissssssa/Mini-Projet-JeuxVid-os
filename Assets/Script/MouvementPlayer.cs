using UnityEngine;

public class MouvementPlayer : MonoBehaviour
{
    [Header("Vitesse")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 12f;
    public float jumpForce = 5f;

    [Header("Tir")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Rigidbody2D rb;
    private Animator animator;

    private float move;
    private float speed;
    private bool isGrounded = true;
    private bool isSprinting = false;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        speed = walkSpeed;
    }

    void Update()
    {
        // Déplacement horizontal
        move = Input.GetAxisRaw("Horizontal");

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift) && move != 0)
        {
            isSprinting = true;
            speed = sprintSpeed;
        }


        // Flip du personnage
        if (move > 0)
        {
            transform.localScale = new Vector3(4, 5, 1);
            facingRight = true;
        }
        else if (move < 0)
        {
            transform.localScale = new Vector3(-4, 5, 1);
            facingRight = false;
        }

        animator.SetFloat("Speed", Mathf.Abs(move));

        // Saut
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("Isjumping", true);
        }

        // Attaque
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.SetTrigger("Attack");
        }

        // Blocage (tant que F est maintenu)
        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetBool("Block", true);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            animator.SetBool("Block", false);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
    }

    // Appelé via Animation Event sur le clip d'attaque
    public void Shoot()
    {
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
        {
            p.SetDirection(dir);
        }
    }

    // Détection du sol
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Isjumping", false);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}