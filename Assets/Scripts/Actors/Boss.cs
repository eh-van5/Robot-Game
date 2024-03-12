using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossMoveState
{
    Idle,
    Following,
    Attacking,
    Moving,
    Teleporting,
}

public class Boss : Actor, IDamageable
{
    public CameraControl cameraControl;

    [SerializeField] private string playerTag = "Player";
    [SerializeField] private GameObject player;

    [SerializeField] protected BossMoveState bossMoveState;

    [Header("Movement")]
    private float yStart;
    public Vector3 followOffset;
    public float smoothTime = 0.25f;
    private Vector3 followVelocity = Vector3.zero;
    public float sideBobMagnitude = 5f; // Amplitude of side swaying during Idle
    public float sideBobSpeed = 1f;     // Speed at which boss sways when Idle
    public float bobMagnitude = 0.5f;   // Amplitude of bob float
    public float bobSpeed = 5f;     // Speed at which boss bobs when floating


    [Header("Combat")]
    [SerializeField] private float maxHitpoints = 100;
    [SerializeField] private float hitpoints = 100;
    [SerializeField] private bool phaseTwo = false;
    [SerializeField] private float phaseTwoThreshold = 50;   // Hitpoints when boss changes to phase two
    [SerializeField] private float phaseChangeTime = 2;
    [SerializeField] protected float timeBetweenAttacks = 5;    // time between the end of one attack and the start of another (in seconds)
    [SerializeField] protected float teleportSpeed = 1;  // Time to disappear and reappear when teleporting (in seconds)
    [SerializeField] protected float timeBetweenTeleport = 0.75f;   // Time in between disappearing and reappearing (in seconds)

    [SerializeField] protected float tentacleStabDelay = 2;     // Time before boss activates tentacle stab attack (in seconds)

    [Header("Testing")]
    public Sprite attackSprite;
    public Color phaseTwoColor;
    public Color indicatorColor;
    public Color attackColor;
    public float timeBeforeAttack = 0.75f;  // Time before attack activates and can inflict damage (in seconds)
    public float attackDuration = 1;    // How long the hitbox is active for after activation (in seconds)

    [SerializeField] private float actionTimer;

    public float Hitpoints { get { return hitpoints; } }

    void Start()
    {
        base.Start();
        yStart = transform.position.y;
        player = GameObject.FindWithTag(playerTag);
        bossMoveState = BossMoveState.Idle;

        Color transparent = sr.color;
        transparent.a = 0;
        sr.color = transparent;

        StartCoroutine(fadeIn(1));
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        switch (bossMoveState)
        {
            case BossMoveState.Idle:
                if (actionTimer >= timeBetweenAttacks)
                {
                    Attack();
                    actionTimer = 0;
                }
                actionTimer += Time.deltaTime;

                // Following the player
                if (player != null)
                {
                    float xOffset = sideBobMagnitude * Mathf.Sin(Time.time * sideBobSpeed);
                    float yOffset = bobMagnitude * Mathf.Abs(Mathf.Cos(Time.time * bobSpeed));
                    Vector3 targetPosition = new Vector3(player.transform.position.x + xOffset, yStart + yOffset, 0);
                    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref followVelocity, smoothTime);
                }
                
                break;

            case BossMoveState.Following:
                // Following the player
                if (player != null)
                {
                    float yOffset = bobMagnitude * Mathf.Sin(Time.time * bobSpeed);
                    Vector3 targetPosition = new Vector3(player.transform.position.x, yStart, 0);
                    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref followVelocity, 0.1f);
                }

                break;

