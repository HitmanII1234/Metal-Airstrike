using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    public Salud saludDestino;
    public Image barraLlenado;

    void Update()
    {
        if (saludDestino != null && barraLlenado != null)
        {
            barraLlenado.fillAmount = saludDestino.ObtenerPorcentajeVida();
        }
    }
}
