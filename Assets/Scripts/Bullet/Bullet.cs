using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 25;
    public float lifeTime = 2f;

    void Start()
    {
        // Usamos linearVelocity (correcto para Unity 6)
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 1. ¿Es un Zombie o Bandido? (Script "Enemy")
        Enemy zombie = hitInfo.GetComponent<Enemy>();
        if (zombie != null)
        {
            zombie.TakeDamage(damage);
            Destroy(gameObject); // Importante: destruir y salir para no seguir comprobando
            return; 
        }

        // 2. ¿Es un Robot Militar? (Script "RobotEnemy")
        RobotEnemy robot = hitInfo.GetComponent<RobotEnemy>();
        if (robot != null)
        {
            robot.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // 3. ¿Es un Robot Volador? (Script "FlyingEnemy")
        FlyingEnemy drone = hitInfo.GetComponent<FlyingEnemy>();
        if (drone != null)
        {
            drone.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Si llegamos aquí, es que chocó con algo que no es un enemigo (pared, suelo, etc.)
        // Asegúrate de configurar las capas (Layers) para que no choque con triggers invisibles
        Destroy(gameObject);
    }
}