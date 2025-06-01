using System.Collections;
using UnityEngine;

public class OrcController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;
    public float attackRange = 1.0f;
    public bool Recibedanio;
    public float fuerzaRebote = 4f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    public int maxHealth = 20;
    private int currentHealth;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer < attackRange)
            {
                movement = Vector2.zero;
                animator.SetBool("IsRunning", false);
                animator.SetTrigger("Attack");
            }
            else if (distanceToPlayer < detectionRadius)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                movement = new Vector2(direction.x, 0);
                animator.SetBool("IsRunning", true);

                // Giro del enemigo
                if (direction.x < 0)
                    transform.localScale = new Vector3(-6, 6, 1);
                else
                    transform.localScale = new Vector3(6, 6, 1);
            }
            else
            {
                movement = Vector2.zero;
                animator.SetBool("IsRunning", false);
            }
        }
            else
            {
                movement = Vector2.zero; // Detener movimiento mientras ataca
            }
    }

    private void FixedUpdate()
    {
        if (!Recibedanio)
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Attack");
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
             movement = Vector2.zero;
            collision.gameObject.GetComponent<AxelMovement>().RecibeDanio(direccionDanio, 5);
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lanza"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);
            RecibeDanio(direccionDanio, 5);
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!Recibedanio)
        {
            Recibedanio = true;

            currentHealth -= cantDanio;
            Debug.Log("Vida enemigo: " + currentHealth);

            if (currentHealth <= 0)
            {
                Morir();
                return;
            }

            // Rebote solo horizontal, sin componente vertical
            float direccionX = Mathf.Sign(transform.position.x - direccion.x);
            Vector2 rebote = new Vector2(direccionX, 0).normalized;
            rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);

            StartCoroutine(DesactivaDanio());
        }
    }

    private void Morir()
    {
        animator.SetTrigger("Die"); // Activa la animación de muerte
        Vector3 nuevaPos = transform.position;
        nuevaPos.y = -27.51f; // Cambia este valor al Y deseado para el suelo
        transform.position = nuevaPos;
        // Opcional: desactivar colisiones y movimiento
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        this.enabled = false;

        // Destruir el objeto tras un delay para que termine la animación (ejemplo 1.5s)
        Destroy(gameObject, 1.5f);
    }


    IEnumerator DesactivaDanio()
    {
        yield return new WaitForSeconds(0.4f);
        Recibedanio = false;
        rb.linearVelocity = Vector2.zero;
    }
}
