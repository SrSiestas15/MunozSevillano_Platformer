using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum facingDirection
    {
        left, right
    }
    public facingDirection currentFacingDirection = facingDirection.left;

    public enum CharacterState
    {
        idle, walk, jump, die
    }
    public CharacterState currentState = CharacterState.idle;
    public CharacterState previousState = CharacterState.idle;


    Rigidbody2D rb;
    public float speed;
    Vector2 playerInput;
  
    FacingDirection lastDirection;

    //used to check linecast with the ground and bouncepads
    public LayerMask groundLayer;
    public LayerMask bouncepadLayer;

    //used to calculate player physics
    float gravity;
    public float apexHeight;
    public float apexTime;

    public float maxSpeed;
    public float dashSpeed;
    public float timeToReachMaxSpeed;
    public float timeToDecelerate;
    private float initialJumpVelocity;
    private float acceleration;
    private float deceleration;

    public float terminalSpeed;

    //used to keep track of jump calculations
    bool groundBellow;
    private bool didWeJump = false;
    private bool didWeWalljump = false;
    private bool isDashing = false;
    public float dashTimer = 0;
    bool coyoteJumpPossible = true;
    public float coyoteTime = 0;

    //used to bugtest "dying"
    public int currentHealth = 10;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //physics calculated based on current value of MaxSpeed (changes often)
        gravity = -2 * apexHeight / Mathf.Pow(apexTime, 2);
        initialJumpVelocity = 2 * apexHeight / apexTime;
        acceleration = maxSpeed / timeToReachMaxSpeed;
        deceleration = maxSpeed / timeToDecelerate;

        previousState = currentState;

        //runs if player presses Dash and is not already Dashing
        if (isDashing)
        {
            //begins timer to end dash
            dashTimer += Time.deltaTime;
            if (dashTimer >= .2)
            {
                //ends dash, resets maxSpeed
                maxSpeed -= dashSpeed;
                isDashing = false;
            }
        }
        else dashTimer = 0;
            
        if (IsDead())
        {
            currentState = CharacterState.die;
        }

        switch (currentState)
        {
            case CharacterState.idle:
                if (IsWalking())
                {
                    currentState = CharacterState.walk;
                }
                if (!IsGrounded())
                {
                    currentState = CharacterState.jump;
                }
                break;
            case CharacterState.walk:
                if (!IsWalking())
                {
                    currentState = CharacterState.idle;
                }
                if (!IsGrounded())
                {
                    currentState = CharacterState.jump;
                }
                break;
            case CharacterState.jump:
                if (IsGrounded())
                {
                    if (IsWalking())
                    {
                        currentState = CharacterState.walk;
                    }
                    else currentState = CharacterState.idle;
                }
                break;
            case CharacterState.die:
                
                break;
        }

        
        playerInput = new Vector2(Input.GetAxis("Horizontal"), 0f);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentFacingDirection = facingDirection.left;
        } else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentFacingDirection = facingDirection.right;
        }

        if ((Input.GetKeyDown(KeyCode.UpArrow) && groundBellow) || (Input.GetKeyDown(KeyCode.UpArrow) && coyoteJumpPossible))
        {
            coyoteJumpPossible = false;
            didWeJump = true;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && !groundBellow)
        {
            didWeWalljump = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isDashing)
        {
            JumpAndDirection(Vector2.right * 10 * MathF.Sign(rb.velocity.x));
            maxSpeed += dashSpeed;
            isDashing = true;
        }

        if (!groundBellow)
        {
            coyoteTime += Time.deltaTime;
            if (coyoteTime < .1 && !didWeJump)
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
        
        //jump if on bouncepad
        if(Physics2D.Linecast(transform.position, transform.position + Vector3.down * .70f, bouncepadLayer))
        {
            JumpAndDirection(Vector2.up);
        }
            Debug.DrawLine(transform.position, transform.position + Vector3.left * .55f);
        
        //walljump calculations
        if (didWeWalljump)
        {
            if (Physics2D.Linecast(transform.position, transform.position + Vector3.left * .55f, groundLayer))
            {
                JumpAndDirection(new Vector2(.3f, .7f));
            }
            else if (Physics2D.Linecast(transform.position, transform.position + Vector3.right * .55f, groundLayer))
            {
                JumpAndDirection(new Vector2(-.3f, .7f));
            }
            else didWeWalljump = false;
        }

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
            JumpAndDirection(Vector2.up);
            didWeJump = false;
        }
        
        //gravity
        rb.velocity += Vector2.up * Time.deltaTime * gravity;

    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public void OnDeathAnimationComplete()
    {
        gameObject.SetActive(false);
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

    void JumpAndDirection(Vector2 jumpDirection)
    {
        rb.velocity += jumpDirection * initialJumpVelocity;
    }
}
