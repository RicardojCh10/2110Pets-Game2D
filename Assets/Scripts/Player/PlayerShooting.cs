using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Configuración de Disparo")]
    public Transform firePoint; // El punto desde donde sale la bala
    public GameObject bulletPrefab; // El prefab de la bala

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
    }
}