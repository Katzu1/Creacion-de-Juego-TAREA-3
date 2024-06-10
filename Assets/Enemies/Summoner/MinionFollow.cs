using UnityEngine;

public class MinionFollow : MonoBehaviour
{
    public float speed = 3.0f; // Velocidad de movimiento del minion
    public float stoppingDistance = 1.0f; // Distancia a la que el minion se detiene del jugador
    public float visionRange = 5.0f; // Rango de visión del minion
    public float disappearTime = 5.0f; // Tiempo en segundos antes de desaparecer si no se mueve

    private Transform target; // Referencia al jugador
    private float timeSinceLastMovement; // Tiempo transcurrido sin movimiento

    public SmallEnemyController sec;

    void Start()
    {
        // Buscar el GameObject con la etiqueta "Player" y obtener su transform
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Comprobar si el jugador existe
        if (target != null)
        {
            // Calcular la distancia al jugador
            float distance = Vector2.Distance(transform.position, target.position);

            // Comprobar si el jugador está dentro del rango de visión
            if (distance <= visionRange)
            {
                // Si la distancia es mayor que la distancia de parada, moverse hacia el jugador
                if (distance > stoppingDistance)
                {
                    // Moverse hacia el jugador
                    transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

                    // Reiniciar el tiempo sin movimiento
                    timeSinceLastMovement = 0;
                }
            }
        }

        // Contar el tiempo sin movimiento
        timeSinceLastMovement += Time.deltaTime;

        // Si el tiempo sin movimiento es mayor o igual a disappearTime, destruir el minion
        if (timeSinceLastMovement >= disappearTime)
        {
            sec.DestroyEnemy();
            //Destroy(gameObject);
        }
    }
}
