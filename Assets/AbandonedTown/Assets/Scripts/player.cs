using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {

    Rigidbody2D rb;
    Animator animator;
    float slashTime = 0.5f;
    float jumpTime = 0.5f;
    float jumpTime2 = 0.5f;
    bool slash = false;
    bool jump = false;
    bool jump2 = false; 
 
    bool isGround; 
    RaycastHit2D hit; 


    // Use this for initialization
    void Start () {
     


        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
     

    }
	
	// Update is called once per frame
	void Update () {

        hit = Physics2D.Raycast(transform.position + new Vector3(0,-1.1f,0), -Vector2.up,0.01f);
        if (hit.collider)
        {
            Debug.Log(hit.collider); 
          
            //Debug.Log(hit.transform.name);
        }

        if (hit.collider)
            animator.SetBool("inAir", false);
            else
            animator.SetBool("inAir", true);


            if (transform.position.y >= 8f)
        {
            transform.position = new Vector3(transform.position.x, 8, transform.position.z);
        }

        if (transform.position.y <= -3f)
        {
            transform.position = new Vector3(transform.position.x, -3f, transform.position.z);
        }
        if (Input.GetKey(KeyCode.A)&& slash ==false)
        {
            transform.Translate(-0.1f, 0, 0);

                animator.SetBool("run", true);



            transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);

        }

        if (Input.GetKey(KeyCode.D)&& slash == false)
        {
            transform.Translate(0.1f, 0, 0);
                animator.SetBool("run", true);
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        }

        if (Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.D) == false)
        {
                animator.SetBool("run", false);
        }



        if (Input.GetKeyDown(KeyCode.E))
        {
            slash = true; 
    
            animator.SetBool("slash", true);
        }

   
        if(slash == true)
        {
            slashTime -= 0.02f;
        }
        if (slashTime <= 0)
        {
            slash = false; 
            animator.SetBool("slash", false);
            slashTime = 0.6f;
          
        }

        if (Input.GetKeyDown(KeyCode.Space) && hit.collider)
        {
            //hit.collider
            if (jump == false)
            {
                jump = true;
                animator.SetBool("jump", true);

            }



            rb.AddForce(new Vector3(0, 300, 0));
        }
        if (jump == false && hit.collider)
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0.0899f, gameObject.transform.position.z);

        if (jump == true)
        {

            jumpTime -= 0.01f;
        }
        if (jumpTime <= 0)
        {
            jump = false;
            animator.SetBool("jump", false);
            jumpTime = 0.6f;

        }



    }
}
