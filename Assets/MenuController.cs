using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("UI Elements Principal")]
    public GameObject panelPrincipal;
    public GameObject panelNombres;
    public TMP_InputField inputName;
    public TMP_InputField inputName2;

    [Header("UI Opciones")]
    public GameObject panelOpciones;
    public Slider sliderMaster;
    public Slider sliderMusic;
    public Slider sliderSFX;
    public AudioSource musicSource;
    public AudioSource[] sfxSources;
    public Toggle fullscreenToggle;
    public Dropdown qualityDropdown;
    public GameObject[] optionSubPanels;

    [Header("UI Game Over")]
    public GameObject panelGameOver;
    public TextMeshProUGUI textoScoreFinal;

    [Header("UI Scores")]
    public GameObject panelScores;
    public Transform contenedorListaScores;
    public GameObject prefabEntradaScore;
    public TextMeshProUGUI textoListaVacia;

    private bool esMultijugador = false;
    private float masterVolume = 1f;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    private void Start()
    {
        LoadAudioSettings();
        if (inputName2 != null)
            inputName2.gameObject.SetActive(false);

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMenuMusic();

        if (sliderMaster != null)
            sliderMaster.onValueChanged.AddListener(SetMasterVolume);

        if (sliderMusic != null)
            sliderMusic.onValueChanged.AddListener(SetMusicVolume);

        if (sliderSFX != null)
            sliderSFX.onValueChanged.AddListener(SetSFXVolume);

        if (panelScores != null)
            panelScores.SetActive(false);
    }

    private void OnDestroy()
    {
        // Limpiar los listeners para evitar memory leaks
        if (sliderMaster != null)
            sliderMaster.onValueChanged.RemoveListener(SetMasterVolume);

        if (sliderMusic != null)
            sliderMusic.onValueChanged.RemoveListener(SetMusicVolume);

        if (sliderSFX != null)
            sliderSFX.onValueChanged.RemoveListener(SetSFXVolume);
    }

    public void AbrirOpciones()
    {
        if (panelOpciones != null)
            panelOpciones.SetActive(true);
    }

    public void CerrarOpciones()
    {
        if (panelOpciones != null)
            panelOpciones.SetActive(false);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        AudioListener.volume = masterVolume;
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        UpdateMusicVolume();
        UpdateSFXVolume();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        UpdateMusicVolume();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        UpdateSFXVolume();
    }

    public void SetFullscreen(bool fullScreen)
    {
        Screen.fullScreen = fullScreen;
        PlayerPrefs.SetInt("Fullscreen", fullScreen ? 1 : 0);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }

    public void ResetOptionsToDefault()
    {
        SetMasterVolume(1f);
        SetMusicVolume(1f);
        SetSFXVolume(1f);

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = true;

        if (qualityDropdown != null)
            SetQuality(Mathf.Max(qualityDropdown.options.Count - 1, 0));
    }

    public void OpenOptionSubPanel(int index)
    {
        if (optionSubPanels == null)
            return;

        for (int i = 0; i < optionSubPanels.Length; i++)
        {
            if (optionSubPanels[i] != null)
                optionSubPanels[i].SetActive(i == index);
        }
    }

    private void LoadAudioSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        AudioListener.volume = masterVolume;
        UpdateMusicVolume();
        UpdateSFXVolume();

        if (sliderMaster != null)
            sliderMaster.value = masterVolume;

        if (sliderMusic != null)
            sliderMusic.value = musicVolume;

        if (sliderSFX != null)
            sliderSFX.value = sfxVolume;

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;

        if (qualityDropdown != null)
        {
            int maxQualityIndex = Mathf.Max(qualityDropdown.options.Count - 1, 0);
            int savedQuality = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
            savedQuality = Mathf.Clamp(savedQuality, 0, maxQualityIndex);
            qualityDropdown.value = savedQuality;
            qualityDropdown.RefreshShownValue();
            QualitySettings.SetQualityLevel(savedQuality);
        }
    }

    private void UpdateMusicVolume()
    {
        float vol = masterVolume * musicVolume;
        if (MusicManager.Instance != null)
            MusicManager.Instance.SetMusicVolume(vol);
        else if (musicSource != null)
            musicSource.volume = vol;
    }

    private void UpdateSFXVolume()
    {
        if (sfxSources != null)
        {
            foreach (AudioSource source in sfxSources)
            {
                if (source != null)
                    source.volume = masterVolume * sfxVolume;
            }
        }
    }

    public void BotonModoHistoria()
    {
        esMultijugador = false;
        panelPrincipal.SetActive(false);
        panelNombres.SetActive(true);
        if (inputName2 != null)
            inputName2.gameObject.SetActive(false);
    }

    public void BotonMultijugador()
    {
        esMultijugador = true;
        panelPrincipal.SetActive(false);
        panelNombres.SetActive(true);
        if (inputName2 != null)
            inputName2.gameObject.SetActive(true);
    }

    public void VolverAlMenu()
    {
        if (panelNombres != null)
            panelNombres.SetActive(false);

        if (panelPrincipal != null)
            panelPrincipal.SetActive(true);
    }

    public void EmpezarJuego()
    {
        string nombre1 = inputName != null ? inputName.text.Trim() : string.Empty;
        string nombre2 = inputName2 != null ? inputName2.text.Trim() : string.Empty;

        if (esMultijugador)
        {
            if (nombre1 == "" || nombre2 == "")
                return;

            PlayerPrefs.SetString("Player1Name", nombre1);
            PlayerPrefs.SetString("Player2Name", nombre2);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.playerName = nombre1;
                GameManager.Instance.player2Name = nombre2;
            }
        }
        else
        {
            if (nombre1 == "")
                return;

            PlayerPrefs.SetString("Player1Name", nombre1);
            PlayerPrefs.SetString("Player2Name", "");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.playerName = nombre1;
                GameManager.Instance.player2Name = string.Empty;
            }
        }

        PlayerPrefs.SetInt("Multijugador", esMultijugador ? 1 : 0);
        if (GameManager.Instance != null) GameManager.Instance.ResetearEstadisticas();
        if (MusicManager.Instance != null) MusicManager.Instance.PlayGameplayMusic();
        SceneManager.LoadScene("MainLevel"); // Asegúrate que tu escena de juego se llame así
    }

    public void MostrarGameOver(int score)
    {
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
            if (textoScoreFinal != null) textoScoreFinal.text = "SCORE FINAL: " + score;
        }
    }

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null) GameManager.Instance.ResetearEstadisticas();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null) GameManager.Instance.ResetearEstadisticas();
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void AbrirPanelScores()
    {
        if (panelScores != null)
        {
            panelScores.SetActive(true);
            PoblarListaScores();
        }
    }

    public void CerrarPanelScores()
    {
        if (panelScores != null)
            panelScores.SetActive(false);
    }

    void PoblarListaScores()
    {
        if (contenedorListaScores == null) return;

        // Mantener layout vertical para la lista de entradas
        var hLayout = contenedorListaScores.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
        if (hLayout != null) UnityEngine.Object.DestroyImmediate(hLayout);
        
        var vLayout = contenedorListaScores.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        if (vLayout == null)
        {
            vLayout = contenedorListaScores.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
        }
        vLayout.padding = new RectOffset(10, 10, 10, 10);
        vLayout.spacing = 5;
        vLayout.childControlWidth = true;
        vLayout.childForceExpandWidth = true;
        vLayout.childControlHeight = false;
        vLayout.childForceExpandHeight = false;

        // Configurar ScrollRect para scroll vertical
        var scrollRect = contenedorListaScores.GetComponentInParent<UnityEngine.UI.ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
        }

        foreach (Transform child in contenedorListaScores)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }

        if (ScoreManager.Instance == null) return;

        List<ScoreEntry> scores = ScoreManager.Instance.ObtenerTodosLosScores();

        if (scores.Count == 0)
        {
            if (textoListaVacia != null)
                textoListaVacia.gameObject.SetActive(true);
            return;
        }

        if (textoListaVacia != null)
            textoListaVacia.gameObject.SetActive(false);

        for (int i = 0; i < scores.Count; i++)
        {
            ScoreEntry entry = scores[i];
            GameObject entradaObj;

            if (prefabEntradaScore != null)
            {
                entradaObj = Instantiate(prefabEntradaScore, contenedorListaScores);
            }
            else
            {
                entradaObj = new GameObject("EntradaScore");
                entradaObj.transform.SetParent(contenedorListaScores, false);

                RectTransform rect = entradaObj.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                rect.sizeDelta = new Vector2(650, 60);

                var entryLayout = entradaObj.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                entryLayout.padding = new RectOffset(10, 10, 5, 5);
                entryLayout.spacing = 15;
                entryLayout.childControlWidth = false;
                entryLayout.childForceExpandWidth = false;
                entryLayout.childControlHeight = true;
                entryLayout.childForceExpandHeight = false;
                entryLayout.childAlignment = TextAnchor.MiddleLeft;

                // Posición
                CreateTextChild(entradaObj, (i + 1) + ".", 80, 24, Color.yellow);
                // Nombre
                string nombreDisplay = entry.esMultijugador 
                    ? entry.nombre + " & " + entry.nombre2 
                    : entry.nombre;
                CreateTextChild(entradaObj, nombreDisplay, 150, 24, Color.white);
                // Nivel
                CreateTextChild(entradaObj, "Nivel " + entry.nivel, 80, 24, Color.cyan);
                // Score
                CreateTextChild(entradaObj, "Score: " + entry.score, 100, 24, Color.green);
                // Fecha
                CreateTextChild(entradaObj, entry.fecha, 80, 20, Color.gray);
            }
        }
    }

    void CreateTextChild(GameObject parent, string text, int width, int fontSize, Color color)
    {
        GameObject child = new GameObject("Text_" + text);
        child.transform.SetParent(parent.transform, false);

        RectTransform rect = child.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, 40);

        TextMeshProUGUI txt = child.AddComponent<TextMeshProUGUI>();
        txt.text = text;
        txt.alignment = TextAlignmentOptions.Left;
        txt.fontSize = fontSize;
        txt.color = color;
    }
}
