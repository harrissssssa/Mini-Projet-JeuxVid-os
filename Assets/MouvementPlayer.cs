using UnityEngine;

public class MouvementPlayer : MonoBehaviour
{
    [Header("Vitesse")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 12f;
    public float jumpForce = 5f;

    private Rigidbody2D rb;
    private Animator animator;

    private float move;
    private float speed;
    private bool isGrounded = true;
    private bool isSprinting = false;

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
        else
        {
            isSprinting = false;
            speed = walkSpeed;
        }
        animator.SetBool("IsRunning", isSprinting);

        // Flip du personnage
        if (move > 0)
        {
            transform.localScale = new Vector3(4, 5, 1);
        }
        else if (move < 0)
        {
            transform.localScale = new Vector3(-4, 5, 1);
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
            animator.SetTrigger("Attack1");
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