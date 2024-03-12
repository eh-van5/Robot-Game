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
    [SerializeField] private float angle;

    public bool canAttack;

    private Color rayColor = Color.green;

    // Start is called before the first frame update
    void Start()
    {
        canAttack = true;
        box.width = weapon.xSize;
        box.height = weapon.ySize;
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    #region Raycasting
    private void DrawBox()
    {
        Vector2 boxcenter = new Vector2(box.x + box.width / 2, box.y + box.width / 2);
        hits = Physics2D.BoxCastAll((Vector2)transform.position + boxcenter * direction, box.size, angle, direction, 0);
        hitsGO = hits.Where(x => x.collider != null).Select(x => x.collider.gameObject).ToArray();

        Vector2[] corners = GetBoxCorners((Vector2)transform.position + boxcenter * direction, box.size, angle);

        Debug.DrawLine(transform.position, (Vector2)transform.position + boxcenter * direction, Color.green);
        //Draw rays for each corner
        for (int i = 0; i < corners.Length; i++)
        {
            Debug.DrawRay(corners[i], corners[(i + 1) % corners.Length] - corners[i], rayColor);
            //Debug.DrawLine(corners[i], (Vector2)corners[i] + direction * 50, Color.green);
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
    #endregion

    public IEnumerator AttackTimer()
    {
        canAttack = false;

        // The windup to the attack
        yield return new WaitForSeconds(weapon.anticipationTime);

        //GameObject strike = Instantiate(AttackObj, (Vector2)transform.position + (direction * weapon.range), Quaternion.Euler(0, 0, angle));
        //strike.transform.localScale = new Vector3(weapon.xSize, weapon.ySize, 0);
        rayColor = Color.red;
        foreach(RaycastHit2D hit in hits)
        {
            GameObject go = hit.collider.gameObject;
            if(go != null)
            {
                IDamageable damageable = go.GetComponent<IDamageable>();
                if (damageable != this.GetComponent<IDamageable>() && damageable != null)
                {
                    Vector2 hitDir = ((Vector2)go.transform.position - hit.point).normalized;
                    float xForce;
                    Debug.DrawLine(go.transform.position, hit.point, Color.yellow, 2f);

                    if (hitDir.x > 0)
                        xForce = 1;
                    else if (hitDir.x == 0)
                        xForce = 0;
                    else
                        xForce = -1;
                    Debug.Log(xForce);
                    Vector2 knockback = new Vector2(xForce, 0) * weapon.knockbackForce;
                    damageable.Damage(weapon.damage, knockback);
                }
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
