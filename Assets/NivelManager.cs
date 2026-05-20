using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NivelManager : MonoBehaviour
{
    public static NivelManager Instance;

    [Header("UI de selección de poderes")]
    public GameObject panelSeleccionPoder;
    public TextMeshProUGUI textoNivel;
    public TextMeshProUGUI textoTituloSeleccion;
    public Image[] iconosPoder;
    public TextMeshProUGUI[] nombresPoder;
    public TextMeshProUGUI[] descripcionesPoder;
    public Button[] botonesPoder;

    [Header("Pantalla de carga")]
    public GameObject pantallaCarga;
    public TextMeshProUGUI textoCarga;
    public float duracionPantallaCarga = 1.25f;

    [Header("Game Over UI")]
    public GameObject panelGameOver;
    public TextMeshProUGUI textoScoreFinal;

    [Header("Configuración")]
    public int opcionesPorNivel = 3;

    private List<PowerUpData> opcionesActuales = new List<PowerUpData>();
    private int jugadorElegiendoPoder = 1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (panelSeleccionPoder != null)
            panelSeleccionPoder.SetActive(false);

        if (pantallaCarga != null)
            pantallaCarga.SetActive(false);

        if (panelGameOver != null)
            panelGameOver.SetActive(false);
    }

    public void MostrarPantallaSeleccion()
    {
        jugadorElegiendoPoder = 1;
        MostrarPantallaParaJugadorActual();
    }

    private void MostrarPantallaParaJugadorActual()
    {
        Debug.Log("[NivelManager] Poderes: " + (PowerUpManager.Instance != null ? PowerUpManager.Instance.todosLosPoderes.Count.ToString() : "PowerUpManager NULO"));
        Debug.Log("[NivelManager] Botones: " + (botonesPoder != null ? botonesPoder.Length.ToString() : "NULL"));

        if (panelSeleccionPoder == null)
        {
            Debug.LogError("[NivelManager] panelSeleccionPoder es NULO!");
            return;
        }

        if (PowerUpManager.Instance == null)
        {
            Debug.LogError("[NivelManager] PowerUpManager.Instance es NULO!");
            return;
        }

        if (PowerUpManager.Instance.todosLosPoderes == null || PowerUpManager.Instance.todosLosPoderes.Count == 0)
        {
            Debug.LogError("[NivelManager] todosLosPoderes está vacío o es NULO! Asegúrate de asignar los ScriptableObjects en el Inspector del PowerUpManager.");
            return;
        }

        if (botonesPoder == null || botonesPoder.Length == 0)
        {
            Debug.LogError("[NivelManager] botonesPoder está vacío o es NULO!");
            return;
        }

        Time.timeScale = 0f;
        panelSeleccionPoder.SetActive(true);

        if (textoNivel != null)
            textoNivel.text = "Nivel " + (GameManager.Instance.rondaActual + 1);

        if (textoTituloSeleccion != null)
        {
            bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;
            if (esMulti)
                textoTituloSeleccion.text = "Elige un poder, Jugador " + jugadorElegiendoPoder;
            else
                textoTituloSeleccion.text = "Elige un poder";
        }

        opcionesActuales = ElegirPoderesAleatorios();

        Debug.Log("[NivelManager] Opciones elegidas: " + opcionesActuales.Count);

        for (int i = 0; i < botonesPoder.Length; i++)
        {
            if (botonesPoder[i] == null)
            {
                Debug.LogWarning("[NivelManager] botonesPoder[" + i + "] es NULO!");
                continue;
            }

            if (i < opcionesActuales.Count)
            {
                PowerUpData poder = opcionesActuales[i];
                
                if (poder == null)
                {
                    Debug.LogWarning("[NivelManager] poder en índice " + i + " es NULO!");
                    botonesPoder[i].gameObject.SetActive(false);
                    continue;
                }

                Debug.Log("[NivelManager] Configurando botón " + i + " con poder: " + poder.nombre);

                if (iconosPoder != null && i < iconosPoder.Length && iconosPoder[i] != null)
                {
                    iconosPoder[i].sprite = poder.icono;
                    iconosPoder[i].enabled = true;
                    Debug.Log("[NivelManager] Icono asignado: " + (poder.icono != null ? poder.icono.name : "NULL"));
                }
                else
                {
                    Debug.LogWarning("[NivelManager] iconosPoder[" + i + "] es NULO o no existe!");
                }

                if (nombresPoder != null && i < nombresPoder.Length && nombresPoder[i] != null)
                {
                    nombresPoder[i].text = poder.nombre;
                    nombresPoder[i].enabled = true;
                    nombresPoder[i].gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("[NivelManager] nombresPoder[" + i + "] es NULO o no existe!");
                }

                if (descripcionesPoder != null && i < descripcionesPoder.Length && descripcionesPoder[i] != null)
                {
                    descripcionesPoder[i].text = poder.descripcion;
                    descripcionesPoder[i].enabled = true;
                    descripcionesPoder[i].gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("[NivelManager] descripcionesPoder[" + i + "] es NULO o no existe!");
                }

                Button boton = botonesPoder[i];
                boton.gameObject.SetActive(true);
                boton.onClick.RemoveAllListeners();
                int opcionIndex = i;
                boton.onClick.AddListener(() => SeleccionarPoder(opcionIndex));
            }
            else
            {
                botonesPoder[i].gameObject.SetActive(false);
            }
        }
    }

    private List<PowerUpData> ElegirPoderesAleatorios()
    {
        List<PowerUpData> poderDisponible = new List<PowerUpData>();
        ControlAvion jugador = FindObjectOfType<ControlAvion>();

        Debug.Log("[NivelManager] Total poderes en PowerUpManager: " + PowerUpManager.Instance.todosLosPoderes.Count);

        foreach (PowerUpData p in PowerUpManager.Instance.todosLosPoderes)
        {
            if (p == null)
            {
                Debug.LogWarning("[NivelManager] Encontrado poder NULO en la lista, saltando...");
                continue;
            }

            if (p.tipo == PowerUpType.DobleDisparo && jugador != null && jugador.tieneDobleDisparo)
            {
                Debug.Log("[NivelManager] Excluyendo DobleDisparo porque el jugador ya lo tiene");
                continue;
            }
            
            Debug.Log("[NivelManager] Poder disponible: " + p.nombre);
            poderDisponible.Add(p);
        }

        Debug.Log("[NivelManager] Poderes disponibles después de filtrar: " + poderDisponible.Count);

        List<PowerUpData> elegidos = new List<PowerUpData>();

        for (int i = 0; i < opcionesPorNivel && poderDisponible.Count > 0; i++)
        {
            int indice = Random.Range(0, poderDisponible.Count);
            elegidos.Add(poderDisponible[indice]);
            Debug.Log("[NivelManager] Poder elegido " + i + ": " + poderDisponible[indice].nombre);
            poderDisponible.RemoveAt(indice);
        }

        return elegidos;
    }

    public void SeleccionarPoder(int indice)
    {
        if (indice < 0 || indice >= opcionesActuales.Count)
            return;

        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.ApplyPowerUp(opcionesActuales[indice], jugadorElegiendoPoder);
        }

        bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;

        if (esMulti && jugadorElegiendoPoder == 1)
        {
            jugadorElegiendoPoder = 2;
            MostrarPantallaParaJugadorActual();
        }
        else
        {
            if (panelSeleccionPoder != null)
                panelSeleccionPoder.SetActive(false);

            Time.timeScale = 1f;
            StartCoroutine(TransicionANuevaRonda());
        }
    }

    IEnumerator TransicionANuevaRonda()
    {
        if (pantallaCarga != null)
        {
            pantallaCarga.SetActive(true);
            if (textoCarga != null)
                textoCarga.text = "NIVEL " + (GameManager.Instance.rondaActual + 1);
        }

        yield return new WaitForSecondsRealtime(duracionPantallaCarga);

        if (pantallaCarga != null)
            pantallaCarga.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.SiguienteRonda();

        if (CombatDirector.Instance != null)
            CombatDirector.Instance.IniciarRonda();
    }

    public void MostrarGameOver(int scoreFinal)
    {
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }
        if (textoScoreFinal != null)
        {
            textoScoreFinal.text = "SCORE FINAL:\n" + scoreFinal;
        }
    }

    public void BotonReintentar()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null) GameManager.Instance.ResetearEstadisticas();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void BotonMenuPrincipal()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null) GameManager.Instance.ResetearEstadisticas();
        // Asegúrate de que tu escena de menú se llame "MenuPrincipal" en los Build Settings
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipal"); 
    }
}
