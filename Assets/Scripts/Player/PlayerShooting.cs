using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Configuración de Disparo")]
    public Transform firePoint; // El punto desde donde sale la bala
    public GameObject bulletPrefab; // El prefab de la bala

    // Variables para Audio
    [Header("Audio de Disparo")]
    private AudioSource audioSource;  
    public AudioClip shootSound;

    // Configuración inicial
    private void Start()
    {
        // Obtener la referencia al AudioSource que ya existe en el GameObject
        audioSource = GetComponent<AudioSource>();
        
        // Opcional: Verificar que lo encontró
        if (audioSource == null)
        {
            Debug.LogError("PlayerShooting no encontró un AudioSource en el GameObject de Aiden.");
        }
    }
    // Esta es la función pública que conectarás al Evento "Fire"
    public void OnFire(InputAction.CallbackContext context)
    {
        // "performed" significa que la acción se acaba de presionar
        if (context.performed)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // 1. Determinamos la rotación correcta para la bala
        Quaternion bulletRotation = firePoint.rotation;
        if (transform.localScale.x < 0) // Si el jugador está volteado (escala negativa)
        {
            bulletRotation = Quaternion.Euler(0, 180, 0); // Giramos la bala 180 grados
        }

        // 2. "Instantiate" crea una copia de tu prefab de bala
        // Usamos la 'bulletRotation' que acabamos de calcular
        Instantiate(bulletPrefab, firePoint.position, bulletRotation);

        // 3. Reproducir el sonido de disparo
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}