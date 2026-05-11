using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("UI Elements Principal")]
    public GameObject panelPrincipal;
    public GameObject panelNombres;
    public TMP_InputField inputName;

    [Header("UI Game Over")]
    public GameObject panelGameOver;
    public TextMeshProUGUI textoScoreFinal;

    private bool esMultijugador = false;

    public void BotonModoHistoria()
    {
        esMultijugador = false;
        panelPrincipal.SetActive(false);
        panelNombres.SetActive(true);
    }

    public void BotonMultijugador()
    {
        esMultijugador = true;
        panelPrincipal.SetActive(false);
        panelNombres.SetActive(true);
    }

    public void EmpezarJuego()
    {
        if (inputName.text.Trim() != "")
        {
            if (GameManager.Instance != null)
                GameManager.Instance.playerName = inputName.text;

            PlayerPrefs.SetInt("Multijugador", esMultijugador ? 1 : 0);
            if (GameManager.Instance != null) GameManager.Instance.ResetearEstadisticas();
            SceneManager.LoadScene("MainLevel"); // Asegúrate que tu escena de juego se llame así
        }
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
