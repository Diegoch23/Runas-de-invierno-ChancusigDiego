using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    public Image rellenoBarraVida;
    private AxelMovement playerController;
    private float vidaMaxima;

    void Start()
    {
        playerController = GameObject.Find("Axel").GetComponent<AxelMovement>();
        vidaMaxima = playerController.maxHealth;
    }

    void Update()
    {
        rellenoBarraVida.fillAmount = (float)playerController.CurrentHealth / vidaMaxima;
    }
}
