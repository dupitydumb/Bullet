using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{


    private Animator anim;
    private CharacterController controller;

    public float speed = 12f;

    [Header("Jump")]
    public float jumpHeight = 15f;
    public bool isGrounded;


    [Header("Gravity")]
    public float fallMultiplier = 2.5f;
    public float gravity = -9.81f;
    public float velocity;

    [Header("Dash")]
    public float dashSpeed = 10f;
    public float dashTime = 1f;
    public float dashCooldown = 2f;
    public bool isDashing = true;
    public GameObject dashTrail;
    Vector3 dashDir;

    [Header("Projectile")]
    public GameObject projectile;
    public GameObject ProjectorSpawnPos;

    public float FireRate = 0.5f;

    



    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
        Dash();
        DashCooldown();
        ApplyGravity();
        Jump();
        
        isGrounded = controller.isGrounded;
        
    }   

    //Movement
    public void Move()
    {
        anim.SetFloat("Side", Input.GetAxis("Horizontal"));
        anim.SetFloat("Forward", Input.GetAxis("Vertical"));
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
    }

    public void Jump()
    {
        anim.SetBool("Grounded", isGrounded);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (controller.isGrounded)
            {
                anim.SetTrigger("Jump");
                velocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                controller.Move(new Vector3(0, velocity, 0) * Time.deltaTime);
            }

        }
        else
        {
            anim.ResetTrigger("Jump");
        }
        

    }

    public void Fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Fire");
            
            //invoke Repeatly
            InvokeRepeating("Firing", 0.2f, FireRate);


            anim.SetBool("isFiring", true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            anim.ResetTrigger("Fire");
            anim.SetBool("isFiring", false);
            CancelInvoke("Firing");
        }
    }

    public void Dash()
    {
        dashDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("Dash" + speed + " " + dashSpeed);
            if (isDashing)
            {
                dashTrail.SetActive(true);
                dashCooldown = dashTime;
                isDashing = false;
                speed = 14f;
                controller.Move(dashDir * dashSpeed * Time.deltaTime);
            }
        }

        if (isDashing && dashCooldown <= 0)
        {
            dashTrail.SetActive(false);
            speed = 4f;
        }
        
    }

    public void DashCooldown(){
        dashCooldown -= Time.deltaTime;
        if (dashCooldown <= 0)
        {
            isDashing = true;
        }
    }


    public void Firing()
    {
        
        //Set anim trigger
        anim.SetTrigger("Fire");

        // Shot projectile
        Instantiate(projectile, ProjectorSpawnPos.transform.position, ProjectorSpawnPos.transform.rotation);
        

    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided");
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            anim.ResetTrigger("Jump");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
            
        }
    }


    public void ApplyGravity()
    {
        if(controller.isGrounded == true){
            velocity = -2f;
        }
        velocity += gravity * Time.deltaTime;
        controller.Move(new Vector3(0, velocity, 0) * Time.deltaTime);
        
    }



}
