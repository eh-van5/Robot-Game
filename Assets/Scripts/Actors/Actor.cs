using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveState
{
    Default,
    Dashing
}

public class Actor : MonoBehaviour
{
    public ActorStats stats;
    [SerializeField] protected GameObject actor;
    protected Rigidbody2D rb;
    protected BoxCollider2D col;

    [SerializeField] protected MoveState moveState;

    [Header("Movement")]
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected float jumpForce = 12f;
    [SerializeField] protected float gravity = 20f;
    [SerializeField] protected float crouchGravity = 30f;
    [SerializeField] protected bool doubleJump;
    protected bool crouching = false;

    [Header("Ground Check")]
    [SerializeField] protected LayerMask groundLayer;
    protected float groundCheckDistance = 0.1f;

    [Header("Debugging")]
    [SerializeField] protected Vector2 moveDir; //Displays actor velocity
    [SerializeField] protected bool headTouched; //Made so gravity is set to 0 only once when head touches ceiling

    protected void Start()
    {
        actor = gameObject;
        rb = actor.GetComponent<Rigidbody2D>();
        col = actor.GetComponent<BoxCollider2D>();
        moveState = MoveState.Default;

        headTouched = isHeadTouch();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

    }

    protected void Update()
    {
        if (isGrounded())
        {
            doubleJump = true;
        }
    }

    protected void FixedUpdate()
    {
        switch (moveState)
        {
            case MoveState.Default:
                #region Horizontal Movement
                float moveSpeed = crouching ? (speed / 3) : speed;
                rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y);
                #endregion
                #region Ground + Head Check
                if (!isGrounded() && !crouching)
                {
                    moveDir.y -= gravity * Time.deltaTime;
                }
                if (isHeadTouch() && !headTouched)
                {
                    headTouched = true;
                    moveDir.y = 0;
                }
                else if (!isHeadTouch())
                {
                    headTouched = false;
                }
                #endregion
                #region Crouching
                if (crouching)
                {
                    moveDir.y = -crouchGravity;
                    doubleJump = false;
                }
                #endregion
                break;
        }
    }

    //protected void MoveLeft()
    //{
    //    moveDir.x = -1;
    //}

    //protected void MoveRight()
    //{
    //    moveDir.x = 1;
    //}
    //protected void Stop()
    //{
    //    moveDir.x = 0;
    //}

    //protected void Jump()
    //{
    //    moveDir.y = jumpForce;
    //    doubleJump = false;
    //}


    #region RayCast Checks
    protected bool isGrounded()
    {

        Color rayColor;
        RaycastHit2D boxCast = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, groundCheckDistance, groundLayer);

        if (boxCast.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(col.bounds.center + new Vector3(col.bounds.extents.x, 0), Vector2.down * (col.bounds.extents.y + groundCheckDistance), rayColor);
        Debug.DrawRay(col.bounds.center - new Vector3(col.bounds.extents.x, 0), Vector2.down * (col.bounds.extents.y + groundCheckDistance), rayColor);
        Debug.DrawRay(col.bounds.center - new Vector3(col.bounds.extents.x, col.bounds.extents.y + groundCheckDistance), Vector2.right * col.bounds.size.x, rayColor);

        return boxCast.collider != null;
    }

    protected bool isHeadTouch()
    {
        Color rayColor;
        RaycastHit2D boxCast = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.up, groundCheckDistance, groundLayer);

        if (boxCast.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(col.bounds.center + new Vector3(col.bounds.extents.x, 0), Vector2.up * (col.bounds.extents.y + groundCheckDistance), rayColor);
        Debug.DrawRay(col.bounds.center - new Vector3(col.bounds.extents.x, 0), Vector2.up * (col.bounds.extents.y + groundCheckDistance), rayColor);
        Debug.DrawRay(col.bounds.center + new Vector3(col.bounds.extents.x, col.bounds.extents.y + groundCheckDistance), Vector2.left * col.bounds.size.x, rayColor);

        return boxCast.collider != null;
    }

    #endregion
}
