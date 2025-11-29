using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int value = 10; 
    
    [Header("Efecto de Flotación (Arriba/Abajo)")]
    [SerializeField] private float floatAmplitude = 0.1f; 
    [SerializeField] private float floatSpeed = 2f;       

    [Header("Efecto de Pulsación (Escala)")]
    [SerializeField] private float pulseAmplitude = 0.05f; 
    [SerializeField] private float pulseSpeed = 4f;

    private Vector3 initialPosition;
    private Vector3 initialScale;


    void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = initialPosition + new Vector3(0f, yOffset, 0f);

        float scaleOffset = Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
        Vector3 newScale = initialScale + (Vector3.one * scaleOffset);
        transform.localScale = newScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(value); 
            
            Destroy(gameObject);
        }
    }
}