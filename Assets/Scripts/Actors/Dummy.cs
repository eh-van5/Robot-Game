using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Actor, IDamageable
{
    public float Hitpoints { get { return 0; } }
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
    #region IDamageable
    public void Damage(float damageAmount, Vector2 knockback)
    {
        Flash();
        rb.AddForce(knockback);
        Debug.Log("Dummy Hit");
    }

    public void Heal(float healAmount)
    {

    }

    public void Death()
    {

    }
    #endregion
}
