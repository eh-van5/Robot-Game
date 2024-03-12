using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{   
    // Used to determine when bullet disappears
    public enum DisperseType
    {
        Duration,   // Bullet destroys itself after a certain amount of time
        Distance    // Bullet destroys itself after traversing a certain distance
    }
    public DisperseType disperseType = DisperseType.Duration;
    public float damage;
    public float speed;
    public float duration;
    public float distance;

    private Vector2 spawnPoint;
    private float timer = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = new Vector2(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        // If timer exceeds bullet duration, then destroy this bullet
        if(timer > duration)
        {
            Destroy(gameObject);
        }
        timer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        transform.position = UpdatePosition(timer);
    }

    private Vector2 UpdatePosition(float timer)
    {
        float x = timer * speed * transform.right.x;
        float y = timer * speed * transform.right.y;

        return new Vector2(x + spawnPoint.x, y + spawnPoint.y);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Destroy(gameObject);
    //    IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
    //    if (damageable != null)
    //    {
    //        damageable.Damage(damage);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(damage, Vector2.zero);
            Destroy(gameObject);
        }
    }
}
