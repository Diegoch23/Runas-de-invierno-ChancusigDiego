using UnityEngine;

public class Guadania : MonoBehaviour
{
    public float fuerzaEmpuje = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 direccionEmpuje = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(direccionEmpuje * fuerzaEmpuje, ForceMode2D.Impulse);

                AxelMovement player = collision.GetComponent<AxelMovement>();
                if (player != null)
                {
                    player.RecibeDanio(direccionEmpuje, 5);
                }
            }
        }
    }

    public void ActivarGuadania()
    {
        GetComponent<Collider2D>().enabled = true;
    }

    public void DesactivarGuadania()
    {
        GetComponent<Collider2D>().enabled = false;
    }
}
