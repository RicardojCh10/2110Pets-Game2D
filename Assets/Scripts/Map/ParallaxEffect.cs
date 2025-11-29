using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Header("Velocidad de Parallax")]
    [Range(0f, 1f)]
    public float parallaxStrength = 0.5f; 

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    void Update()
    {
        // 1. Calcula cuánto se ha movido la cámara desde el último frame
        Vector3 cameraDelta = cameraTransform.position - lastCameraPosition;

        // 2. Aplica el delta de la cámara, pero multiplicado por la fuerza de parallax
        // Usamos solo el movimiento horizontal (X)
        float parallaxMovementX = cameraDelta.x * parallaxStrength;
        
        // 3. Mueve este sprite
        transform.position += new Vector3(parallaxMovementX, 0, 0);

        // 4. Actualiza la posición de la cámara para el próximo frame
        lastCameraPosition = cameraTransform.position;
    }
}