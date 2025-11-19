using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Datos del Nivel Actual")]
    public LevelData levelData; // <--- ¡ScriptableObject!

    [Header("Puntos de Aparición (Esto sí es físico de la escena)")]
    public Transform[] spawnPoints;

    private int enemiesSpawned = 0;

    void Start()
    {
        // Validación de seguridad
        if (levelData == null)
        {
            Debug.LogError("¡Falta asignar el LevelData en el Spawner!");
            return;
        }
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        while (true)
        {
            enemiesSpawned = 0;

            // Leemos los datos desde el ScriptableObject (levelData)
            for (int i = 0; i < levelData.enemiesPerWave; i++)
            {
                // Usamos la lista del ScriptableObject
                GameObject enemyToSpawn = levelData.enemyPrefabs[Random.Range(0, levelData.enemyPrefabs.Length)];
                
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);

                yield return new WaitForSeconds(levelData.timeBetweenSpawns);
            }

            yield return new WaitForSeconds(levelData.timeBetweenWaves);
        }
    }
}