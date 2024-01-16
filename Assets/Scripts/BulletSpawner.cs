using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SpawnerType
{
    Straight,       // Single straight line
    Spread          // Spread around circle
}
public class BulletSpawner : MonoBehaviour
{
    [Header("Bullet Attributes")]
    public GameObject bulletPrefab;
    [SerializeField] private float damage = 30;
    [SerializeField] private float speed = 8;
    [SerializeField] private float bulletDuration = 10;

    [Header("Spawner Attributes")]
    [SerializeField] private SpawnerType spawnerType = SpawnerType.Straight;

    [SerializeField] private float spawnerDuration = 5; // Amount of time before spawner stops spawning
    [SerializeField] private float fireRate = 0.1f;    // Measured in seconds per shot
    [SerializeField] private float rotationSpeed = 1; // Measured in angles per second
    [SerializeField] private int numBullets = 10;     // Number of bullets in spread shot, spread across [-180, 180] degrees

    private float totalTimer;   // Used to measure how long the spawner has been active
    private float timer;        // Time in between fires

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (totalTimer > spawnerDuration) Destroy(gameObject);

        switch (spawnerType)
        {
            case SpawnerType.Straight:
                if (timer > fireRate)
                {
                    FireBullet(transform.rotation);
                    timer = 0;
                }
                break;

            case SpawnerType.Spread:
                if (timer > fireRate)
                {
                    int anglePerBullet = 360 / numBullets;
                    for (int num = 0; num < numBullets; num++)
                    {
                        FireBullet(Quaternion.Euler(0, 0, transform.eulerAngles.z + (anglePerBullet * num)));
                    }
                    timer = 0;
                }
                break;

        }

        totalTimer += Time.deltaTime;
        timer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 0, rotationSpeed));
    }

    //public void Initialize()

    private void FireBullet(Quaternion rotation)
    {
        if (bulletPrefab)
        {
            GameObject spawnedBullet = Instantiate(bulletPrefab, transform.position, rotation);
            Bullet bulletProperties = spawnedBullet.GetComponent<Bullet>();
            bulletProperties.damage = damage;
            bulletProperties.speed = speed;
            bulletProperties.duration = bulletDuration;
        }
    }
}
