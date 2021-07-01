// SUPER MESSY RIGIDBODY BASED MOVEMENT BY ME, ANT!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float moveSpeed = 15;
    public float jumpForce = 7;
    float horizontalMove;
    float verticalMove;

    public bool isGrounded;
    public bool headButting;
    
    public LayerMask whatisGround;
    public Transform groundcheck;
    public Transform ceilingcheck;
    public float airMultiplier = 0.4f;


    public Transform orientation;

    public float groundDrag = 6f;
    public float airDrag = 2f;
    public float movementMultiplier = 10f;

    Vector3 playerScale;
    Vector3 crouchScale = new Vector3 (1f,0.5f,1f);
    

    Rigidbody rb;

    Vector3 moveDirection;
   
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    
    void Start()
    {
        playerScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {   
        isGrounded = Physics.CheckSphere(groundcheck.position, 0.2f, whatisGround);
        headButting = Physics.Raycast(ceilingcheck.position, Vector3.up,2.25f, whatisGround);
        input();
    }

    void FixedUpdate()
    {   
        controlDrag();
        movement();

    }

    void input()
    {   
        // WASD Movement
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        
        // direction to apply force
        moveDirection = orientation.forward * verticalMove + orientation.right * horizontalMove;

        // jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jump();
        }
        
        if (Input.GetButtonDown("Crouch"))
        {
            startCrouch();
        }else if (Input.GetButtonUp("Crouch") && !headButting)
        {
            StopCrouch();
        }
    }

    // Methods
    
    void movement()
    {
        if (isGrounded)
        {
            rb.AddForce(moveDirection * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }else if (!isGrounded)
        {
            rb.AddForce(moveDirection * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }
            
        

    void jump()
    { 
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
    }


    void startCrouch()
    {
        transform.localScale = crouchScale;
        transform.position = new Vector3 (transform.position.x, transform.position.y - 0.5f, transform.position.z);
    }
    void StopCrouch()
    {
        transform.localScale = playerScale;
    }
    
    void controlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }else
        {
            rb.drag = airDrag;
        }
    }

}
