using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor, IDamageable

{

    public CameraControl cameraControl;

    private PlayerCombat pCombat;

    [Header("Debugging")]
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

    public float Hitpoints { get { return stats.currentHP; } }



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
        base.Start();
        pCombat = GetComponent<PlayerCombat>();
        stats.currentHP = stats.maxHP;
    }

    protected void Update()
    {
        #region Movement Input
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
        if (numJumps < stats.bonusJump)
        {
            Jump();
        }
        if (isGrounded())
        {
            numJumps = 0;
        }
        #endregion
        #region Combat Input
        if (Input.GetKeyDown(KeyCode.Mouse0) && pCombat.canAttack)
        {
            Attack();
        }
        #endregion
    }

    protected void FixedUpdate()
    {

        #region Horizontal Movement
        if (moveDir.x > 0.1f || moveDir.x < -0.1f)
        {
            float moveSpeed = stats.speed;
            rb.velocity = new Vector2(moveDir.x * moveSpeed, rb.velocity.y);
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
            rb.AddForce(new Vector2(0f, Physics2D.gravity.y * (stats.fallingGravityScale - 1)));
        }
        else if (rb.velocity.y > 0 && !jumpButton)
        {
            rb.AddForce(new Vector2(0f, Physics2D.gravity.y * (stats.lowJumpGravityScale - 1)));

        }
        if (moveDir.y < -0.1f)
        {
            rb.AddForce(new Vector2(0f, -9.81f * stats.slamScale), ForceMode2D.Impulse);
        }

        #endregion
        #endregion
        vel = rb.velocity;
        #region Dashing
        if (dash)
        {
            if (moveDir.x == 0f)
            {
                rb.AddForce(new Vector2(stats.dashSpeed, 0f), ForceMode2D.Impulse);

            }
            else
            {
                rb.AddForce(new Vector2(moveDir.x * stats.dashSpeed, 0f), ForceMode2D.Impulse);
            }
        }
        #endregion

    }

    private void Attack()
    {
        //hitsGO = hits.Where(x => x.collider != null).Select(x => x.collider.gameObject).ToArray();
        StartCoroutine(pCombat.AttackTimer());
    }
    protected void Jump()
    {
        if (jumpPressed)
        {
            isJumping = true;
            jumpTime = jumpStartTime;
            rb.velocity = Vector2.up * stats.jumpForce;
            numJumps++;

        }
    }

    #region IDamageable
    public void Damage(float damageAmount, Vector2 knockback)
    {
        stats.currentHP -= damageAmount;
        Flash();
        StartCoroutine(cameraControl.Shake(0.2f, 0.5f));
        if (stats.currentHP <= 0)
        {
            Death();
        }
    }

    public void Heal(float healAmount)
    {
        stats.currentHP += healAmount;
        if (stats.currentHP >= stats.maxHP)
        {
            stats.currentHP = stats.maxHP;
        }
    }

    public void Death()
    {
        Debug.Log("You died.");
        Destroy(gameObject);
    }
    #endregion
}