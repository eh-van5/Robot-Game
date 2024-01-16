using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    Vector3 Position { get; }
    float Hitpoints { get; set; }

    void Damage(float damageAmount);
    void Heal(float healAmount);
    void Death();
}
