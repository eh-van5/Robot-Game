using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Behavior for attacks with a short timer before attack activates while boss is not moving
public class NoMoveAttack : MonoBehaviour, IBossAttack
{
    private SpriteRenderer sr;
    private BoxCollider2D col;

    public Color indicator;
    public Color attack;

    [Header("Transform")]
    public float xSize = 1;
    public float ySize = 20;
    public float angle = 0;

    public float damage = 30;
    public float timeBeforeAttack = 0.75f;  // Time before attack activates and can inflict damage (in seconds)
    public float attackDuration = 1;    // How long the hitbox is active for after activation (in seconds)

    public float Damage { get { return damage; } set { damage = value; } }

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();

        transform.position = new Vector3(transform.position.x, transform.position.y);
        transform.localScale = new Vector3(xSize, ySize, 0);
        transform.rotation = Quaternion.Euler(0, 0, angle);
        //col.size = new Vector3(xSize, ySize, 0);
        col.isTrigger = true;
        col.enabled = false;

        sr.color = indicator;
        sr.sortingOrder = -1;

        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        Color color = sr.color;
        while (color.a <= 0.4f)
        {
            color.a += Time.deltaTime / (timeBeforeAttack);
            sr.color = color;

            yield return null;
        }

        yield return new WaitForSeconds(timeBeforeAttack);

        col.enabled = true;
        sr.color = attack;


        yield return new WaitForSeconds(attackDuration);

        while(color.a > 0)
        {
            color.a -= Time.deltaTime / (timeBeforeAttack);
            sr.color = color;

            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null && collision.gameObject.GetComponent<Boss>() == null)
        {
            damageable.Damage(damage, Vector2.zero);
        }
    }
}
