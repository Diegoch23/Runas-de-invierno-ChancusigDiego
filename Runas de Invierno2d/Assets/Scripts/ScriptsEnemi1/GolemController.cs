using UnityEngine;

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

    private Animator animator;

    void Start()
    {
        groundCheck = new GameObject("GroundCheck").transform;
        groundCheck.parent = transform;
        groundCheck.localPosition = new Vector3(-0.05f, -0.5f, 0);

        animator = GetComponent<Animator>();
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
}
