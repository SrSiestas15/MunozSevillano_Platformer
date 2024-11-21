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
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        playerInput = new Vector2(Input.GetAxis("Horizontal"), 0f);
    }

    private void FixedUpdate()
    {
        groundBellow = Physics2D.Linecast(transform.position, transform.position + Vector3.down, groundLayer);
        //Debug.DrawLine(transform.position, transform.position + Vector3.down);
        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        rb.AddForce(playerInput * speed * 1000 * Time.fixedDeltaTime);
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
