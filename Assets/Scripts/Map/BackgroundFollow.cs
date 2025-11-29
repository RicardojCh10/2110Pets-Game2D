using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    private float initialY; 

    void Start()
    {
        initialY = transform.position.y;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
    }
}