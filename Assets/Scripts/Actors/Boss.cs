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

public class Boss : Actor
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private GameObject player;
    protected SpriteRenderer sr;

    [SerializeField] protected BossMoveState bossMoveState;

    [Header("Movement")]
    private float yStart;
    public Vector3 followOffset;
    public float smoothTime = 0.25f;
    private Vector3 followVelocity = Vector3.zero;
    public float bobMagnitude = 0.5f;   // Amplitude of bob float
    public float bobSpeed = 5f;     // Speed at which boss bobs when floating


    [Header("Combat")]
    [SerializeField] protected float timeBetweenAttacks = 5;    // time between the end of one attack and the start of another (in seconds)
    [SerializeField] protected float teleportSpeed = 1;  // Time to disappear and reappear when teleporting (in seconds)
    [SerializeField] protected float timeBetweenTeleport = 0.75f;   // Time in between disappearing and reappearing (in seconds)

    [SerializeField] protected float tentacleStabDelay = 2;     // Time before boss activates tentacle stab attack (in seconds)

    [Header("Testing")]
    public Sprite attackSprite;
    public Color indicatorColor;
    public Color attackColor;
    public float timeBeforeAttack = 0.75f;  // Time before attack activates and can inflict damage (in seconds)
    public float attackDuration = 1;    // How long the hitbox is active for after activation (in seconds)

    [SerializeField] private float actionTimer;

    void Start()
    {
        base.Start();
        yStart = transform.position.y;
        player = GameObject.FindWithTag(playerTag);
        sr = GetComponent<SpriteRenderer>();
        bossMoveState = BossMoveState.Idle;

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
                    float yOffset = bobMagnitude * Mathf.Sin(Time.time * bobSpeed);
                    Vector3 targetPosition = new Vector3(player.transform.position.x, yStart + yOffset, 0);
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
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //StopAllCoroutines();
            //StartCoroutine(TentacleStab());
            //StartCoroutine(GroundRupture());
            //Teleport(teleportLocation());
            //if(bossMoveState == BossMoveState.Idle)
            //{
            //    bossMoveState = BossMoveState.Moving;
            //}
            //else if(bossMoveState == BossMoveState.Moving)
            //{
            //    bossMoveState = BossMoveState.Idle;
            //}
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
                StartCoroutine(Tackle());
                break;
        }
        
    }

    protected Vector3 teleportLocation()
    {
        int randInt = (int) Random.Range(0, 2);
        float yPos = player.transform.position.y + 10;
        if (randInt == 0)
        {
            return new Vector3(player.transform.position.x + 10, yPos, 0);
        }
        return new Vector3(player.transform.position.x - 10, yPos, 0);
    }

    // Boss disappears briefly moves to new location after completely disappeared
    IEnumerator startTeleport(Vector3 newLocation)
    {
        bossMoveState = BossMoveState.Teleporting;

        Color color = sr.color;
        while(color.a > 0)
        {
            color.a -= Time.deltaTime / (teleportSpeed);
            sr.color = color;

            yield return null;
        }

        transform.position = newLocation;

        yield return new WaitForSeconds(timeBetweenTeleport);

        yield return endTeleport();
    }

    // Boss reappears
    IEnumerator endTeleport()
    {
        Debug.Log("reappearing now");

        Color color = sr.color;
        while (color.a <= 1)
        {
            color.a += Time.deltaTime / (teleportSpeed);
            sr.color = color;

            yield return null;
        }

        bossMoveState = BossMoveState.Idle;
    }


    #region Attacks
    // The boss buries his tentacles under the ground and erupt them creating several lines of hazard from the ground to the top of the map
    IEnumerator GroundRupture()
    {
        yield return startTeleport(teleportLocation());
        bossMoveState = BossMoveState.Attacking;
        for (int x = -21; x < 21; x += 3)
        {
            GameObject rupture = new GameObject();
            SpriteRenderer rupturesr = rupture.AddComponent<SpriteRenderer>();
            rupturesr.sprite = attackSprite;
            BoxCollider2D rupturecol = rupture.AddComponent<BoxCollider2D>();
            NoMoveAttack attack = rupture.AddComponent<NoMoveAttack>();

            attack.xSize = 1;
            attack.ySize = 20;
            attack.indicator = indicatorColor;
            attack.attack = attackColor;
            attack.timeBeforeAttack = timeBeforeAttack;
            attack.attackDuration = attackDuration;

            rupture.transform.position = new Vector3(player.transform.position.x + x, -9 + (attack.ySize / 2));
        }

        yield return new WaitForSeconds(attackDuration);
        bossMoveState = BossMoveState.Idle;
    }

    // Boss follows player and fires two tentacles in a cross
    IEnumerator TentacleStab()
    {
        float realSmoothTime = smoothTime;
        smoothTime = 0.1f;
        float realYStart = yStart;
        yStart -= 3;

        bossMoveState = BossMoveState.Following;

        yield return new WaitForSeconds(0.5f);
        bossMoveState = BossMoveState.Attacking;

        // First tentacle
        GameObject tentacle1 = new GameObject();
        tentacle1.transform.position = new Vector3(player.transform.position.x, yStart - 2);
        SpriteRenderer rupturesr1 = tentacle1.AddComponent<SpriteRenderer>();
        rupturesr1.sprite = attackSprite;
        BoxCollider2D rupturecol1 = tentacle1.AddComponent<BoxCollider2D>();
        NoMoveAttack attack1 = tentacle1.AddComponent<NoMoveAttack>();

        attack1.xSize = 2;
        attack1.ySize = 15;
        attack1.angle = 55;
        attack1.indicator = indicatorColor;
        attack1.attack = attackColor;
        attack1.timeBeforeAttack = timeBeforeAttack;
        attack1.attackDuration = attackDuration;

        // Second tentacle
        GameObject tentacle2 = new GameObject();
        tentacle2.transform.position = new Vector3(player.transform.position.x, yStart - 2);
        SpriteRenderer rupturesr2 = tentacle2.AddComponent<SpriteRenderer>();
        rupturesr2.sprite = attackSprite;
        BoxCollider2D rupturecol2 = tentacle2.AddComponent<BoxCollider2D>();
        NoMoveAttack attack2 = tentacle2.AddComponent<NoMoveAttack>();

        attack2.xSize = 2;
        attack2.ySize = 15;
        attack2.angle = -55;
        attack2.indicator = indicatorColor;
        attack2.attack = attackColor;
        attack2.timeBeforeAttack = timeBeforeAttack;
        attack2.attackDuration = attackDuration;

        yield return new WaitForSeconds(timeBeforeAttack + attackDuration);

        smoothTime = realSmoothTime;
        yStart = realYStart;
        bossMoveState = BossMoveState.Idle;
        
    }

    IEnumerator Tackle()
    {
        float realSmoothTime = smoothTime;
        smoothTime = 0.1f;
        bossMoveState = BossMoveState.Moving;

        yield return new WaitForSeconds(1);

        bossMoveState = BossMoveState.Attacking;

        GameObject tackle = new GameObject();
        SpriteRenderer tacklesr = tackle.AddComponent<SpriteRenderer>();
        tacklesr.sprite = attackSprite;
        BoxCollider2D tacklecol = tackle.AddComponent<BoxCollider2D>();
        NoMoveAttack attack = tackle.AddComponent<NoMoveAttack>();

        attack.xSize = 1;
        attack.ySize = 10;
        attack.angle = 90;
        attack.indicator = indicatorColor;
        attack.attack = attackColor;
        attack.timeBeforeAttack = timeBeforeAttack;
        attack.attackDuration = 0.1f;

        tackle.transform.position = new Vector3(player.transform.position.x, transform.position.y);
        Vector3 endPos = player.transform.position + new Vector3(-5, 0, 0);

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
}
