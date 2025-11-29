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
        Destroy(gameObject, lifeTime);

    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            GameManager.Instance.TakePlayerDamage(damage);
            
            playerHealth.StartCoroutine("DamageEffect"); 
        }
        
        Destroy(gameObject); 
    }
    else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
    {
        Destroy(gameObject);
    }
}
}