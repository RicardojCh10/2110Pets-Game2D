using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 25;
    public float lifeTime = 2f; // Tiempo antes de autodestruirse

    void Start()
    {
        // Le da velocidad hacia la derecha (local) de donde fue disparada
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
        // Se destruye después de 'lifeTime' segundos
        Destroy(gameObject, lifeTime);
    }

    // Se activa cuando toca un Trigger
    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Intenta encontrar el script "Enemy" en lo que sea que golpeó
        Enemy enemy = hitInfo.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage); // Le hace daño
        }

        // Destruye la bala al chocar con CUALQUIER COSA
        // (Excepto otros triggers que no sean enemigos, para eso usaríamos Layers,
        // pero por ahora esto funciona)
        Destroy(gameObject);
    }
}