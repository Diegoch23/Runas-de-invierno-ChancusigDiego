using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;  // Importa para manejo de escenas

public class ScoreVisualManager : MonoBehaviour
{
    public int puntos = 0;  // puntos iniciales
    public int puntosPorEnemigo = 10;
    public int puntosParaGanar = 30;

    public string escenaFinal = "FinalScene"; 

    private TextMeshProUGUI textMesh;
    public static ScoreVisualManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        puntos = 0;
        ActualizarTexto();
        Debug.Log("Puntaje inicial: " + puntos);
    }

    // Llamar esta función cuando se mate a un enemigo
    public void SumarPuntosPorEnemigo()
    {
        Debug.Log($"Sumando puntos. Puntos antes: {puntos}");
        puntos += puntosPorEnemigo;
        Debug.Log($"Puntos después: {puntos}");
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
        SceneManager.LoadScene(escenaFinal);  // Carga la escena final
    }
}
