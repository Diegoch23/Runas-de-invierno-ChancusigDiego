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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
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

            // Aquí el giro del enemigo según la posición del jugador
            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-6, 6, 1); // Mira a la izquierda
            }
            else if (direction.x > 0)
            {
                transform.localScale = new Vector3(6, 6, 1); // Mira a la derecha
            }
        }
        else
        {
            movement = Vector2.zero;
            animator.SetBool("IsRunning", false);
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
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
            collision.gameObject.GetComponent<AxelMovement>().RecibeDanio(direccionDanio, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lanza"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);
            RecibeDanio(direccionDanio, 0);
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!Recibedanio)
        {
            Recibedanio = true;
            // Rebote solo horizontal, sin componente vertical
            float direccionX = Mathf.Sign(transform.position.x - direccion.x);
            Vector2 rebote = new Vector2(direccionX, 0).normalized;
            rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
            StartCoroutine(DesactivaDanio());
        }
    }
    IEnumerator DesactivaDanio()
    {
        yield return new WaitForSeconds(0.4f);
        Recibedanio = false;
        rb.linearVelocity = Vector2.zero;
    }
}
