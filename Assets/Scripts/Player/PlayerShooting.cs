using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Configuración de Disparo")]
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("Audio de Disparo")]
    private AudioSource audioSource;  
    public AudioClip shootSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            Debug.LogError("PlayerShooting no encontró un AudioSource en el GameObject de Aiden.");
        }
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Quaternion bulletRotation = firePoint.rotation;
        if (transform.localScale.x < 0) 
        {
            bulletRotation = Quaternion.Euler(0, 180, 0); 
        }

        Instantiate(bulletPrefab, firePoint.position, bulletRotation);

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}