            case BossMoveState.Moving:
                if(player != null)
                {
                    Vector3 targetPosition = new Vector3(player.transform.position.x + 5, player.transform.position.y, 0);
                    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref followVelocity, smoothTime);
                }

                break;
        }
        
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // Chooses an attack at random 
    private void Attack()
    {
        int randInt = (int)Random.Range(0, 3);
        switch (randInt)
        {
            case 0:
                StartCoroutine(GroundRupture());
                break;
            case 1:
                StartCoroutine(TentacleStab());
                break;
            case 2:
                StartCoroutine(Tackle(true));
                break;
        }
        
    }

    protected Vector3 teleportLocation()
    {
        int randInt = (int)Random.Range(0, 2);
        float yPos = player.transform.position.y + 10;
        if (randInt == 0)
        {
            return new Vector3(player.transform.position.x + 10, yPos, 0);
        }
        return new Vector3(player.transform.position.x - 10, yPos, 0);
    }

    protected GameObject CreateAttack(Vector3 position, float xSize, float ySize, float angle, float indicatorTime, float attackDuration, Sprite attackSprite, Color indicatorColor, Color attackColor)
    {
        GameObject attackObject = new GameObject();
        SpriteRenderer attacksr = attackObject.AddComponent<SpriteRenderer>();
        attacksr.sprite = attackSprite;
        BoxCollider2D attackcol = attackObject.AddComponent<BoxCollider2D>();
        NoMoveAttack attack = attackObject.AddComponent<NoMoveAttack>();

        attack.xSize = xSize;
        attack.ySize = ySize;
        attack.angle = angle;
        attack.indicator = indicatorColor;
        attack.attack = attackColor;
        attack.timeBeforeAttack = indicatorTime;
        attack.attackDuration = attackDuration;

        attackObject.transform.position = position;
        return attackObject;
    }

    #region IEnumerators
    IEnumerator changePhase(float transitionSpeed)
    {
        yield return StartCoroutine(startTeleport(Vector3.zero));
        StartCoroutine(cameraControl.Zoom(transform.position, transitionSpeed, 2));


        bossMoveState = BossMoveState.Attacking;

        float colorTimer = 0;
        Color startColor = sr.color;
        while(sr.color != phaseTwoColor)
        {
            colorTimer += Time.deltaTime / transitionSpeed;
            sr.color = Color.Lerp(startColor, phaseTwoColor, colorTimer);
            yield return null;
        }

        sr.color = phaseTwoColor;

        StartCoroutine(cameraControl.Shake(1f, 0.5f));
        yield return new WaitForSeconds(2);

        actionTimer = 0;
        phaseTwo = true;
        sideBobSpeed *= 2;
        bossMoveState = BossMoveState.Idle;
    }

    IEnumerator fadeOut(float fadeSpeed)
    {
        Color color = sr.color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime / (fadeSpeed);
            sr.color = color;

            yield return null;
        }
    }

    IEnumerator fadeIn(float fadeSpeed)
    {
        Color color = sr.color;
        while (color.a <= 1)
        {
            color.a += Time.deltaTime / (fadeSpeed);
            sr.color = color;

            yield return null;
        }
    }

    // Boss disappears briefly moves to new location after completely disappeared
    IEnumerator startTeleport(Vector3 newLocation)
    {
        bossMoveState = BossMoveState.Teleporting;

        yield return StartCoroutine(fadeOut(teleportSpeed));

        transform.position = newLocation;

        yield return new WaitForSeconds(timeBetweenTeleport);

        yield return endTeleport();
    }

    // Boss reappears
    IEnumerator endTeleport()
    {
        Debug.Log("reappearing now");

        yield return StartCoroutine(fadeIn(teleportSpeed));

        bossMoveState = BossMoveState.Idle;
    }

    #endregion

    #region Attacks
    // The boss buries his tentacles under the ground and erupt them creating several lines of hazard from the ground to the top of the map
    IEnumerator GroundRupture()
    {
        yield return startTeleport(teleportLocation());
        bossMoveState = BossMoveState.Attacking;

        for (int x = -21; x < 21; x += 3)
        {
            CreateAttack(new Vector3(player.transform.position.x + x, 1), 1, 20, 0, timeBeforeAttack, attackDuration, attackSprite, indicatorColor, attackColor);
        }

        if (phaseTwo)
        {
            yield return new WaitForSeconds(attackDuration + timeBeforeAttack);

            for (int x = -21; x < 21; x += 3)
            {
                CreateAttack(new Vector3(player.transform.position.x, x * 1.5f + 1), 100, 1, 0, timeBeforeAttack, attackDuration / 3, attackSprite, indicatorColor, attackColor);
            }
        }

        yield return new WaitForSeconds(attackDuration + timeBeforeAttack);
        bossMoveState = BossMoveState.Idle;
    }

    // Boss follows player and fires two tentacles in a cross
    IEnumerator TentacleStab()
    {
        float realSmoothTime = smoothTime;
        smoothTime = 0.1f;
        float realYStart = yStart;
        yStart -= 4;

        bossMoveState = BossMoveState.Following;

        yield return new WaitForSeconds(0.5f);
        bossMoveState = BossMoveState.Attacking;

        if (phaseTwo)
        {
            // Initial slam in the middle
            CreateAttack(new Vector3(transform.position.x, yStart - 1), 3, 10, 0, timeBeforeAttack, attackDuration / 2, attackSprite, indicatorColor, attackColor);

            yield return new WaitForSeconds(attackDuration + timeBeforeAttack);

            // Second ground slam
            CreateAttack(new Vector3(transform.position.x - 6, yStart - 1), 10, 10, 0, timeBeforeAttack, attackDuration / 2, attackSprite, indicatorColor, attackColor);

            // Third ground slam
            CreateAttack(new Vector3(transform.position.x + 6, yStart - 1), 10, 10, 0, timeBeforeAttack, attackDuration / 2, attackSprite, indicatorColor, attackColor);

            yield return new WaitForSeconds(attackDuration + timeBeforeAttack);

        }

        // First tentacle
        CreateAttack(new Vector3(player.transform.position.x, yStart - 1), 2, 15, 55, timeBeforeAttack, attackDuration, attackSprite, indicatorColor, attackColor);

        // Second tentacle
        CreateAttack(new Vector3(player.transform.position.x, yStart - 1), 2, 15, -55, timeBeforeAttack, attackDuration, attackSprite, indicatorColor, attackColor);

        yield return new WaitForSeconds(timeBeforeAttack + attackDuration);

        smoothTime = realSmoothTime;
        yStart = realYStart;
        bossMoveState = BossMoveState.Idle;
        
    }

    // if fromRight is true, then attack starts from the right
    IEnumerator Tackle(bool fromRight)
    {
        float realSmoothTime = smoothTime;
        smoothTime = 0.1f;
        bossMoveState = BossMoveState.Moving;

        yield return new WaitForSeconds(1);

        bossMoveState = BossMoveState.Attacking;

        CreateAttack(new Vector3(player.transform.position.x, transform.position.y), 1, 10, 90, timeBeforeAttack, 0.1f, attackSprite, indicatorColor, attackColor);

        Vector3 endPos;
        if (fromRight)
        {
            endPos = player.transform.position + new Vector3(-5, 0, 0);
        }
        else
        {
            endPos = player.transform.position + new Vector3(5, 0, 0);
        }

        yield return new WaitForSeconds(timeBeforeAttack);

        float t = 0;
        Vector3 startPos = transform.position;
        while (t < 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime / (timeBeforeAttack / 5);
            yield return null;
        }
        transform.position = endPos;

        yield return new WaitForSeconds(0.1f);

        smoothTime = realSmoothTime;
        bossMoveState = BossMoveState.Idle;
    }


    #endregion

    #region IDamageable
    public void Damage(float damageAmount, Vector2 knockback)
    {
        hitpoints -= damageAmount;
        Flash();
        Debug.Log("hitpoints: " + hitpoints);
        if(!phaseTwo && hitpoints <= phaseTwoThreshold)
        {
            StartCoroutine(changePhase(phaseChangeTime));
        }
        if (hitpoints <= 0)
        {
            Death();
        }
    }

    public void Heal(float healAmount)
    {
        hitpoints += healAmount;
        if (hitpoints >= maxHitpoints)
        {
            hitpoints = maxHitpoints;
        }
    }

    public void Death()
    {
        Debug.Log("Boss defeated");
        Destroy(gameObject);
    }
    #endregion
}
