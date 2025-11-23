using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    // --- LÓGICA DE RECOGIDA ---
    public int value = 10; // Cuántas monedas te da
    
    // --- EFECTOS VISUALES AJUSTABLES EN EL INSPECTOR ---
    [Header("Efecto de Flotación (Arriba/Abajo)")]
    // **NOTA:** He reducido este valor a 0.1f para que el movimiento sea menor.
    [SerializeField] private float floatAmplitude = 0.1f; // Altura máxima que flota (menor movimiento)
    [SerializeField] private float floatSpeed = 2f;       // Velocidad del movimiento

    [Header("Efecto de Pulsación (Escala)")]
    [SerializeField] private float pulseAmplitude = 0.05f; // Cantidad máxima de cambio de escala
    [SerializeField] private float pulseSpeed = 4f;        // Velocidad de la pulsación

    // --- Variables privadas de estado ---
    private Vector3 initialPosition;    // Posición inicial de la moneda
    private Vector3 initialScale;       // Escala inicial de la moneda

    // --- MÉTODOS DE UNITY ---

    void Start()
    {
        // Guardar la posición y escala inicial al comienzo
        initialPosition = transform.position;
        initialScale = transform.localScale;
    }

    void Update()
    {
        // 1. Efecto de Flotación (Arriba y abajo)
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = initialPosition + new Vector3(0f, yOffset, 0f);

        // 2. Efecto de Pulsación (Crecer y decrecer)
        float scaleOffset = Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
        Vector3 newScale = initialScale + (Vector3.one * scaleOffset);
        transform.localScale = newScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Lógica para recoger la moneda
        if (other.CompareTag("Player"))
        {
            // Asegúrate de tener una referencia a GameManager o un sistema de puntaje
            GameManager.Instance.AddScore(value); 
            
            // Destruir la moneda
            Destroy(gameObject);
        }
    }
}