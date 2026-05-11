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

    public float ObtenerPorcentajeVida()
    {
        return vidaActual / vidaMaxima;
    }

    public void RecibirDanio(float cantidad)
    {
        if (gameObject.CompareTag("Player") && Random.value < GameManager.Instance.bulletImmunityChance)
        {
            return; 
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
                if (GameManager.Instance.hasLifeSteal)
                {
                    GameObject[] jugadores = GameObject.FindGameObjectsWithTag("Player");
                    foreach(GameObject j in jugadores) j.GetComponent<Salud>().Curar(5f);
                }
                
                CombatDirector director = FindObjectOfType<CombatDirector>();
                if (director != null) director.EnemigoDerrotado();
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