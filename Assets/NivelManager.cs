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

    [Header("Configuración")]
    public int opcionesPorNivel = 3;

    private List<PowerUpData> opcionesActuales = new List<PowerUpData>();

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
    }

    public void MostrarPantallaSeleccion()
    {
        if (panelSeleccionPoder == null || PowerUpManager.Instance == null || PowerUpManager.Instance.todosLosPoderes == null)
            return;

        Time.timeScale = 0f;
        panelSeleccionPoder.SetActive(true);

        if (textoNivel != null)
            textoNivel.text = "Nivel " + (GameManager.Instance.rondaActual + 1);

        if (textoTituloSeleccion != null)
            textoTituloSeleccion.text = "Elige un poder";

        opcionesActuales = ElegirPoderesAleatorios();

        for (int i = 0; i < botonesPoder.Length; i++)
        {
            if (i < opcionesActuales.Count)
            {
                PowerUpData poder = opcionesActuales[i];
                if (iconosPoder != null && i < iconosPoder.Length && iconosPoder[i] != null)
                    iconosPoder[i].sprite = poder.icono;

                if (nombresPoder != null && i < nombresPoder.Length && nombresPoder[i] != null)
                    nombresPoder[i].text = poder.nombre;

                if (descripcionesPoder != null && i < descripcionesPoder.Length && descripcionesPoder[i] != null)
                    descripcionesPoder[i].text = poder.descripcion;

                Button boton = botonesPoder[i];
                if (boton != null)
                {
                    boton.gameObject.SetActive(true);
                    boton.onClick.RemoveAllListeners();
                    int opcionIndex = i;
                    boton.onClick.AddListener(() => SeleccionarPoder(opcionIndex));
                }
            }
            else
            {
                if (botonesPoder[i] != null)
                    botonesPoder[i].gameObject.SetActive(false);
            }
        }
    }

    private List<PowerUpData> ElegirPoderesAleatorios()
    {
        List<PowerUpData> poderDisponible = new List<PowerUpData>(PowerUpManager.Instance.todosLosPoderes);
        List<PowerUpData> elegidos = new List<PowerUpData>();

        for (int i = 0; i < opcionesPorNivel && poderDisponible.Count > 0; i++)
        {
            int indice = Random.Range(0, poderDisponible.Count);
            elegidos.Add(poderDisponible[indice]);
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
            PowerUpManager.Instance.ApplyPowerUp(opcionesActuales[indice]);
        }

        if (panelSeleccionPoder != null)
            panelSeleccionPoder.SetActive(false);

        Time.timeScale = 1f;
        StartCoroutine(TransicionANuevaRonda());
    }

    IEnumerator TransicionANuevaRonda()
    {
        if (pantallaCarga != null)
        {
            pantallaCarga.SetActive(true);
            if (textoCarga != null)
                textoCarga.text = "Cargando siguiente nivel...";
        }

        yield return new WaitForSecondsRealtime(duracionPantallaCarga);

        if (pantallaCarga != null)
            pantallaCarga.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.SiguienteRonda();

        if (CombatDirector.Instance != null)
            CombatDirector.Instance.IniciarRonda();
    }
}
