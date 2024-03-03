using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon")]
public class WeaponStats : ScriptableObject
{
    public enum WeaponType
    {
        DualSword,
        GreatSword,
        Dagger,
        Claws
    }

    public Sprite sprite;
    public WeaponType type;
    public float damage;
    public float range;

    [Header("Collider Size")]
    public float xSize;
    public float ySize;

    [Header("Attack Animation")]
    public float anticipationTime;
    public float strikeTime;
    public float recoveryTime;
    public float AttackTime { get { return anticipationTime + strikeTime + recoveryTime; } }
}
