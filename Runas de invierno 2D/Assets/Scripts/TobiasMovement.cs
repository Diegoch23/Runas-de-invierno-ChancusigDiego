using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TobiasMovement : MonoBehaviour
{
    public float JumpForce = 5f;
    public float Speed = 10f;
    public int combo = 0;
    public bool isAttacking = false;
    public Animator animator; // Asegúrate de tener un componente Animator en tu personaje
    public AudioSource audioSource; // Asegúrate de tener un componente AudioSource en tu personaje
    public AudioClip[] attackSounds; // Arrastra tus clips de audio de ataque aquí

    private Rigidbody2D rb;
    private float horizontal;
    private bool grounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject!");
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on this GameObject!");
        }
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal < 0.0f) transform.localScale = new Vector3(-4.261569f, 4.28919f, 1.0f);
        else if (horizontal > 0.0f) transform.localScale = new Vector3(4.261569f, 4.28919f, 1.0f);

        animator.SetBool("Running", horizontal != 0.0f);
        animator.SetBool("IsJumping", !grounded);

        Debug.DrawRay(transform.position, Vector3.down * 0.1f, Color.red);
        grounded = Physics2D.Raycast(transform.position, Vector3.down, 0.1f);

        if (Input.GetKeyDown(KeyCode.W) && Mathf.Abs(rb.velocity.y) < 0.01f && grounded)
        {
            Jump();
        }

        // Lógica de Combos
        if (Input.GetKeyDown(KeyCode.C))
        {
            isAttacking = true;
            combo++;
            if (combo > 3) // Puedes ajustar el número máximo de combos
            {
                combo = 1;
            }
            animator.SetTrigger(combo.ToString()); // Dispara el trigger de la animación del combo (ej: "1", "2", "3")

            // Reproducir sonido de ataque si hay clips disponibles
            if (attackSounds != null && attackSounds.Length > 0 && combo <= attackSounds.Length)
            {
                audioSource.clip = attackSounds[combo - 1]; // Los arrays son base 0, el combo base 1
                audioSource.Play();
            }
            else if (attackSounds == null || attackSounds.Length == 0)
            {
                Debug.LogWarning("No attack sounds assigned to the character.");
            }
            else if (combo > attackSounds.Length)
            {
                Debug.LogWarning("No attack sound available for combo number: " + combo);
            }
        }
    }

    private void Jump()
    {
        animator.SetBool("IsJumping", true);
        rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * Speed, rb.velocity.y);
    }

    // Esta función será llamada por un Evento de Animación al final de cada animación de ataque
    public void FinishAttackAnimation()
    {
        isAttacking = false;
        combo = 0;
    }
}