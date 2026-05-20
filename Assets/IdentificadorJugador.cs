using UnityEngine;
using TMPro;

public class IdentificadorJugador : MonoBehaviour
{
    [Header("Configuración del Jugador")]
    [Tooltip("1 para Player 1, 2 para Player 2")]
    public int numeroJugador = 1;

    [Header("Referencias de UI")]
    public TextMeshProUGUI textoNombre;

    void Start()
    {
        ActualizarNombre();
    }

    public void ActualizarNombre()
    {
        if (textoNombre == null) return;

        if (GameManager.Instance != null)
        {
            if (numeroJugador == 1)
            {
                textoNombre.text = !string.IsNullOrEmpty(GameManager.Instance.playerName) 
                    ? GameManager.Instance.playerName 
                    : "Jugador 1";
            }
            else if (numeroJugador == 2)
            {
                textoNombre.text = !string.IsNullOrEmpty(GameManager.Instance.player2Name) 
                    ? GameManager.Instance.player2Name 
                    : "Jugador 2";
            }
        }
        else
        {
            textoNombre.text = "Jugador " + numeroJugador;
        }
    }
}
