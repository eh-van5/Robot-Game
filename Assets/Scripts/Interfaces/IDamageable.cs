using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    float Hitpoints { get; }

    void Damage(float damageAmount);
    void Heal(float healAmount);
    void Death();
}
