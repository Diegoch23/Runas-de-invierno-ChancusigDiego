using UnityEngine;

public class FlyingEnemyController : MonoBehaviour
{
    // Cuatro puntos que definen el rectángulo de patrulla
    public Transform pointA;  // Esquina inferior izquierda
    public Transform pointB;  // Esquina inferior derecha
    public Transform pointC;  // Esquina superior derecha
    public Transform pointD;  // Esquina superior izquierda

    public float speed = 3f;
    private Vector2 targetPosition;

    public float detectionRange = 6f;
    public Transform player;
    public float chaseSpeed = 5f;

    public LayerMask groundLayer;
    private Transform groundCheck;
    public float edgeDetectionDistance = 0.3f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isDead = false;

    [Header("Vida")]
    public int maxHealth = 40;
    private int currentHealth;
    private bool isTakingDamage = false;
    public float knockbackForce = 4f;

    [Header("Score")]
    public ScoreVisualManager scoreManager;

    // Variables para mover entre puntos del rectángulo en orden
    private int currentTargetIndex = 0;
    private Transform[] patrolPoints;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        groundCheck = new GameObject("GroundCheck").transform;
        groundCheck.parent = transform;
        groundCheck.localPosition = new Vector3(0, -0.5f, 0);

        // Lista de puntos en orden para patrullar (A -> B -> C -> D -> A ...)
        patrolPoints = new Transform[] { pointA, pointB, pointC, pointD };
        targetPosition = patrolPoints[currentTargetIndex].position;

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player != null)
        {
            float distToPlayer = Vector2.Distance(transform.position, player.position);

            if (distToPlayer <= detectionRange)
            {
                // Perseguir jugador
                targetPosition = player.position;
                MoveTowards(targetPosition, chaseSpeed);
                return;
            }
        }

        PatrolInRectangle();
    }

    void PatrolInRectangle()
    {
        Vector2 position = rb.position;

        if (Vector2.Distance(position, targetPosition) < 0.1f)
        {
            // Cambiar al siguiente punto en el ciclo
            currentTargetIndex = (currentTargetIndex + 1) % patrolPoints.Length;
            targetPosition = patrolPoints[currentTargetIndex].position;
        }

        // Raycast para detectar borde (opcional)
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, edgeDetectionDistance, groundLayer);
        Debug.DrawRay(groundCheck.position, Vector2.down * edgeDetectionDistance, Color.red);
        if (groundInfo.collider == false)
        {
            Flip();
        }

        MoveTowards(targetPosition, speed);
    }

    void MoveTowards(Vector2 target, float moveSpeed)
    {
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.deltaTime);
        rb.MovePosition(newPos);

        Vector2 direction = (target - rb.position).normalized;
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        Vector3 groundCheckPos = groundCheck.localPosition;
        groundCheckPos.x *= -1;
        groundCheck.localPosition = groundCheckPos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
         if (collision.gameObject.CompareTag("Player"))
        {
            // Activar animación de ataque
            if(animator != null)
                animator.SetTrigger("Attack");

            AxelMovement playerScript = collision.gameObject.GetComponent<AxelMovement>();
            if (playerScript != null)
            {
                playerScript.RecibeDanio(transform.position, 10);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lanza"))
        {
            Vector2 direccionDanio = collision.transform.position;
            RecibeDanio(direccionDanio, 10); 
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (isDead || isTakingDamage)
            return;

        isTakingDamage = true;

        currentHealth -= cantDanio;
        Debug.Log("Vida enemigo volador: " + currentHealth);

        if (currentHealth <= 0)
        {
            Morir();
            return;
        }

        float direccionX = Mathf.Sign(transform.position.x - direccion.x);
        Vector2 rebote = new Vector2(direccionX, 0).normalized;
        rb.AddForce(rebote * knockbackForce, ForceMode2D.Impulse);

        Invoke(nameof(DesactivaDanio), 0.4f);
    }

    private void DesactivaDanio()
    {
        isTakingDamage = false;
        rb.linearVelocity = Vector2.zero;
    }

    private void Morir()
    {
        if (isDead) return;
        isDead = true;
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Desactivar movimiento y colisiones para que no siga interactuando
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        this.enabled = false;

        // Destruir después de que termine la animación de muerte (ajusta tiempo)
        Destroy(gameObject, 1.5f);

        if (scoreManager != null)
        {
            ScoreVisualManager.Instance.SumarPuntosPorEnemigo();

        }
    }
}
