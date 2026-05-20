using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PowerUpType
{
    MejoraVelocidadDisparo, // Cadencia
    MejoraVida,             // +10 HP max
    MejoraRecuperacionVida, // +2 hp / 5s
    MejoraEscudo,           // +25 Escudo Maximo
    DobleDisparo,           // Dispara 2 balas
    RecuperacionTotalVida,  // Cura vida al 100%
    RecargaEscudoPorTiempo, // +5 escudo / 15s
    AumentoResistencia      // Reduce daño recibido
}

[CreateAssetMenu(fileName = "NewPowerUp", menuName = "PowerUp")]
public class PowerUpData : ScriptableObject
{
    public PowerUpType tipo;
    public string nombre;
    public string descripcion;
    public Sprite icono;
}

public class PowerUpManager : MonoBehaviour
{
    private static PowerUpManager _instance;
    public static PowerUpManager Instance
    {
        get
        {
            if (_instance == null || _instance.todosLosPoderes == null || _instance.todosLosPoderes.Count == 0)
            {
                PowerUpManager[] managers = FindObjectsOfType<PowerUpManager>();
                foreach (PowerUpManager m in managers)
                {
                    if (m.todosLosPoderes != null && m.todosLosPoderes.Count > 0)
                    {
                        _instance = m;
                        return _instance;
                    }
                }
                if (managers.Length > 0)
                    _instance = managers[0];
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    public List<PowerUpData> todosLosPoderes;
    private Coroutine autoRegenCoroutine;

    void Awake()
    {
        // Forzamos que se asigne esta instancia si tiene los poderes
        if (_instance == null || (todosLosPoderes != null && todosLosPoderes.Count > 0))
        {
            _instance = this;
        }
    }

    public void ApplyPowerUp(PowerUpData data, int targetPlayerNumber = 1)
    {
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject j in jugadores)
        {
            if (j == null) continue;

            ControlAvion avion = j.GetComponent<ControlAvion>();
            Salud salud = j.GetComponent<Salud>();

            // Determinar a qué jugador pertenece este objeto
            // Si tiene avion, ese es el identificador principal
            // Solo saltar si podemos confirmar que NO es el jugador objetivo
            if (avion != null && avion.numeroJugador != targetPlayerNumber) continue;

            switch (data.tipo)
            {
                case PowerUpType.MejoraVelocidadDisparo:
                    if (avion != null)
                    {
                        // Aumenta un 10% la cadencia actual, se stackea haciéndolo más rápido cada vez
                        avion.cadenciaDisparo *= 0.90f; 
                        avion.cadenciaDisparo = Mathf.Max(0.05f, avion.cadenciaDisparo);
                    }
                    break;

                case PowerUpType.MejoraVida:
                    if (salud != null)
                    {
                        salud.vidaMaxima += 10f; // +10 Vida Maxima (Stackeable)
                        salud.Curar(10f);
                    }
                    break;

                case PowerUpType.MejoraRecuperacionVida:
                    if (salud != null)
                    {
                        salud.cantidadRegeneracion += 2f; // Stackeable
                    }
                    if (autoRegenCoroutine == null)
                    {
                        autoRegenCoroutine = StartCoroutine(AutoRegenRoutine());
                    }
                    break;

                case PowerUpType.MejoraEscudo:
                    if (salud != null)
                    {
                        salud.ActivarMejoraEscudo(25f); // Stackeable
                    }
                    break;

                case PowerUpType.DobleDisparo:
                    if (avion != null)
                    {
                        avion.tieneDobleDisparo = true;
                    }
                    break;

                case PowerUpType.RecuperacionTotalVida:
                    if (salud != null)
                    {
                        salud.RecuperarTotalVida();
                    }
                    break;

                case PowerUpType.RecargaEscudoPorTiempo:
                    if (salud != null)
                    {
                        salud.cantidadRegeneracionEscudo += 5f; // Stackeable
                    }
                    if (autoRegenCoroutine == null)
                    {
                        autoRegenCoroutine = StartCoroutine(AutoRegenRoutine());
                    }
                    break;

                case PowerUpType.AumentoResistencia:
                    if (salud != null)
                    {
                        // Añade 5% de resistencia por cada vez (se stackea) max 80%
                        salud.reduccionDanio += 0.05f;
                        if (salud.reduccionDanio > 0.8f) salud.reduccionDanio = 0.8f;
                    }
                    break;
            }
        }
    }

    IEnumerator AutoRegenRoutine()
    {
        int timer = 0;
        while (true)
        {
            yield return new WaitForSeconds(1f); // Contamos por segundo
            timer++;

            GameObject[] jugadores = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject j in jugadores)
            {
                if (j != null && j.activeInHierarchy)
                {
                    Salud salud = j.GetComponent<Salud>();
                    if (salud != null)
                    {
                        // Cada 5 segundos cura vida
                        if (timer % 5 == 0 && salud.cantidadRegeneracion > 0)
                        {
                            salud.Curar(salud.cantidadRegeneracion);
                        }

                        // Cada 15 segundos regenera escudo
                        if (timer % 15 == 0 && salud.cantidadRegeneracionEscudo > 0)
                        {
                            salud.ActivarMejoraEscudo(salud.cantidadRegeneracionEscudo);
                        }
                    }
                }
            }
        }
    }

    public void ResetPoderes()
    {
        if (autoRegenCoroutine != null)
        {
            StopCoroutine(autoRegenCoroutine);
            autoRegenCoroutine = null;
        }
    }
}
