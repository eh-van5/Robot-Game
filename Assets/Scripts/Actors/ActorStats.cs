using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Actor")]
public class ActorStats : ScriptableObject
{
    [Header("Movement")]
    [SerializeField] public float speed = 10f;
    [SerializeField] public float jumpForce = 8f;
    [SerializeField] public float jumptime = 10f;
    [SerializeField] public float fallingGravityScale = 2f;
    [SerializeField] public float lowJumpGravityScale = 2.5f;
    [SerializeField] public float slamScale = 20f;
    [SerializeField] public int bonusJump = 1;


    private float airTime;
    [SerializeField] public bool doubleJump;


   
    [Header("Dashing")]
    [SerializeField] public float dashSpeed = 20f;
    [SerializeField] public float dashTime = 0.2f; //How long player stays in dash
    [SerializeField] public float dashFalloffMultiplier = 5f;
    [SerializeField] public float timeBeforeDash = 5f; //Seconds before player can dash again
    [SerializeField] public bool canDash;
    [Header("Combat")]
    [SerializeField] public float maxHP = 100f;
    public float currentHP;
    float Hitpoints { get { return currentHP; }}

}
