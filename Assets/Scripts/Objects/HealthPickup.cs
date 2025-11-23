using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // --- LÓGICA DE RECOGIDA ---
    public int healAmount = 20; // Cuánta vida recupera

    // --- EFECTOS VISUALES AJUSTABLES EN EL INSPECTOR ---
    [Header("Efecto de Flotación (Arriba/Abajo)")]
    // **REDUCIDO:** Valor de 0.03f para un movimiento muy sutil.
    [SerializeField] private float floatAmplitude = 0.03f; 
    [SerializeField] private float floatSpeed = 1.5f;     

    [Header("Efecto de Destello/Brillo")]
    [SerializeField] private float flashSpeed = 6f;       
    
    // El color de destello se establecerá aquí, asegurando que sea verde claro.
    private Color flashColor = new Color(0.7f, 1.0f, 0.7f, 1f); 

    // --- Variables privadas de estado ---
    private Vector3 initialPosition;    
    private SpriteRenderer spriteRenderer; 

    // --- MÉTODOS DE UNITY ---

    void Start()
    {
        initialPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("HealthPickup necesita un componente SpriteRenderer para el efecto de brillo.");
        }
        
        // **CORRECCIÓN DE COLOR:** Si el color no se estaba aplicando, nos aseguramos de que
        // el valor privado 'flashColor' tenga el verde de curación que queremos.
        // Si quieres que el color sea ajustable nuevamente en el Inspector, usa [SerializeField] aquí.
    }

    void Update()
    {
        // 1. Efecto de Flotación (Arriba y abajo)
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = initialPosition + new Vector3(0f, yOffset, 0f);

        // 2. Efecto de Destello/Brillo (Cambio de Color)
        if (spriteRenderer != null)
        {
            float t = (Mathf.Sin(Time.time * flashSpeed) + 1f) / 2f; 
            
            // Interpolamos al color de curación (verde claro)
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