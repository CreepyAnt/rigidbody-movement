// SUPER MESSY RIGIDBODY BASED MOVEMENT BY ME, ANT!
// ALSO, A LITTLE-BIT OF CODE IS STOLEN FROM DANI AND PLAI :)

// THE CODE ISN'T FULLY OPTIMIZED SO IF THERE IS ANY PROBLEM, ITS BECAUSE I WAS TOO LAZY TO FIX THAT 
// FEEL FREE TO CONTRIBUTE TO THE CODE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float playerHeight = 2f;
    
    [Header("Movement")]
    public float moveSpeed = 15;
    public float jumpForce = 7;
    public float slideForce = 800;
    float horizontalMove;
    float verticalMove;

    [Header("Multipliers")]
    public float movementMultiplier = 10f;
    public float airMultiplier = 0.4f;
    
    [Header("Drag")]
    public float groundDrag = 6f;
    public float airDrag = 2f;
    public float crouchDrag = 1f;

    [Header("Checks")]
    public Transform groundcheck;
    public Transform ceilingcheck;

    [Header("assignables")]
    public LayerMask whatisGround;
    public Transform orientation;
    
    [Header("Bools")]
    public bool isGrounded;
    public bool headButting;
    public bool isCrouching;
    

    Vector3 playerScale;
    Vector3 crouchScale = new Vector3 (1f,0.5f,1f);

    RaycastHit slopeHit;
    bool onSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
        }else
        {
            return false;
        }
        return false;
    }
    
    Rigidbody rb;

    Vector3 moveDirection;
    Vector3 slopeDirection;
   
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        rb.freezeRotation = true;
        playerScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {   
        isGrounded = Physics.CheckSphere(groundcheck.position, 0.2f, whatisGround);
        headButting = Physics.Raycast(ceilingcheck.position, Vector3.up, 1f);
        input();

        if (transform.position.y < -25)
        {
            transform.position = new Vector3(0,15,0);
        }

        slopeDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
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
            isCrouching = true;
            
        }else if (Input.GetButtonUp("Crouch") && !headButting)
        {
            StopCrouch();
            isCrouching = false;

        }
    }

    
    void movement()
    {
        if (isGrounded && !isCrouching && !onSlope())
        {
            rb.AddForce(moveDirection * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        
        }else if (isGrounded && onSlope())
        {
            rb.AddForce(slopeDirection * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        
        }else if (!isGrounded)
        {
            rb.AddForce(moveDirection * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }

        if (onSlope() && isCrouching)
        {
            rb.AddForce(Vector3.down * 300);
        }
    }
            
        

    void jump()
    {   
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);    
    }

   void startCrouch()
    {
        
        transform.localScale = crouchScale;
        transform.position = new Vector3 (transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 10 && isGrounded)
        {
            rb.AddForce(orientation.transform.forward * slideForce * 10);
        }
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
        }else if (isCrouching)
        {
            rb.drag = crouchDrag;
        }else if (!isGrounded)
        {
            rb.drag = airDrag;
        }
    }
}

