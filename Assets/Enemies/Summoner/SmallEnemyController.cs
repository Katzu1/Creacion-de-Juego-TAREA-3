using UnityEngine;

public class SmallEnemyController : MonoBehaviour
{
    public float moveSpeed = 4f;    // Velocidad de movimiento del pequeño enemigo
    public float stoppingDistance = 1.5f; // Distancia a la que el pequeño enemigo se detendrá del jugador
    public float retreatDistance = 3f;   // Distancia a la que el pequeño enemigo empezará a retroceder del jugador
    public Transform target;        // Referencia al jugador
    public GameObject explosionEffect;  // Efecto de explosión al ser destruido el pequeño enemigo

    private Rigidbody2D rb;

    // Delegado y evento para notificar cuando este enemigo es destruido
    public delegate void EnemyDestroyed();
    public event EnemyDestroyed OnEnemyDestroyed;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (target == null)
            return;

        // Calcula la dirección hacia el jugador
        Vector2 direction = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target.position);

        // Si la distancia es mayor que la distancia de retroceso, pero menor que la distancia de detención, sigue al jugador
        if (distance > retreatDistance && distance < stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
        // Si la distancia es menor que la distancia de retroceso, retrocede del jugador
        else if (distance <= retreatDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, -moveSpeed * Time.deltaTime);
        }
    }

    // Función para destruir este enemigo
    public void DestroyEnemy()
    {
        // Efecto de explosión
        if(explosionEffect) Instantiate(explosionEffect, transform.position, transform.rotation);

        // Notifica que este enemigo ha sido destruido
        OnEnemyDestroyed?.Invoke();

        // Destruye el objeto de este enemigo
        Destroy(gameObject);
    }
}
