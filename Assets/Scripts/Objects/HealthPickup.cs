using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 20;

    [Header("Efecto de Flotación (Arriba/Abajo)")]
    [SerializeField] private float floatAmplitude = 0.03f; 
    [SerializeField] private float floatSpeed = 1.5f;     

    [Header("Efecto de Destello/Brillo")]
    [SerializeField] private float flashSpeed = 6f;       
    
    private Color flashColor = new Color(0.7f, 1.0f, 0.7f, 1f); 

    private Vector3 initialPosition;    
    private SpriteRenderer spriteRenderer; 

    void Start()
    {
        initialPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("HealthPickup necesita un componente SpriteRenderer para el efecto de brillo.");
        }
        
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = initialPosition + new Vector3(0f, yOffset, 0f);

        if (spriteRenderer != null)
        {
            float t = (Mathf.Sin(Time.time * flashSpeed) + 1f) / 2f; 
            
            spriteRenderer.color = Color.Lerp(Color.white, flashColor, t);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.HealPlayer(healAmount);
            Destroy(gameObject);
        }
    }
}