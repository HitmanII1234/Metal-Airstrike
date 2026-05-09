using UnityEngine;

public class Salud : MonoBehaviour
{
    public float vidaMaxima = 100f;
    private float vidaActual;

    void Start()
    {
        vidaActual = vidaMaxima;
    }

    public void RecibirDanio(float cantidad)
    {
        vidaActual -= cantidad;
        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        // Aquí podrías instanciar una explosión
        Destroy(gameObject);
    }
}