using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    Vector2 playerInput;
  
    FacingDirection lastDirection;

    bool groundBellow;
    public LayerMask groundLayer;

    float gravity;
    public float apexHeight;
    public float apexTime;

    public float maxSpeed;
    public float timeToReachMaxSpeed;
    public float timeToDecelerate;
    private float initialJumpVelocity;
    private float acceleration;
    private float deceleration;

    public float terminalSpeed;
    bool coyoteJumpPossible = true;
    public float coyoteTime = 0;


    private bool didWeJump = false;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        gravity = -2 * apexHeight / Mathf.Pow(apexTime, 2);
        initialJumpVelocity = 2 * apexHeight / apexTime;
        acceleration = maxSpeed / timeToReachMaxSpeed;
        deceleration = maxSpeed / timeToDecelerate;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        playerInput = new Vector2(Input.GetAxis("Horizontal"), 0f);
        if ((Input.GetKeyDown(KeyCode.UpArrow) && groundBellow) || (Input.GetKeyDown(KeyCode.UpArrow) && coyoteJumpPossible))
        {
            coyoteJumpPossible = false;
            didWeJump = true;
        }
        Debug.DrawLine(transform.position, transform.position + Vector3.down * .70f);

        if (!groundBellow)
        {
            coyoteTime += Time.deltaTime;
            if (coyoteTime < .1)
            {
                coyoteJumpPossible = true;
            }
            else coyoteJumpPossible = false;
        }
        else
        {
            coyoteTime = 0;
            coyoteJumpPossible = true;
        }
    }

    private void FixedUpdate()
    {
        groundBellow = Physics2D.Linecast(transform.position, transform.position + Vector3.down * .70f, groundLayer);
        //rb.AddForce(Vector2.up * gravity);
        //Debug.DrawLine(transform.position, transform.position + Vector3.down);
        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {

        Vector2 currentVelocity = rb.velocity;

        //accelerating
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            currentVelocity += Vector2.left * Time.deltaTime * acceleration;
        }
       
        if (Input.GetKey(KeyCode.RightArrow))
        {
            currentVelocity += Vector2.right * Time.deltaTime * acceleration;
        }

        //decelerating
        if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            currentVelocity += Vector2.ClampMagnitude(Vector2.left * Time.deltaTime * deceleration * MathF.Sign(rb.velocity.x), rb.velocity.magnitude);
        }

        //clamping max speed
        currentVelocity.x = Mathf.Clamp(currentVelocity.x, -maxSpeed, maxSpeed);
        currentVelocity.y = Mathf.Clamp(currentVelocity.y, -terminalSpeed, terminalSpeed);
        rb.velocity = currentVelocity;

        //jump trigger
        if (didWeJump)
        {
            //jump logic
            //apex height and apex time

            rb.velocity += Vector2.up * initialJumpVelocity;
            didWeJump = false;
        }
        
        //gravity
        rb.velocity += Vector2.up * Time.deltaTime * gravity;

    }

    void Jump()
    {

    }

    public bool IsWalking()
    {
        if (Mathf.Abs(playerInput.x) > 0)
        {
            return true;
        } else return false;
    }
    public bool IsGrounded()
    {
        //Debug.Log("touching ground");
        return groundBellow;
    }

    public FacingDirection GetFacingDirection()
    {
        if (playerInput.x > 0)
        {
            lastDirection = FacingDirection.right;
            return FacingDirection.right;
        }
        else if (playerInput.x < 0)
        {
            lastDirection = FacingDirection.left;
            return FacingDirection.left;
        }
        else return lastDirection;
    }
}
