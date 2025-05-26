using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AxelMovement : MonoBehaviour
{
    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private float Horizontal;
    public float JumpForce;
    public float fuerzaRebote=10f;
    public float Speed;

    private bool Grounded;
    private bool isDead = false;
    private bool atacando;
    private bool Recibedanio;

    public int maxHealth = 100;
    private int currentHealth;

    // Parámetros para detectar el suelo
    public float longitudRaycast = 0.2f;
    public LayerMask Ground;

    private int maxSaltos = 2;  // máximo 2 saltos (salto simple + doble salto)
    private int saltosRestantes;

    public int CurrentHealth { get { return currentHealth; } }

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        saltosRestantes = maxSaltos;
    }

    void Update()
    {
        if (!atacando)
        {
            Movimiento();
        }
        
        Animator.SetBool("Atacando", atacando);
        Animator.SetBool("Grounded", Grounded);
        Debug.Log("Grounded: " + Grounded);
        Debug.DrawRay(transform.position + Vector3.down * 0.5f, Vector3.down * longitudRaycast, Color.red);
        Animator.SetBool("Recibedanio", Recibedanio);
    }
    public void Movimiento()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        if (Horizontal < 0.0f) transform.localScale = new Vector3(-7.0f, 7.0f, 1.0f);
        else if (Horizontal > 0.0f) transform.localScale = new Vector3(7.0f, 7.0f, 1.0f);

        Animator.SetBool("Running", Horizontal != 0.0f);

        // Raycast para detectar el suelo
        Grounded = Physics2D.Raycast(transform.position + Vector3.down * 0.5f, Vector2.down, longitudRaycast, Ground);

        if (Grounded)
        {
            saltosRestantes = maxSaltos;
        }

        // Saltar si presiona W y tiene saltos disponibles
        if (Input.GetKeyDown(KeyCode.W) && saltosRestantes > 0 &&!Recibedanio)
        {
            Jump();
            Animator.SetTrigger("Jump");
            saltosRestantes--;  // resta un salto
        }

       if (Input.GetKeyDown(KeyCode.U) && !atacando && Grounded && Horizontal == 0f)
        {
            Atacando();
        }
    }
    
    private void Jump()
    {
        Rigidbody2D.linearVelocity = new Vector2(Rigidbody2D.linearVelocity.x, 0f);
        Rigidbody2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = new Vector2(Horizontal * Speed, Rigidbody2D.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tentacles"))
        {
            TakeDamage(currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Vida actual: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Animator.SetBool("IsDead", true);

        Rigidbody2D.linearVelocity = Vector2.zero;
        this.enabled = false;

        Invoke(nameof(RestartScene), 3.5f);
    }

    public void Atacando()
    {
        atacando = true;
    }

    public void DesactivaAtaque()
    {
        atacando = false;
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!Recibedanio)
        {
            Recibedanio = true;
            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.5f).normalized;
            Rigidbody2D.AddForce(rebote*fuerzaRebote, ForceMode2D.Impulse);
        }
        
    }

    public void DesactivaDanio()
    {
        Recibedanio = false;
        Rigidbody2D.linearVelocity = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        if (Rigidbody2D == null) return;

        // Punto inicial del raycast
        Vector3 origen = transform.position + Vector3.down * 0.5f;
        // Dirección hacia abajo
        Vector3 direccion = Vector3.down * longitudRaycast;

        // Dibuja el raycast en rojo
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origen, origen + direccion);
    }

}
