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
    public GameObject panelSeleccionPoderP2;
    public TextMeshProUGUI textoNivel;
    public TextMeshProUGUI textoNivelP2;
    public TextMeshProUGUI textoTituloSeleccion;
    public TextMeshProUGUI textoTituloSeleccionP2;
    public Image[] iconosPoder;
    public Image[] iconosPoderP2;
    public TextMeshProUGUI[] nombresPoder;
    public TextMeshProUGUI[] nombresPoderP2;
    public TextMeshProUGUI[] descripcionesPoder;
    public TextMeshProUGUI[] descripcionesPoderP2;
    public Button[] botonesPoder;
    public Button[] botonesPoderP2;

    [Header("Pantalla de carga")]
    public GameObject pantallaCarga;
    public TextMeshProUGUI textoCarga;
    public float duracionPantallaCarga = 1.25f;

    [Header("Game Over UI")]
    public GameObject panelGameOver;
    public TextMeshProUGUI textoScoreFinal;

    [Header("Aviso de Subida de Nivel")]
    public GameObject panelAvisoNivel;
    public TextMeshProUGUI textoAvisoNivel;
    public float duracionAvisoNivel = 2f;

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

        if (panelSeleccionPoderP2 != null)
            panelSeleccionPoderP2.SetActive(false);

        if (pantallaCarga != null)
            pantallaCarga.SetActive(false);

        if (panelGameOver != null)
            panelGameOver.SetActive(false);

        if (panelAvisoNivel == null)
        {
            CrearPanelAvisoNivelAutomatico();
        }
        else
        {
            panelAvisoNivel.SetActive(false);
        }
    }

    void CrearPanelAvisoNivelAutomatico()
    {
        GameObject canvas = FindObjectOfType<Canvas>()?.gameObject;
        if (canvas == null)
        {
            Debug.LogError("[NivelManager] No se encontró Canvas para crear el aviso de nivel");
            return;
        }

        panelAvisoNivel = new GameObject("PanelAvisoNivel");
        panelAvisoNivel.transform.SetParent(canvas.transform, false);

        RectTransform rect = panelAvisoNivel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(600, 100);
        rect.anchoredPosition = Vector2.zero;

        Image bg = panelAvisoNivel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);

        GameObject txtObj = new GameObject("TextoAviso");
        txtObj.transform.SetParent(panelAvisoNivel.transform, false);

        RectTransform txtRect = txtObj.AddComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.sizeDelta = Vector2.zero;

        textoAvisoNivel = txtObj.AddComponent<TextMeshProUGUI>();
        textoAvisoNivel.alignment = TextAlignmentOptions.Center;
        textoAvisoNivel.fontSize = 36;
        textoAvisoNivel.color = Color.yellow;
        textoAvisoNivel.fontStyle = FontStyles.Bold;
        textoAvisoNivel.text = "";

        panelAvisoNivel.SetActive(false);
        Debug.Log("[NivelManager] Panel de aviso de nivel creado automáticamente");
    }

    public void MostrarPantallaSeleccion()
    {
        jugadorElegiendoPoder = 1;
        bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;
        
        if (esMulti)
        {
            MostrarPantallaParaJugador(1);
            MostrarPantallaParaJugador(2);
        }
        else
        {
            Debug.Log("[NivelManager] Modo 1 jugador - Panel de mejoras desactivado, continuando automáticamente");
            Time.timeScale = 1f;
            if (GameManager.Instance != null) GameManager.Instance.SiguienteRonda();
            if (CombatDirector.Instance != null) CombatDirector.Instance.IniciarRonda();
        }
    }

    private void MostrarPantallaParaJugador(int numeroJugador)
    {
        Debug.Log("[NivelManager] Mostrando panel para Jugador " + numeroJugador);
        Debug.Log("[NivelManager] PowerUpManager.Instance: " + (PowerUpManager.Instance != null ? "Encontrado" : "NULO"));
        
        if (PowerUpManager.Instance != null)
        {
            Debug.Log("[NivelManager] todosLosPoderes: " + (PowerUpManager.Instance.todosLosPoderes != null ? PowerUpManager.Instance.todosLosPoderes.Count.ToString() : "NULO"));
        }

        if (PowerUpManager.Instance == null || PowerUpManager.Instance.todosLosPoderes == null || PowerUpManager.Instance.todosLosPoderes.Count == 0)
        {
            Debug.LogError("[NivelManager] PowerUpManager o poderes no disponibles!");
            return;
        }

        Time.timeScale = 0f;

        // Seleccionar panel según jugador
        GameObject panelActual = (numeroJugador == 1) ? panelSeleccionPoder : panelSeleccionPoderP2;
        TextMeshProUGUI textoNivelActual = (numeroJugador == 1) ? textoNivel : textoNivelP2;
        TextMeshProUGUI textoTituloActual = (numeroJugador == 1) ? textoTituloSeleccion : textoTituloSeleccionP2;
        Image[] iconosActuales = (numeroJugador == 1) ? iconosPoder : iconosPoderP2;
        TextMeshProUGUI[] nombresActuales = (numeroJugador == 1) ? nombresPoder : nombresPoderP2;
        TextMeshProUGUI[] descripcionesActuales = (numeroJugador == 1) ? descripcionesPoder : descripcionesPoderP2;
        Button[] botonesActuales = (numeroJugador == 1) ? botonesPoder : botonesPoderP2;

        if (panelActual != null)
            panelActual.SetActive(true);

        if (textoNivelActual != null)
            textoNivelActual.text = "Nivel " + (GameManager.Instance.rondaActual + 1);

        if (textoTituloActual != null)
            textoTituloActual.text = "Elige un poder, Jugador " + numeroJugador;

        // Elegir poderes aleatorios para este jugador
        List<PowerUpData> opcionesJugador = ElegirPoderesAleatoriosParaJugador(numeroJugador);
        Debug.Log("[NivelManager] Opciones para J" + numeroJugador + ": " + opcionesJugador.Count);

        if (botonesActuales == null)
        {
            Debug.LogError("[NivelManager] botonesPoder" + (numeroJugador == 2 ? "P2" : "") + " es NULO!");
            return;
        }

        for (int i = 0; i < botonesActuales.Length; i++)
        {
            Debug.Log("[NivelManager] Procesando botón " + i + " de " + botonesActuales.Length);
            
            if (botonesActuales[i] == null) 
            {
                Debug.LogError("[NivelManager] Botón " + i + " es NULO!");
                continue;
            }

            if (i < opcionesJugador.Count)
            {
                PowerUpData poder = opcionesJugador[i];
                if (poder == null)
                {
                    Debug.Log("[NivelManager] Poder " + i + " es NULO, desactivando botón");
                    botonesActuales[i].gameObject.SetActive(false);
                    continue;
                }

                Debug.Log("[NivelManager] J" + numeroJugador + " - Botón " + i + ": " + poder.nombre);
                Debug.Log("[NivelManager] J" + numeroJugador + " - Icono: " + (poder.icono != null ? poder.icono.name : "NULO"));
                Debug.Log("[NivelManager] J" + numeroJugador + " - Descripción: " + poder.descripcion);

                if (iconosActuales != null && i < iconosActuales.Length && iconosActuales[i] != null)
                {
                    Debug.Log("[NivelManager] Asignando icono a botón " + i);
                    iconosActuales[i].sprite = poder.icono;
                    iconosActuales[i].enabled = true;
                    iconosActuales[i].gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("[NivelManager] Icono " + i + " no disponible! iconosActuales=" + (iconosActuales != null ? "OK" : "NULO") + ", Length=" + (iconosActuales != null ? iconosActuales.Length.ToString() : "0"));
                }

                if (nombresActuales != null && i < nombresActuales.Length && nombresActuales[i] != null)
                {
                    Debug.Log("[NivelManager] Asignando nombre a botón " + i);
                    nombresActuales[i].text = poder.nombre;
                    nombresActuales[i].enabled = true;
                    nombresActuales[i].gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("[NivelManager] Nombre " + i + " no disponible!");
                }

                if (descripcionesActuales != null && i < descripcionesActuales.Length && descripcionesActuales[i] != null)
                {
                    Debug.Log("[NivelManager] Asignando descripción a botón " + i);
                    descripcionesActuales[i].text = poder.descripcion;
                    descripcionesActuales[i].enabled = true;
                    descripcionesActuales[i].gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("[NivelManager] Descripción " + i + " no disponible!");
                }

                Button boton = botonesActuales[i];
                Debug.Log("[NivelManager] Activando botón " + i);
                boton.gameObject.SetActive(true);
                boton.onClick.RemoveAllListeners();
                int opcionIndex = i;
                int jugadorRef = numeroJugador;
                boton.onClick.AddListener(() => SeleccionarPoderParaJugador(jugadorRef, opcionIndex));
            }
            else
            {
                Debug.Log("[NivelManager] Desactivando botón " + i + " (sin poder asignado)");
                botonesActuales[i].gameObject.SetActive(false);
            }
        }
    }

    private List<PowerUpData> ElegirPoderesAleatoriosParaJugador(int numeroJugador)
    {
        List<PowerUpData> poderDisponible = new List<PowerUpData>();
        
        // Buscar el avión del jugador específico
        ControlAvion[] todosLosAviones = FindObjectsOfType<ControlAvion>();
        ControlAvion jugadorAvion = null;
        
        foreach (ControlAvion avion in todosLosAviones)
        {
            if (avion.numeroJugador == numeroJugador)
            {
                jugadorAvion = avion;
                break;
            }
        }

        foreach (PowerUpData p in PowerUpManager.Instance.todosLosPoderes)
        {
            if (p == null) continue;

            if (p.tipo == PowerUpType.DobleDisparo && jugadorAvion != null && jugadorAvion.tieneDobleDisparo)
                continue;
            
            poderDisponible.Add(p);
        }

        List<PowerUpData> elegidos = new List<PowerUpData>();

        for (int i = 0; i < opcionesPorNivel && poderDisponible.Count > 0; i++)
        {
            int indice = Random.Range(0, poderDisponible.Count);
            elegidos.Add(poderDisponible[indice]);
            poderDisponible.RemoveAt(indice);
        }

        return elegidos;
    }

    public void SeleccionarPoderParaJugador(int numeroJugador, int indice)
    {
        List<PowerUpData> opcionesJugador = ElegirPoderesAleatoriosParaJugador(numeroJugador);
        
        if (indice < 0 || indice >= opcionesJugador.Count)
            return;

        PowerUpData poderSeleccionado = opcionesJugador[indice];

        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.ApplyPowerUp(poderSeleccionado, numeroJugador);
        }

        // Ocultar el panel de este jugador
        GameObject panelActual = (numeroJugador == 1) ? panelSeleccionPoder : panelSeleccionPoderP2;
        if (panelActual != null)
            panelActual.SetActive(false);

        bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;

        // Si es multijugador y ambos han elegido, continuar
        // Si es un jugador, continuar inmediatamente
        if (!esMulti || numeroJugador == 2)
        {
            Time.timeScale = 1f;
            StartCoroutine(TransicionANuevaRonda());
        }
    }

    public void SeleccionarPoder(int indice)
    {
        SeleccionarPoderParaJugador(jugadorElegiendoPoder, indice);
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

    public void MostrarAvisoSubidaNivel(int numeroJugador, int nuevoNivel)
    {
        if (panelAvisoNivel == null || textoAvisoNivel == null) return;

        StopAllCoroutines();
        StartCoroutine(RutinaAvisoNivel(numeroJugador, nuevoNivel));
    }

    IEnumerator RutinaAvisoNivel(int numeroJugador, int nuevoNivel)
    {
        textoAvisoNivel.text = "¡JUGADOR " + numeroJugador + " SUBIÓ A NIVEL " + nuevoNivel + "!";
        panelAvisoNivel.SetActive(true);

        yield return new WaitForSecondsRealtime(duracionAvisoNivel);

        panelAvisoNivel.SetActive(false);
    }

    public void BotonReintentar()
    {
        Time.timeScale = 1f;
        
        // Destruir el GameManager antiguo para evitar conflictos al recargar
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetearEstadisticas();
            Destroy(GameManager.Instance.gameObject);
        }
        
        // Recargar la escena actual
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void BotonMenuPrincipal()
    {
        Time.timeScale = 1f;
        
        // Destruir el GameManager antiguo para evitar conflictos
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetearEstadisticas();
            Destroy(GameManager.Instance.gameObject);
        }
        
        // Cargar escena de menú principal
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipal"); 
    }
}
