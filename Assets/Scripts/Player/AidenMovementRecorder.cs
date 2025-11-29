using UnityEngine;
using System.Collections.Generic;

public class AidenMovementRecorder : MonoBehaviour
{
    public float delayTime = 0.25f; 
    
    private List<Vector3> positionHistory = new List<Vector3>(); 
    private List<float> velocityYHistory = new List<float>();
    private int historyLength;
    private Rigidbody2D rb;

    void Start()
    {
        historyLength = Mathf.RoundToInt(delayTime / Time.fixedDeltaTime);
        rb = GetComponent<Rigidbody2D>(); 
    }

    void FixedUpdate()
    {
        positionHistory.Insert(0, transform.position);
        velocityYHistory.Insert(0, rb.linearVelocity.y);
        
        if (positionHistory.Count > historyLength)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
            velocityYHistory.RemoveAt(velocityYHistory.Count - 1);
        }
    }

    public Vector3 GetDelayedPosition()
    {
        if (positionHistory.Count < historyLength) return transform.position;
        return positionHistory[historyLength - 1];
    }

    public float GetDelayedVelocityY()
    {
        if (velocityYHistory.Count < historyLength) return 0f;
        return velocityYHistory[historyLength - 1];
    }
}