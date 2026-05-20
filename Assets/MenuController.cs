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

        // Conectar los listeners de los sliders
        if (sliderMaster != null)
            sliderMaster.onValueChanged.AddListener(SetMasterVolume);

        if (sliderMusic != null)
            sliderMusic.onValueChanged.AddListener(SetMusicVolume);

        if (sliderSFX != null)
            sliderSFX.onValueChanged.AddListener(SetSFXVolume);
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
        SceneManager.LoadScene("MenuPrincipal"); // Asegúrate de que esta escena se llame así
    }
}
