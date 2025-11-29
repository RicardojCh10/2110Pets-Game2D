using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Transform target; 
    public Vector3 offset;   
    public Slider slider;    

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
    }

    public void SetCurrentHealth(int currentHealth)
    {
        slider.value = currentHealth;
    }
}