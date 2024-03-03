using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerCombat : MonoBehaviour
{
    public GameObject AttackObj;
    public WeaponStats weapon;

    [Header("Collider")]
    private RaycastHit2D[] hits;
    [SerializeField] private GameObject[] hitsGO;
    [SerializeField] private Rect box;          // The stores the center (x,y) and dimensions (w,h) of collider box
    [SerializeField] private Vector2 direction; // Direction where attack is cast
    [SerializeField] private float distance;    // Distance from box center in the given direction
    [SerializeField] private float angle;

    [SerializeField] private bool canAttack;

    private Color rayColor = Color.green;

    // Start is called before the first frame update
    void Start()
    {
        canAttack = true;
        box.width = weapon.xSize;
        box.height = weapon.ySize;
        distance = weapon.range;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && canAttack)
        {
            Attack();
        }
        GetDimensions();
        DrawBox();
    }

    private void GetDimensions()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseDir = (new Vector3(mousePosition.x, mousePosition.y, 0) - transform.position).normalized;
        float mouseAngle = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg;
        //Debug.Log(mouseAngle);
        direction = mouseDir;
        angle = mouseAngle;
        box.width = weapon.xSize;
        box.height = weapon.ySize;
        distance = weapon.range;
    }

    private void Attack()
    {
        hitsGO = hits.Where(x => x.collider != null).Select(x => x.collider.gameObject).ToArray();
        StartCoroutine(AttackTimer());
    }

    private void DrawBox()
    {
        hits = Physics2D.BoxCastAll((Vector2)transform.position + box.center * direction, box.size, angle, direction, distance);

        Vector2[] corners = GetBoxCorners((Vector2)transform.position + box.center * direction, box.size, angle);

        // Draw rays for each corner
        //for (int i = 0; i < corners.Length; i++)
        //{
        //    Debug.DrawRay(corners[i], corners[(i + 1) % corners.Length] - corners[i], rayColor);
        //}
        foreach (Vector2 corner in corners)
        {
            Vector2 rayDirection = direction.normalized * distance;
            Debug.DrawRay(corner, rayDirection, rayColor);
        }
    }

    // Calculate box corners based on center, size, and rotation angle
    Vector2[] GetBoxCorners(Vector2 center, Vector2 size, float angle)
    {
        Vector2[] corners = new Vector2[4];
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // Calculate corner positions
        corners[0] = center + (Vector2)(rotation * new Vector2(-size.x / 2, -size.y / 2));
        corners[1] = center + (Vector2)(rotation * new Vector2(size.x / 2, -size.y / 2));
        corners[2] = center + (Vector2)(rotation * new Vector2(-size.x / 2, size.y / 2));
        corners[3] = center + (Vector2)(rotation * new Vector2(size.x / 2, size.y / 2));

        return corners;
    }

    IEnumerator AttackTimer()
    {
        canAttack = false;

        // The windup to the attack
        yield return new WaitForSeconds(weapon.anticipationTime);

        //GameObject strike = Instantiate(AttackObj, (Vector2)transform.position + (direction * weapon.range), Quaternion.Euler(0, 0, angle));
        //strike.transform.localScale = new Vector3(weapon.xSize, weapon.ySize, 0);
        rayColor = Color.red;
        foreach(GameObject go in hitsGO)
        {
            IDamageable damageable = go.GetComponent<IDamageable>();
            if (damageable != this.GetComponent<IDamageable>() && damageable != null)
            {
                damageable.Damage(weapon.damage);
            }
        }
        // The actual strike
        yield return new WaitForSeconds(weapon.strikeTime);

        //Destroy(strike);

        // Player recovers shortly after attacking
        yield return new WaitForSeconds(weapon.recoveryTime);

        canAttack = true;
        rayColor = Color.green;
    }
}
