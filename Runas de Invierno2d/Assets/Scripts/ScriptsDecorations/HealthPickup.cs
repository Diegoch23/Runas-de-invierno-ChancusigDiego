using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private bool playerInRange = false;
    private bool used = false;  // Para saber si ya se usó

    void Update()
    {
        if (playerInRange && !used && Input.GetKeyDown(KeyCode.Space))
        {
            // Obtener script AxelMovement del jugador
            AxelMovement player = GameObject.FindGameObjectWithTag("Player").GetComponent<AxelMovement>();
            if (player != null)
            {
                player.HealFull();  // Método que debes crear en AxelMovement para restaurar vida completa
                used = true;
                // Opcional: desactivar el objeto o la interacción visual
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            // Opcional: mostrar UI de "Presiona Espacio para curarte"
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            // Opcional: ocultar UI
        }
    }
}
