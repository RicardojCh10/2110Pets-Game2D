using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Datos del Nivel Actual")]
    public LevelData levelData; 

    [Header("Puntos de Aparición (Esto sí es físico de la escena)")]
    public Transform[] spawnPoints;

    private int enemiesSpawned = 0;

    void Start()
    {
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

            for (int i = 0; i < levelData.enemiesPerWave; i++)
            {
                GameObject enemyToSpawn = levelData.enemyPrefabs[Random.Range(0, levelData.enemyPrefabs.Length)];
                
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);

                yield return new WaitForSeconds(levelData.timeBetweenSpawns);
            }

            yield return new WaitForSeconds(levelData.timeBetweenWaves);
        }
    }
}