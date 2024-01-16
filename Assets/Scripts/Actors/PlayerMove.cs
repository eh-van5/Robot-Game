using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : Actor, IDamageable
{
    public CameraControl cameraControl;

    [Header("Dashing")]
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashTime = 0.2f; //How long player stays in dash
    [SerializeField] private float dashFalloffMultiplier = 5f;
    [SerializeField] private float timeBeforeDash = 5f; //Seconds before player can dash again
    [SerializeField] private bool canDash;
    private float currentDashSpeed;
    private float doubleClickThreshhold = 0.2f; //Time interval where double clicks will register
    private bool leftMoveClick;
    private bool rightMoveClick;
    private float lastClickTime;

    [Header("Combat")]
    [SerializeField] private float maxHitpoints = 100;
    [SerializeField] private float hitpoints = 100;

    public Vector3 Position { get { return transform.position; } }
    public float Hitpoints { get { return hitpoints; } set { hitpoints = value; } }

    void Awake()
    {
        canDash = true;
        leftMoveClick = false;
        rightMoveClick = false;
        hitpoints = maxHitpoints;
    }

    void Update()
    {
        base.Update();
        InputHandler();
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
        Movement();
    }

    private void InputHandler()
    {
        switch (moveState)
        {
            case MoveState.Default:
                #region Horizontal Input
                if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
                {
                    Stop();
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    MoveLeft();
                    if (!leftMoveClick)
                    {
                        lastClickTime = Time.time;
                        leftMoveClick = true;
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            if(Time.time - lastClickTime <= doubleClickThreshhold && canDash)
                            {
                                Dash();
                                leftMoveClick = false;
                            }
                            else
                            {
                                lastClickTime = Time.time;
                            }
                        }
                    }
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    MoveRight();
                    if (!rightMoveClick)
                    {
                        lastClickTime = Time.time;
                        rightMoveClick = true;
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.D))
                        {
                            if (Time.time - lastClickTime <= doubleClickThreshhold && canDash)
                            {
                                Dash();
                                rightMoveClick = false;
                            }
                            else
                            {
                                lastClickTime = Time.time;
                            }
                        }
                    }
                }
                else
                {
                    Stop();
                }

                #endregion
                #region Jumping
                if (!crouching && Input.GetKeyDown(KeyCode.Space) && (isGrounded() || doubleJump))
                {
                    Jump();
                }
                #endregion
                #region Crouching
                crouching = Input.GetKey(KeyCode.S);
                #endregion
                break;

        }
        
    }

    private void Movement()
    {
        switch (moveState)
        {
            case MoveState.Dashing:
                #region Dashing
                rb.velocity = new Vector2(moveDir.x * currentDashSpeed, 0);
                currentDashSpeed -= dashSpeed * dashFalloffMultiplier * Time.deltaTime;
                #endregion
                break;
        }

    }

    public void Damage(float damageAmount)
    {
        StartCoroutine(cameraControl.Shake(0.2f, 0.5f));
        hitpoints -= damageAmount;
        if (hitpoints <= 0)
        {
            Death();
        }
    }

    public void Heal(float healAmount)
    {
        hitpoints += healAmount;
        if(hitpoints >= maxHitpoints)
        {
            hitpoints = maxHitpoints;
        }
    }

    public void Death()
    {
        Debug.Log("You died");
        Destroy(gameObject);
    }

    private void Dash()
    {
        StartCoroutine(dashMoveTimer());
        StartCoroutine(dashTimer());
    }

    IEnumerator dashMoveTimer()
    {
        moveState = MoveState.Dashing;
        currentDashSpeed = dashSpeed;
        moveDir.y = 0; //Stops vertical movement after dash ends

        yield return new WaitForSeconds(dashTime);

        moveState = MoveState.Default;
    }
    IEnumerator dashTimer()
    {
        canDash = false;

        yield return new WaitForSeconds(timeBeforeDash);

        canDash = true;
    }

}
