using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AxelMovement : MonoBehaviour
{
    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private float Horizontal;
    public float JumpForce;
    public float fuerzaRebote = 10f;
    public float Speed;

    private bool Grounded;
    private bool isDead = false;
    private bool atacando;
    private bool Recibedanio;

    public int maxHealth = 100;
    private int currentHealth;
    [Header("Audio")]
    public AudioSource attackAudioSource;
    public AudioClip[] attackClips;  // Array de clips para el ataque
    private int currentAttackAudioIndex = 0;

    // Parámetros para detectar el suelo
    public float longitudRaycast = 0.2f;
    public LayerMask Ground;

    private int maxSaltos = 2;  // máximo 2 saltos (salto simple + doble salto)
    private int saltosRestantes;
    

    public int CurrentHealth { get { return currentHealth; } }


    [Header("Escalar")]
    [SerializeField] private float velocidadEscalar;
    private CapsuleCollider2D capsuleCollider2D;
    private float gravedadInicial;
    private bool escalando;
    private Vector2 input;

    [Header("Escaleras horizontales")]
    [SerializeField] private float velocidadEscalarHorizontal;
    private bool escalandoHorizontal;
    private MusicManager2 musicManager;

   void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        saltosRestantes = maxSaltos;

        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        gravedadInicial = Rigidbody2D.gravityScale;

        // Buscar la instancia de MusicManager2 en la escena
        musicManager = Object.FindFirstObjectByType<MusicManager2>();

        // Asegurarte que la música se esté reproduciendo al inicio
        if (musicManager != null && !musicManager.IsPlaying())
        {
            musicManager.PlayMusic();
        }
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (!atacando)
        {
            Movimiento();
            Escalar();
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
        if (Input.GetKeyDown(KeyCode.W) && saltosRestantes > 0 && !Recibedanio)
        {
            Jump();
            Animator.SetTrigger("Jump");
            saltosRestantes--;  // resta un salto
        }

        if (Input.GetKeyDown(KeyCode.U) && !atacando && Grounded && Horizontal == 0f)
        {
            Atacando();
        }
        if (escalando) return; // No moverse horizontalmente mientras escalas
        Horizontal = Input.GetAxisRaw("Horizontal");
    }

    private void Jump()
    {
        Rigidbody2D.linearVelocity = new Vector2(Rigidbody2D.linearVelocity.x, 0f);
        Rigidbody2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        if (escalando)
        {
            Rigidbody2D.linearVelocity = new Vector2(input.x * Speed, input.y * velocidadEscalar);
        }
        else if (escalandoHorizontal)
        {
            Rigidbody2D.linearVelocity = new Vector2(input.x * velocidadEscalarHorizontal, Rigidbody2D.linearVelocity.y);
        }
        else
        {
            Rigidbody2D.linearVelocity = new Vector2(input.x * Speed, Rigidbody2D.linearVelocity.y);
            Rigidbody2D.gravityScale = gravedadInicial;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tentacles"))
        {
            TakeDamage(currentHealth);
        }
        else if (collision.CompareTag("Golem"))
        {
            GolemController golem = collision.GetComponent<GolemController>();
            if (golem != null)
            {
                Vector2 direccionDanio = transform.position;
                int damage = 10;
                golem.RecibeDanio(direccionDanio, damage);
            }
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

        // Pausar la música al morir
        if (musicManager != null)
        {
            musicManager.StopMusic();
        }

        Invoke(nameof(RestartScene), 3.5f);
    }

    public void Atacando()
    {
        atacando = true;

        if (attackAudioSource != null && attackClips != null && attackClips.Length > 0)
        {
            attackAudioSource.PlayOneShot(attackClips[currentAttackAudioIndex]);
            currentAttackAudioIndex = (currentAttackAudioIndex + 1) % attackClips.Length;
        }
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
            TakeDamage(cantDanio);

            // Rebote al recibir daño
            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1.0f).normalized;
            Rigidbody2D.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);

            // Activa animación de recibir daño
            Animator.SetBool("Recibedanio", true);

            // Invoca para desactivar el estado de daño después de 0.5 segundos
            Invoke(nameof(DesactivaDanio), 0.5f);

        }
    }

    public void DesactivaDanio()
    {
        Recibedanio = false;
        Animator.SetBool("Recibedanio", false);
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

    private void Escalar()
    {
        bool estaEnEscaleraVertical = capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Escaleras"));
        bool estaEnEscaleraHorizontal = capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("EscalerasHorizontales"));

        if (estaEnEscaleraVertical)
        {
            escalando = true;
            escalandoHorizontal = false;

            Rigidbody2D.gravityScale = 0f;
            Rigidbody2D.linearVelocity = new Vector2(input.x * Speed, input.y * velocidadEscalar);

            Animator.SetBool("Climbing", true);
            Animator.SetBool("ClimbingHorizontal", false);
        }
        else if (estaEnEscaleraHorizontal)
        {
            escalandoHorizontal = true;
            escalando = false;

            Rigidbody2D.gravityScale = 0f;
            Rigidbody2D.linearVelocity = new Vector2(input.x * velocidadEscalarHorizontal, Rigidbody2D.linearVelocity.y);

            Animator.SetBool("ClimbingHorizontal", true);
            Animator.SetBool("Climbing", false);
        }
        else
        {
            if (escalando || escalandoHorizontal)
            {
                escalando = false;
                escalandoHorizontal = false;

                Rigidbody2D.gravityScale = gravedadInicial;
                Animator.SetBool("Climbing", false);
                Animator.SetBool("ClimbingHorizontal", false);
            }
        }
    }
    
    public void HealFull()
    {
        currentHealth = maxHealth;
        Debug.Log("Vida restaurada al máximo: " + currentHealth);
    }

}
