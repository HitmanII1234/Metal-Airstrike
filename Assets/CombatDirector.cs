using System.Collections;
using UnityEngine;

public class CombatDirector : MonoBehaviour
{
    public static CombatDirector Instance;

    [Header("Spawning y Enemigos")]

    [Header("Spawning")]
    public Transform[] spawnPoints;
    public float radioComprobacionSpawn = 1.5f;
    public LayerMask capaEnemigos;
    
    // Ya no usamos costos, generaremos enemigos infinitamente hasta alcanzar el score.

    [Header("Escalado de Dificultad")]
    public float incrementoVelocidadPorNivel = 0.06f;
    public float incrementoDisparoPorNivel = 0.08f;

    private int enemigosVivos;
    private float velocidadSpawn = 2f;
    private bool rondaCompletada = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        IniciarRonda();
    }

    public void IniciarRonda()
    {
        rondaCompletada = false;
        enemigosVivos = 0;
        StartCoroutine(RutinaSpawn());
    }

    IEnumerator RutinaSpawn()
    {
        float nivelFactor = 1f + incrementoVelocidadPorNivel * (GameManager.Instance.rondaActual - 1);
        float tiempoEspera = Mathf.Max(0.25f, velocidadSpawn / nivelFactor);

        while (!rondaCompletada)
        {
            if (enemigosVivos < 40) // Límite de enemigos en pantalla al mismo tiempo
            {
                string tagEnemigo = DeterminarEnemigoPorNivel(GameManager.Instance.rondaActual);
                
                Transform spawnPunto = SeleccionarPuntoSpawnValido();
                if (spawnPunto != null)
                {
                    GameObject enemigo = ObjectPool.Instance.SpawnFromPool(tagEnemigo, spawnPunto.position, Quaternion.identity);
                    if (enemigo != null)
                    {
                        enemigosVivos++;
                        Salud salud = enemigo.GetComponent<Salud>();
                        if (salud != null)
                        {
                            salud.AumentarVidaMaxima(0.1f * GameManager.Instance.rondaActual);
                        }
                    }
                }
            }
            
            yield return new WaitForSeconds(Random.Range(tiempoEspera - 0.4f, tiempoEspera + 0.4f));
        }
    }

    string DeterminarEnemigoPorNivel(int nivel)
    {
        float rnd = Random.value; // de 0.0 a 1.0

        if (nivel <= 2)
        {
            // Nivel 1 y 2: solo básicos
            return "EnemigoBasico";
        }
        else if (nivel == 3 || nivel == 4)
        {
            // Nivel 3 y 4: pocos intermedios (20%), demás básicos (80%)
            if (rnd < 0.80f) return "EnemigoBasico";
            else return "EnemigoIntermedio";
        }
        else
        {
            // Nivel 5 en adelante: más intermedios, pocos básicos, medios avanzados
            // 20% Básicos, 50% Intermedios, 30% Avanzados
            if (rnd < 0.20f) return "EnemigoBasico";
            else if (rnd < 0.70f) return "EnemigoIntermedio";
            else return "EnemigoAvanzado";
        }
    }

    public void TerminarRondaPorScore()
    {
        if (rondaCompletada) return; // Evitar que se llame múltiples veces
        rondaCompletada = true;

        LimpiarEnemigos();

        if (NivelManager.Instance != null)
        {
            NivelManager.Instance.MostrarPantallaSeleccion();
        }
        else
        {
            GameManager.Instance.SiguienteRonda();
            IniciarRonda();
        }
    }

    void LimpiarEnemigos()
    {
        // Desactiva todos los enemigos para limpiar la pantalla y que el jugador pueda elegir su poder
        Salud[] entidades = FindObjectsOfType<Salud>();
        foreach (Salud s in entidades)
        {
            if (!s.gameObject.CompareTag("Player"))
            {
                s.gameObject.SetActive(false);
            }
        }
        enemigosVivos = 0;
    }

    Transform SeleccionarPuntoSpawnValido()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform temp = spawnPoints[i];
            int randomIndex = Random.Range(i, spawnPoints.Length);
            spawnPoints[i] = spawnPoints[randomIndex];
            spawnPoints[randomIndex] = temp;
        }

        foreach (Transform punto in spawnPoints)
        {
            Collider2D colision = Physics2D.OverlapCircle(punto.position, radioComprobacionSpawn, capaEnemigos);
            if (colision == null)
            {
                return punto;
            }
        }

        return null;
    }

    public void EnemigoDerrotado()
    {
        enemigosVivos--;
    }

    public int GetEnemigosVivos()
    {
        return enemigosVivos;
    }
}
