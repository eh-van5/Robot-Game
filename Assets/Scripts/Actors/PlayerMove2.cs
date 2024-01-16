using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class PlayerMove2 : MonoBehaviour

{
    GameObject player;
    public Actor2 actor;
    protected Rigidbody2D rb;
    [Header("Debugging")]
    [SerializeField] protected Vector2 moveDir; //Displays actor velocity
    [SerializeField] protected Vector2 vel; //Displays actor velocity
    [SerializeField] protected bool space;
    [SerializeField] protected bool ran;
    [SerializeField] protected bool isGrounded;
    [SerializeField] protected bool jumpPressed;
    [SerializeField] protected bool jumpLetGo;
    [SerializeField] protected bool jumpButton;
    [SerializeField] protected bool isJumping;
    [SerializeField] protected float jumpTime;
    [SerializeField] protected float jumpStartTime = 10f;
    [SerializeField] protected bool dash;
    [SerializeField] protected int numJumps = 0;





    /*    private float currentDashSpeed;
        private float doubleClickThreshhold = 0.2f; //Time interval where double clicks will register
        private bool leftMoveClick;
        private bool rightMoveClick;
        private float lastClickTime;*/


    /* void Awake()
     {
         canDash = true;
         leftMoveClick = false;
         rightMoveClick = false;
     }*/

    void Start()
    {
        player = gameObject;
        rb = player.GetComponent<Rigidbody2D>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    protected void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            space = true;
        }
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.y = Input.GetAxisRaw("Vertical");
        jumpPressed = Input.GetButtonDown("Jump");
        jumpButton = Input.GetButton("Jump");
        jumpLetGo = Input.GetButtonUp("Jump");
        dash = Input.GetKey(KeyCode.C);
        if (numJumps < actor.bonusJump)
        {
            Jump();
        }
        if (isGrounded)
        {
            numJumps = 0;
        }



    }

    protected void FixedUpdate()
    {

        #region Horizontal Movement
        if (moveDir.x > 0.1f || moveDir.x < -0.1f)
        {
            float moveSpeed = actor.speed;
            rb.velocity = new Vector2(moveDir.x * moveSpeed , rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        #endregion
        #region Jump
/*        Jump();
*/
        #region Falling
        if (rb.velocity.y < 0)
        {
            rb.AddForce(new Vector2(0f, Physics2D.gravity.y * (actor.fallingGravityScale - 1)));
        }
        else if (rb.velocity.y > 0 && !jumpButton){
            rb.AddForce(new Vector2(0f, Physics2D.gravity.y * (actor.lowJumpGravityScale - 1)));

        }
        if (moveDir.y < -0.1f)
        {
            rb.AddForce(new Vector2(0f, -9.81f * actor.slamScale), ForceMode2D.Impulse);
        }
        
        #endregion
        #endregion
        vel = rb.velocity;
        #region Dashing
        if(dash)
        {
            if (moveDir.x == 0f)
            {
                rb.AddForce(new Vector2(actor.dashSpeed, 0f), ForceMode2D.Impulse);

            }
            else
            {
                rb.AddForce(new Vector2(moveDir.x * actor.dashSpeed, 0f), ForceMode2D.Impulse);
            }
        }
        #endregion

    }



    protected void Jump()
    {
        if (jumpPressed)
        {
            isJumping = true;
            jumpTime = jumpStartTime;
            rb.velocity = Vector2.up * actor.jumpForce;
            numJumps++;

        }
     

        /* if (jumpButton && isJumping == true)
         {
             if (jumpTime > 0)
             {
                 rb.velocity = Vector2.up * actor.jumpForce;

                 jumpTime -= Time.deltaTime;
             }
             else
             {
                 isJumping = false;

             }
         }

         if (jumpLetGo)
         {
             isJumping = false;

         }*/
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Surface")
        {
            isGrounded = true;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Surface")
        {
            isGrounded = false;
        }
    }



    /* private void InputHandler()
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
                             if (Time.time - lastClickTime <= doubleClickThreshhold && canDash)
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

     }*/

    /*  private void Movement()
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

      }*/

   /* public void Damage(float damageAmount)
    {
        hitpoints -= damageAmount;
    }

    public void Heal(float healAmount)
    {
        hitpoints += healAmount;
    }*/

   /* private void Dash()
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
    }*/

}