using System.Collections;
using UnityEngine;

public class EnemySummon : MonoBehaviour
{
    public float moveSpeed = 3f;        // Velocidad de movimiento del enemigo
    public float visionRadius = 5f;     // Radio de visi�n para seguir al jugador
    public float timeBetweenSpawns = 10f; // Tiempo entre cada invocaci�n de peque�os enemigos
    public GameObject smallEnemyPrefab; // Prefab de los peque�os enemigos
    public Vector2 areaSize = new Vector2(10f, 4f); // Tama�o del �rea de movimiento del enemigo
    public int maxEnemiesToSummon = 5;  // M�ximo n�mero de enemigos a invocar

    private Vector3 initialPosition;    // Posici�n inicial del enemigo
    private Vector3 targetPosition;     // Posici�n objetivo actual del enemigo
    private GameObject player;          // Referencia al jugador
    private bool isFollowing;           // Indica si el enemigo est� siguiendo al jugador

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
        // Si no est� siguiendo al jugador, mueve aleatoriamente dentro del �rea designada
        if (!isFollowing)
        {
            MoveRandomly();
        }
    }

    // Rutina para seguir al jugador si est� dentro del radio de visi�n
    IEnumerator FollowPlayer()
    {
        while (true)
        {
            // Comprueba si el jugador est� dentro del radio de visi�n
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

    // Rutina para invocar peque�os enemigos cada cierto tiempo
    IEnumerator SpawnSmallEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);

            // Si est� siguiendo al jugador o el jugador est� dentro del radio de visi�n, y no se ha alcanzado el m�ximo de invocaciones
            if ((isFollowing || Vector3.Distance(transform.position, player.transform.position) <= visionRadius) &&
                enemiesSummoned < maxEnemiesToSummon)
            {
                InstantiateSmallEnemy();
            }
        }
    }

    // Funci�n para instanciar un peque�o enemigo
    private void InstantiateSmallEnemy()
    {
        GameObject newEnemy = Instantiate(smallEnemyPrefab, transform.position, Quaternion.identity);
        SmallEnemyController enemyController = newEnemy.GetComponent<SmallEnemyController>();

        // Asigna la funci�n que se ejecutar� cuando el enemigo sea destruido
        enemyController.OnEnemyDestroyed += OnSmallEnemyDestroyed;

        enemiesSummoned++;
    }

    // Callback cuando un peque�o enemigo es destruido
    private void OnSmallEnemyDestroyed()
    {
        enemiesDestroyed++;

        // Verifica si se deben invocar m�s enemigos
        if (enemiesDestroyed >= enemiesSummoned)
        {
            enemiesSummoned = 0;
            enemiesDestroyed = 0;
        }
    }

    // Funci�n para moverse aleatoriamente dentro del �rea designada
    private void MoveRandomly()
    {
        // Si alcanza la posici�n objetivo, elige una nueva posici�n aleatoria
        if (transform.position == targetPosition)
        {
            targetPosition = GetRandomPositionInArea();
        }

        // Movimiento hacia la posici�n objetivo
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // Funci�n para obtener una posici�n aleatoria dentro del �rea designada
    private Vector3 GetRandomPositionInArea()
    {
        float randomX = Random.Range(initialPosition.x - areaSize.x / 2f, initialPosition.x + areaSize.x / 2f);
        float randomY = Random.Range(initialPosition.y - areaSize.y / 2f, initialPosition.y + areaSize.y / 2f);
        return new Vector3(randomX, randomY, 0);
    }

    // Dibuja el �rea designada en el editor de Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(initialPosition, new Vector3(areaSize.x, areaSize.y, 0f));
    }
}
