using UnityEngine;
using TMPro;

public class ScoreVisualManager : MonoBehaviour
{
    public int puntos = 0;  // puntos iniciales
    public int puntosPorEnemigo = 10;
    public int puntosParaGanar = 20;

    private TextMeshProUGUI textMesh;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        ActualizarTexto();
    }

    // Llamar esta función cuando se mate a un enemigo
    public void SumarPuntosPorEnemigo()
    {
        puntos += puntosPorEnemigo;
        ActualizarTexto();

        if (puntos >= puntosParaGanar)
        {
            TerminarJuego();
        }
    }

    private void ActualizarTexto()
    {
        if (textMesh != null)
        {
            textMesh.text = "Puntaje: " + puntos.ToString();
        }
    }

    private void TerminarJuego()
    {
        Debug.Log("¡Has ganado! Puntaje máximo alcanzado.");
    }
}
