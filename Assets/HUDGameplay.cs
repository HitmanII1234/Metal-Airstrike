using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDGameplay : MonoBehaviour
{
    public static HUDGameplay Instance;

    [Header("Referencias de Jugador 1 (Top-Left)")]
    public Image barraVidaP1;
    public TextMeshProUGUI textoNombreP1;

    [Header("Referencias de Jugador 2 (Opcional - Multiplayer)")]
    public GameObject contenedorHUDP2; // Contenedor para ocultar/mostrar el HUD del P2
    public Image barraVidaP2;
    public TextMeshProUGUI textoNombreP2;

    [Header("Referencias de Estadísticas (Top-Right) - Jugador 1")]
    public TextMeshProUGUI textoScore;
    public TextMeshProUGUI textoNivel;

    [Header("Referencias de Estadísticas - Jugador 2 (Solo Multijugador)")]
    public TextMeshProUGUI textoScoreP2;
    public TextMeshProUGUI textoNivelP2;

    private Salud saludP1;
    private Salud saludP2;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Configurar HUD según si es multijugador o no
        bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;
        if (contenedorHUDP2 != null)
        {
            contenedorHUDP2.SetActive(esMulti);
        }

        // Asignar nombres en el HUD principal al iniciar
        if (GameManager.Instance != null)
        {
            if (textoNombreP1 != null)
            {
                textoNombreP1.text = !string.IsNullOrEmpty(GameManager.Instance.playerName) 
                    ? GameManager.Instance.playerName 
                    : "Jugador 1";
            }

            if (textoNombreP2 != null && esMulti)
            {
                textoNombreP2.text = !string.IsNullOrEmpty(GameManager.Instance.player2Name) 
                    ? GameManager.Instance.player2Name 
                    : "Jugador 2";
            }
        }
    }

    void Update()
    {
        // 1. Actualizar Score y Nivel (Top-Right)
        if (GameManager.Instance != null)
        {
            string scoreTexto = "SCORE: " + GameManager.Instance.score;
            string nivelTexto = "NIVEL: " + GameManager.Instance.rondaActual;

            if (textoScore != null)
                textoScore.text = scoreTexto;

            if (textoNivel != null)
                textoNivel.text = nivelTexto;

            // En multijugador, el P2 comparte el mismo score global
            if (textoScoreP2 != null)
                textoScoreP2.text = scoreTexto;

            if (textoNivelP2 != null)
                textoNivelP2.text = nivelTexto;
        }

        // 2. Buscar al Jugador 1 si no ha sido asignado aún
        if (saludP1 == null)
        {
            // Intentar encontrar por nombre común de clon de prefab, o por tag
            GameObject p1Obj = GameObject.Find("Player1(Clone)") ?? GameObject.Find("Player1");
            if (p1Obj == null)
            {
                // Fallback a buscar por tag "Player"
                p1Obj = GameObject.FindGameObjectWithTag("Player");
            }

            if (p1Obj != null)
            {
                saludP1 = p1Obj.GetComponent<Salud>();
            }
        }

        // Actualizar barra de vida P1
        if (saludP1 != null && barraVidaP1 != null)
        {
            barraVidaP1.fillAmount = saludP1.ObtenerPorcentajeVida();
        }

        // 3. Si es multijugador, buscar y actualizar al Jugador 2
        bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;
        if (esMulti)
        {
            if (saludP2 == null)
            {
                GameObject p2Obj = GameObject.Find("Player2(Clone)") ?? GameObject.Find("Player2");
                if (p2Obj != null)
                {
                    saludP2 = p2Obj.GetComponent<Salud>();
                }
            }

            if (saludP2 != null && barraVidaP2 != null)
            {
                barraVidaP2.fillAmount = saludP2.ObtenerPorcentajeVida();
            }
        }
    }
}
