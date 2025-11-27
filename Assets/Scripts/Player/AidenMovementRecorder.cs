using UnityEngine;
using System.Collections.Generic;

public class AidenMovementRecorder : MonoBehaviour
{
    // Tiempo de retraso que Kiro tendrá (ej. 0.25 segundos)
    public float delayTime = 0.25f; 
    
    // Almacena un historial de posiciones
    private List<Vector3> positionHistory = new List<Vector3>(); 
    
    // Número de frames que necesitamos para el retraso
    private int historyLength;

    void Start()
    {
        // Calcular cuántos frames a FixedUpdate rate se necesitan
        historyLength = Mathf.RoundToInt(delayTime / Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        // 1. Guardar la posición actual
        // Insert(0, ...) pone el nuevo elemento al principio de la lista.
        positionHistory.Insert(0, transform.position);
        
        // 2. Limitar la longitud del historial
        if (positionHistory.Count > historyLength)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
        }
    }

    // Función pública para que Kiro acceda a la posición grabada con retraso
    public Vector3 GetDelayedPosition()
    {
        // Si no hay suficiente historial (ej. al inicio), devuelve la posición actual.
        if (positionHistory.Count < historyLength)
            return transform.position;
            
        // Devuelve la posición de hace 'historyLength' frames.
        return positionHistory[historyLength - 1];
    }
}