using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will have the hitboxes for every damageable part of a boss
public class BossComponent : MonoBehaviour, IDamageable
{
    [SerializeField] private Boss boss;

    [SerializeField] private float maxHitpoints = 100;
    [SerializeField] private float hitpoints = 100;

    private BoxCollider2D col;

    public float Hitpoints { get { return hitpoints; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region IDamageable
    public void Damage(float damageAmount, Vector2 knockback)
    {
        boss.Damage(damageAmount, knockback);
        hitpoints -= damageAmount;
        if (hitpoints <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Debug.Log("Boss Component Defeated");
        Destroy(gameObject);
    }

    public void Heal(float healAmount)
    {
        boss.Heal(healAmount);
    }
    #endregion
}
