using UnityEngine;

[CreateAssetMenu(fileName = "NuevoNivel", menuName = "Configuración de Nivel")]
public class LevelData : ScriptableObject
{
    [Header("Configuración de Enemigos")]
    public GameObject[] enemyPrefabs; 
    [Header("Dificultad")]
    public int enemiesPerWave = 5;
    public float timeBetweenWaves = 10f;
    public float timeBetweenSpawns = 1f;
}