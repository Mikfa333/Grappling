using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed;

    public float GroundDrag;

    public float JumpForce;
    public float JumpCooldown;
    public float AirMultiplier;
    bool ReadyToJump;

    [Header("Keybinds")]
    public KeyCode JumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float PlayerHeight;
    public LayerMask WhatIsGround;
    bool Grounded;

    public Transform Orientation;

    float HorizontalInput;
    float VerticalInput;

    Vector3 MoveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        //ground check
        Grounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, WhatIsGround);

        MyInput();
        SpeedControl();

        //handle drag
        if (Grounded)
            rb.drag = GroundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");

        //when to jump
        if(Input.GetKey(JumpKey) && ReadyToJump && Grounded)
        {
            ReadyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), JumpCooldown);
        }
    }

    private void MovePlayer()
    {
        //calculate movement direction
        MoveDirection = Orientation.forward * VerticalInput + Orientation.right * HorizontalInput;

        //On ground
        if(Grounded)
            rb.AddForce(MoveDirection.normalized * MoveSpeed * 10f, ForceMode.Force);

        //In air
        else if(!Grounded)
            rb.AddForce(MoveDirection.normalized * MoveSpeed * 10f * AirMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        //limit velocity if needed
        if(flatVel.magnitude > MoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * MoveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        //rest y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);

    }
        
    private void ResetJump()
    {
        ReadyToJump = true;
    }

    
}
