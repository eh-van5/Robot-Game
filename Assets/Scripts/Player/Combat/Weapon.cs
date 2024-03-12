using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponStats stats;
    public bool canAttack;

    void Start()
    {
        canAttack = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
