using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Oleadas")]
    public GameObject[] enemyPrefabs; // Arrastra tus PREFABS de enemigos aquí
    public Transform[] spawnPoints; // Puntos donde pueden aparecer
    
    public int enemiesPerWave = 5; // Cuántos enemigos por oleada
    public float timeBetweenWaves = 10f; // Tiempo (seg) entre oleadas
    public float timeBetweenSpawns = 1f; // Tiempo (seg) entre cada enemigo de una misma oleada

    private int enemiesSpawned = 0;

    void Start()
    {
        // Inicia la corrutina de "spawn"
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        // Bucle infinito para seguir creando oleadas
        while (true)
        {
            Debug.Log("¡Iniciando nueva oleada!");
            enemiesSpawned = 0;

            // Genera la cantidad de enemigos de la oleada
            for (int i = 0; i < enemiesPerWave; i++)
            {
                // 1. Elige un enemigo aleatorio de tu lista de prefabs
                GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

                // 2. Elige un punto de spawn aleatorio de tu lista
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                // 3. Crea el enemigo
                Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);

                enemiesSpawned++;

                // 4. Espera un poco antes de generar el siguiente enemigo
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            // 5. Espera a la siguiente oleada
            Debug.Log("Oleada terminada. Esperando para la siguiente...");
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }
}