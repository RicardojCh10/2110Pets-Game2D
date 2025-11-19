using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Configuración de Bala")]
    public float speed = 7f;
    public int damage = 10;
    public float lifeTime = 4f;

    private Vector2 moveDirection;

    void Start()
    {
        // Destruir la bala automáticamente después de un tiempo para ahorrar memoria
        Destroy(gameObject, lifeTime);
        
        // Calcular la dirección inicial (si queremos que vaya recta hacia donde apuntó el arma)
        // Opcional: Si quieres que la bala tenga "homing" (seguimiento), la lógica iría en Update
    }

    void Update()
    {
        // Mover la bala hacia su "derecha" (hacia adelante)
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
{
    // Si choca con el Jugador (Aiden)
    if (collision.CompareTag("Player"))
    {
        // 1. OBTENER el componente PlayerHealth de Aiden
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            // 2. APLICAR daño
            GameManager.Instance.TakePlayerDamage(damage);
            
            // 3. ACTIVAR el efecto visual llamando a la Coroutine de Aiden
            playerHealth.StartCoroutine("DamageEffect"); // Usamos StartCoroutine con string
        }
        
        Destroy(gameObject); // La bala explota
    }
    // Si choca con el Suelo (Layer "Ground" o Tag "Ground")
    else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
    {
        Destroy(gameObject); // La bala choca con la pared/piso
    }
}
}