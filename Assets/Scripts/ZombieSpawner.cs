using System.Collections;
using UnityEngine;
using UnityEngine.AI;   // Necesario para NavMeshAgent / NavMesh

public class ZombieSpawner : MonoBehaviour
{
    [Header("Puntos de aparición")]
    public Transform[] spawnPoints;           // Spawn1, Spawn2, Spawn3, Spawn4...

    [Header("Prefabs de zombis")]
    public GameObject zombieRound1Prefab;     // Zombie_Base
    public GameObject zombieRound2Prefab;     // Zombie_Base_R2
    public GameObject zombieRound3Prefab;     // Zombie_Base_R3

    [Header("Ajustes")]
    public float spawnInterval = 2f;          // segundos entre spawns

    [Header("Gestión de zombis activos")]
    public bool destroyPreviousRoundZombies = true; // borra zombis de la ronda anterior

    int zombiesToSpawn = 0;                   // cuántos zombis debe crear esta ronda
    int zombiesSpawned = 0;                   // cuántos ya ha creado
    int currentRound = 1;

    Coroutine spawnCoroutine;

    void Awake()
    {
        Debug.Log($"[ZombieSpawner] Awake en objeto: {gameObject.name}");
    }

    /// <summary>
    /// Llamado por GameTimer para iniciar una ronda.
    /// </summary>
    public void StartRound(int round, int totalToSpawn)
    {
        currentRound = round;
        zombiesToSpawn = Mathf.Max(0, totalToSpawn);
        zombiesSpawned = 0;

        Debug.Log($"[ZombieSpawner] StartRound -> RONDA {currentRound}, a spawnear {zombiesToSpawn} zombis");

        // 💥 IMPORTANTE: limpiamos zombis de rondas anteriores (si quieres)
        ClearExistingZombies();

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// Destruye todos los zombis que haya en la escena (tienen ZombieHealth).
    /// Así cada ronda empieza limpia, sin zombis viejos.
    /// </summary>
    void ClearExistingZombies()
    {
        if (!destroyPreviousRoundZombies) return;

        var allZombies = FindObjectsOfType<ZombieHealth>();
        foreach (var z in allZombies)
        {
            Destroy(z.gameObject);
        }

        Debug.Log($"[ZombieSpawner] Se limpiaron {allZombies.Length} zombis de la ronda anterior.");
    }

    IEnumerator SpawnLoop()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] ZombieSpawner: no hay spawnPoints asignados.");
            yield break;
        }

        while (zombiesSpawned < zombiesToSpawn)
        {
            bool spawned = SpawnOneZombie();
            if (spawned)
            {
                zombiesSpawned++;
            }
            else
            {
                // Si no se pudo spawnear (por prefab nulo o sin NavMesh),
                // salimos para no quedarnos en bucle.
                Debug.LogWarning($"[ZombieSpawner] No se pudo spawnear zombi en ronda {currentRound}. Rompiendo SpawnLoop.");
                break;
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        spawnCoroutine = null;
    }

    /// <summary>
    /// Spawnea un zombi de la ronda actual, colocado sobre el NavMesh.
    /// Devuelve true si todo salió bien.
    /// </summary>
    bool SpawnOneZombie()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return false;

        // Punto aleatorio de aparición
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Elegimos el prefab según la ronda
        GameObject prefabToUse = null;
        switch (currentRound)
        {
            case 1: prefabToUse = zombieRound1Prefab; break;
            case 2: prefabToUse = zombieRound2Prefab; break;
            case 3: prefabToUse = zombieRound3Prefab; break;
            default: prefabToUse = zombieRound1Prefab; break;
        }

        if (prefabToUse == null)
        {
            Debug.LogWarning($"[{gameObject.name}] ZombieSpawner: no hay prefab asignado para la ronda {currentRound}");
            return false;
        }

        // Buscamos una posición VÁLIDA en el NavMesh cerca del spawnPoint
        NavMeshHit hit;
        Vector3 spawnPos = point.position;

        if (NavMesh.SamplePosition(point.position, out hit, 3f, NavMesh.AllAreas))
        {
            spawnPos = hit.position;
        }
        else
        {
            Debug.LogWarning($"[ZombieSpawner] No se encontró NavMesh cerca del punto {point.name} para el zombi {prefabToUse.name}");
            return false;
        }

        Debug.Log($"[ZombieSpawner] Spawneando {prefabToUse.name} en RONDA {currentRound}");

        // Instanciamos el zombi directamente sobre el NavMesh
        GameObject instance = Instantiate(prefabToUse, spawnPos, point.rotation);

        // Sincronizamos el NavMeshAgent con esa posición
        NavMeshAgent agent = instance.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.Warp(spawnPos);
        }

        // Comprobamos que el prefab tiene ZombieHealth (para que pueda morir)
        var health = instance.GetComponent<ZombieHealth>();
        if (health == null)
        {
            Debug.LogWarning($"[ZombieSpawner] El prefab instanciado ({prefabToUse.name}) NO tiene ZombieHealth. Este zombi no podrá morir bien.");
        }

        return true;
    }
}
