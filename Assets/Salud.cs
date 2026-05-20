using UnityEngine;

public class Salud : MonoBehaviour, IPooleable
{
    public float vidaMaxima = 100f;
    private float vidaActual;

    [Header("Escudos y Regeneración")]
    public float escudoActual = 0f;
    public float escudoMaximo = 0f;
    public int numeroJugador = 1;
    public float cantidadRegeneracion = 0f;
    public float cantidadRegeneracionEscudo = 0f;
    public float reduccionDanio = 0f;

    void Start()
    {
        vidaActual = vidaMaxima;
    }

    void OnEnable()
    {
        vidaActual = vidaMaxima;
        escudoActual = escudoMaximo;
    }

    public void OnObjectSpawn()
    {
        vidaActual = vidaMaxima;
        escudoActual = escudoMaximo;
    }

    public float ObtenerPorcentajeVida()
    {
        return vidaActual / vidaMaxima;
    }

    public float ObtenerPorcentajeEscudo()
    {
        if (escudoMaximo <= 0) return 0;
        return escudoActual / escudoMaximo;
    }

    public void RecibirDanio(float cantidad)
    {
        // Aplica reducción de daño (ej: 0.1f = 10% menos de daño)
        cantidad *= (1f - reduccionDanio);

        // El escudo absorbe daño primero
        if (escudoActual > 0)
        {
            if (cantidad >= escudoActual)
            {
                cantidad -= escudoActual;
                escudoActual = 0;
            }
            else
            {
                escudoActual -= cantidad;
                cantidad = 0;
            }
        }

        // Si sobra daño, se aplica a la vida
        if (cantidad > 0)
        {
            vidaActual -= cantidad;
            if (vidaActual <= 0)
            {
                Morir();
            }
        }
    }

    public void Curar(float cantidad)
    {
        vidaActual += cantidad;
        if (vidaActual > vidaMaxima) vidaActual = vidaMaxima;
    }

    public void RecuperarTotalVida()
    {
        vidaActual = vidaMaxima;
    }

    public void AumentarVidaMaxima(float porcentaje)
    {
        float incremento = vidaMaxima * porcentaje;
        vidaMaxima += incremento;
        vidaActual += incremento; // Cura la cantidad incrementada también
    }

    public void MejorarRegeneracion(float cantidad)
    {
        cantidadRegeneracion += cantidad;
    }

    public void ActivarMejoraEscudo(float cantidad)
    {
        escudoMaximo += cantidad;
        escudoActual += cantidad; // Agrega el escudo instantáneamente
    }

    void Morir()
    {
        string miTag = gameObject.tag.Trim(); // Limpiar espacios invisibles
        
        if (miTag.Contains("Enemigo"))
        {
            if (GameManager.Instance != null)
            {
                int puntos = 100; // Por defecto
                if (miTag.Contains("Intermedio")) puntos = 150;
                else if (miTag.Contains("Avanzado")) puntos = 200;

                GameManager.Instance.AddScore(puntos);
                
                if (CombatDirector.Instance != null) 
                {
                    CombatDirector.Instance.EnemigoDerrotado();
                }
            }
        }
        else if (miTag == "Player" || gameObject.name.Contains("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerDied();
            }
        }
        gameObject.SetActive(false);
    }
}