using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelPrincipal;
    public GameObject panelNombres;
    public TMP_InputField inputName;

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

            // Iniciar configuración de cámaras si es multijugador en la siguiente escena
            PlayerPrefs.SetInt("Multijugador", esMultijugador ? 1 : 0);
            SceneManager.LoadScene("MainLevel");
        }
    }
}
