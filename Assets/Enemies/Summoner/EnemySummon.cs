using System.Collections;
using UnityEngine;

public class EnemySummon : MonoBehaviour
{
    public float moveSpeed = 3f;        // Velocidad de movimiento del enemigo
    public float visionRadius = 5f;     // Radio de visión para seguir al jugador
    public float timeBetweenSpawns = 10f; // Tiempo entre cada invocación de pequeños enemigos
    public GameObject smallEnemyPrefab; // Prefab de los pequeños enemigos
    public Vector2 areaSize = new Vector2(10f, 4f); // Tamaño del área de movimiento del enemigo
    public int maxEnemiesToSummon = 5;  // Máximo número de enemigos a invocar

    private Vector3 initialPosition;    // Posición inicial del enemigo
    private Vector3 targetPosition;     // Posición objetivo actual del enemigo
    private GameObject player;          // Referencia al jugador
    private bool isFollowing;           // Indica si el enemigo está siguiendo al jugador

    private int enemiesSummoned;        // Contador de enemigos invocados
    private int enemiesDestroyed;       // Contador de enemigos destruidos

    void Start()
    {
        initialPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        isFollowing = false;
        enemiesSummoned = 0;
        enemiesDestroyed = 0;

        StartCoroutine(FollowPlayer());
        StartCoroutine(SpawnSmallEnemies());
    }

    void Update()
    {
        // Si no está siguiendo al jugador, mueve aleatoriamente dentro del área designada
        if (!isFollowing)
        {
            MoveRandomly();
        }
    }

    // Rutina para seguir al jugador si está dentro del radio de visión
    IEnumerator FollowPlayer()
    {
        while (true)
        {
            // Comprueba si el jugador está dentro del radio de visión
            if (Vector3.Distance(transform.position, player.transform.position) <= visionRadius)
            {
                isFollowing = true;
                targetPosition = player.transform.position;
            }
            else
            {
                isFollowing = false;
            }

            yield return null;
        }
    }

    // Rutina para invocar pequeños enemigos cada cierto tiempo
    IEnumerator SpawnSmallEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);

            // Si está siguiendo al jugador o el jugador está dentro del radio de visión, y no se ha alcanzado el máximo de invocaciones
            if ((isFollowing || Vector3.Distance(transform.position, player.transform.position) <= visionRadius) &&
                enemiesSummoned < maxEnemiesToSummon)
            {
                InstantiateSmallEnemy();
            }
        }
    }

    // Función para instanciar un pequeño enemigo
    private void InstantiateSmallEnemy()
    {
        GameObject newEnemy = Instantiate(smallEnemyPrefab, transform.position, Quaternion.identity);
        SmallEnemyController enemyController = newEnemy.GetComponent<SmallEnemyController>();

        // Asigna la función que se ejecutará cuando el enemigo sea destruido
        enemyController.OnEnemyDestroyed += OnSmallEnemyDestroyed;

        enemiesSummoned++;
    }

    // Callback cuando un pequeño enemigo es destruido
    private void OnSmallEnemyDestroyed()
    {
        enemiesDestroyed++;

        // Verifica si se deben invocar más enemigos
        if (enemiesDestroyed >= enemiesSummoned)
        {
            enemiesSummoned = 0;
            enemiesDestroyed = 0;
        }
    }

    // Función para moverse aleatoriamente dentro del área designada
    private void MoveRandomly()
    {
        // Si alcanza la posición objetivo, elige una nueva posición aleatoria
        if (transform.position == targetPosition)
        {
            targetPosition = GetRandomPositionInArea();
        }

        // Movimiento hacia la posición objetivo
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // Función para obtener una posición aleatoria dentro del área designada
    private Vector3 GetRandomPositionInArea()
    {
        float randomX = Random.Range(initialPosition.x - areaSize.x / 2f, initialPosition.x + areaSize.x / 2f);
        float randomY = Random.Range(initialPosition.y - areaSize.y / 2f, initialPosition.y + areaSize.y / 2f);
        return new Vector3(randomX, randomY, 0);
    }

    // Dibuja el área designada en el editor de Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(initialPosition, new Vector3(areaSize.x, areaSize.y, 0f));
    }
}
