using UnityEngine;
using UnityEngine.UI; // Necesario para trabajar con UI

public class EnemyHealthBar : MonoBehaviour
{
    public Transform target; // El enemigo al que seguir� la barra
    public Vector3 offset;   // Para ajustar la posici�n de la barra sobre el enemigo
    public Slider slider;    // La referencia a nuestro componente Slider

    void Update()
    {
        // Hace que la barra siga la posici�n del enemigo en la pantalla
        if (target != null)
        {
            transform.position = target.position + offset;
        }
        else
        {
            // Si el enemigo muere (target es nulo), destruimos la barra de vida
            Destroy(gameObject);
        }
    }

    // Una funci�n para inicializar la vida m�xima y el valor actual
    public void SetHealth(int currentHealth, int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
    }

    // Una funci�n para actualizar solo el valor actual
    public void SetCurrentHealth(int currentHealth)
    {
        slider.value = currentHealth;
    }
}