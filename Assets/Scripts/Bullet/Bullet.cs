using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 25;
    public float lifeTime = 2f;

    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Enemy zombie = hitInfo.GetComponent<Enemy>();
        if (zombie != null)
        {
            zombie.TakeDamage(damage);
            Destroy(gameObject); 
            return; 
        }

        RobotEnemy robot = hitInfo.GetComponent<RobotEnemy>();
        if (robot != null)
        {
            robot.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        FlyingEnemy drone = hitInfo.GetComponent<FlyingEnemy>();
        if (drone != null)
        {
            drone.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }


        Destroy(gameObject);
    }
}