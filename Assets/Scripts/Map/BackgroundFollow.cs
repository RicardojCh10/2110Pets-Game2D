using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    private float initialY; // La altura fija donde queremos que se quede

    void Start()
    {
        // 1. Al empezar el juego, guardamos la altura (Y) mundial actual del fondo
        // Asegúrate de colocar el fondo a la altura correcta en la escena antes de dar Play
        initialY = transform.position.y;
    }

    // Usamos LateUpdate para corregir la posición DESPUÉS de que la cámara se haya movido
    void LateUpdate()
    {
        // 2. Mantenemos la posición X y Z que la cámara nos da (por ser hijos)
        // 3. Pero FORZAMOS la posición Y a ser la original que guardamos
        transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
    }
}