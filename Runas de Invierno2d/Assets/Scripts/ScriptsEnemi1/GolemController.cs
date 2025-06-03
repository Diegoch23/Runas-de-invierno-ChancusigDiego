using UnityEngine;
using System.Collections;

public class GolemController : MonoBehaviour
{
    public float speed = 2f;
    public float edgeDetectionDistance = 0.1f;
    public LayerMask groundLayer;

    private bool movingRight = true;
    private Transform groundCheck;

    public int damageAmount = 20;
    public float attackCooldown = 1f;  // Tiempo entre ataques
    private float lastAttackTime;

    private bool playerInRange = false;
    private GameObject player;

    public int maxHealth = 50;
    private int currentHealth;
    private bool Recibedanio = false;
    public float fuerzaRebote = 4f;
    private Rigidbody2D rb;
    public ScoreVisualManager scoreManager;
    private Animator animator;
    private bool isDead = false;
    

    void Start()
    {
        groundCheck = new GameObject("GroundCheck").transform;
        groundCheck.parent = transform;
        groundCheck.localPosition = new Vector3(-0.05f, -0.5f, 0);

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (playerInRange && player != null)
        {
            animator.SetBool("Attack", true);

            // Mirar hacia el jugador
            LookAtPlayer();

            // Atacar solo si ha pasado el cooldown
            if (Time.time > lastAttackTime + attackCooldown)
            {
                AxelMovement playerScript = player.GetComponent<AxelMovement>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage(damageAmount);
                    lastAttackTime = Time.time;
                }
            }
        }
        else
        {
            animator.SetBool("Attack", false);

            // Patrullar
            transform.Translate(Vector2.right * speed * Time.deltaTime * (movingRight ? 1 : -1));

            RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, edgeDetectionDistance, groundLayer);

            if (groundInfo.collider == false)
            {
                Flip();
            }
        }
    }

    void LookAtPlayer()
    {
        // Si el jugador está a la derecha del golem, mirar hacia la derecha (escala positiva)
        if (player.transform.position.x > transform.position.x && transform.localScale.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        // Si el jugador está a la izquierda, mirar hacia la izquierda (escala negativa)
        else if (player.transform.position.x < transform.position.x && transform.localScale.x > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        Vector3 groundCheckPos = groundCheck.localPosition;
        groundCheckPos.x *= -1;
        groundCheck.localPosition = groundCheckPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * edgeDetectionDistance);
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (isDead || Recibedanio)  // Agrega la condición isDead para no recibir daño si ya está muerto
            return;

        Recibedanio = true;

        currentHealth -= cantDanio;
        Debug.Log("Vida Golem: " + currentHealth);

        if (currentHealth <= 0)
        {
            Morir();
            return;
        }

        float direccionX = Mathf.Sign(transform.position.x - direccion.x);
        Vector2 rebote = new Vector2(direccionX, 0).normalized;
        rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);

        animator.SetTrigger("Hurt");

        StartCoroutine(DesactivaDanio());
    }
    
    private IEnumerator DesactivaDanio()
    {
        yield return new WaitForSeconds(0.4f);
        Recibedanio = false;
        rb.linearVelocity = Vector2.zero;
    }

    private void Morir()
    {
        if (isDead) return;  // Asegura que no se ejecute más de una vez
        isDead = true;       // Marca como muerto

        transform.position += new Vector3(0, -0.5f, 0);
        animator.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        this.enabled = false;

        Destroy(gameObject, 1.5f);
        ScoreVisualManager.Instance.SumarPuntosPorEnemigo();

    }
}
