using UnityEngine;

public class Salud : MonoBehaviour, IPooleable
{
    public float vidaMaxima = 100f;
    private float vidaActual;

    void Start()
    {
        vidaActual = vidaMaxima;
    }

    void OnEnable()
    {
        vidaActual = vidaMaxima;
    }

    public void OnObjectSpawn()
    {
        vidaActual = vidaMaxima;
    }

    public void RecibirDanio(float cantidad)
    {
        // Check for Bullet Immunity (probabilidad)
        if (gameObject.CompareTag("Player") && Random.value < GameManager.Instance.bulletImmunityChance)
        {
            return; // Inmune a este ataque
        }

        vidaActual -= cantidad;
        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    public void Curar(float cantidad)
    {
        vidaActual += cantidad;
        if (vidaActual > vidaMaxima) vidaActual = vidaMaxima;
    }

    public void AumentarVidaMaxima(float porcentaje)
    {
        vidaMaxima *= (1f + porcentaje);
        vidaActual = vidaMaxima;
    }

    void Morir()
    {
        if (gameObject.CompareTag("Enemigo"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(1);
                // Life Steal si el jugador lo tiene
                if (GameManager.Instance.hasLifeSteal)
                {
                    GameObject jugador = GameObject.FindGameObjectWithTag("Player");
                    if (jugador != null) jugador.GetComponent<Salud>().Curar(5f);
                }
            }
        }
        else if (gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerDied();
            }
        }
        gameObject.SetActive(false);
    }
}