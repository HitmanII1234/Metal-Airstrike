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
    private int enemigosVivosP1;
    private int enemigosVivosP2;
    private float velocidadSpawn = 2f;
    private bool rondaCompletada = false;

    public void SubirNivelJugador(int numeroJugador)
    {
        bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;
        if (!esMulti) return;

        Debug.Log("[CombatDirector] Jugador " + numeroJugador + " subió de nivel!");
    }

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
        while (!rondaCompletada)
        {
            if (this == null) yield break;

            if (GameManager.Instance == null || ObjectPool.Instance == null)
            {
                yield return new WaitForSeconds(0.2f);
                continue;
            }

            bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;
            int maxEnemigos = esMulti ? 80 : 40;

            if (enemigosVivos < maxEnemigos) 
            {
                Transform spawnPunto = SeleccionarPuntoSpawnValido();
                if (spawnPunto != null)
                {
                    if (esMulti)
                    {
                        int nivelP1 = GameManager.Instance.rondaJugador1;
                        int nivelP2 = GameManager.Instance.rondaJugador2;

                        float nivelFactorP1 = 1f + incrementoVelocidadPorNivel * (nivelP1 - 1);
                        float tiempoEsperaP1 = Mathf.Max(0.25f, velocidadSpawn / nivelFactorP1);

                        Vector3 posP1 = spawnPunto.position;
                        posP1.y = Random.Range(1f, 9f);
                        
                        string tagEnemigoP1 = DeterminarEnemigoPorNivel(nivelP1);
                        GameObject enemigo1 = ObjectPool.Instance.SpawnFromPool(tagEnemigoP1, posP1, Quaternion.identity);
                        ConfigurarSaludEnemigo(enemigo1, nivelP1);
                        if (MultiplayerManager.Instance != null && MultiplayerManager.Instance.layerP1 >= 0)
                            MultiplayerManager.AsignarLayerRecursivo(enemigo1, MultiplayerManager.Instance.layerP1);
                        IAEnemigo ia1 = enemigo1.GetComponent<IAEnemigo>();
                        if (ia1 != null)
                        {
                            ia1.yMinZona = 0.5f;
                            ia1.yMaxZona = 9.5f;
                            ia1.nivelJugadorAsignado = 1;
                        }

                        Vector3 posP2 = spawnPunto.position;
                        posP2.y = Random.Range(-9f, -1f);
                        
                        Collider2D colision = Physics2D.OverlapCircle(posP2, radioComprobacionSpawn, capaEnemigos);
                        if (colision == null)
                        {
                            string tagEnemigoP2 = DeterminarEnemigoPorNivel(nivelP2);
                            GameObject enemigo2 = ObjectPool.Instance.SpawnFromPool(tagEnemigoP2, posP2, Quaternion.identity);
                            ConfigurarSaludEnemigo(enemigo2, nivelP2);
                            if (MultiplayerManager.Instance != null && MultiplayerManager.Instance.layerP2 >= 0)
                                MultiplayerManager.AsignarLayerRecursivo(enemigo2, MultiplayerManager.Instance.layerP2);
                            IAEnemigo ia2 = enemigo2.GetComponent<IAEnemigo>();
                            if (ia2 != null)
                            {
                                ia2.yMinZona = -9.5f;
                                ia2.yMaxZona = -0.5f;
                                ia2.nivelJugadorAsignado = 2;
                            }
                        }

                        yield return new WaitForSeconds(Random.Range(tiempoEsperaP1 - 0.4f, tiempoEsperaP1 + 0.4f));
                    }
                    else
                    {
                        float nivelFactor = 1f + incrementoVelocidadPorNivel * (GameManager.Instance.rondaActual - 1);
                        float tiempoEspera = Mathf.Max(0.25f, velocidadSpawn / nivelFactor);

                        string tagEnemigo = DeterminarEnemigoPorNivel(GameManager.Instance.rondaActual);
                        
                        Vector3 posicionSpawn = spawnPunto.position;
                        posicionSpawn.y = Mathf.Clamp(posicionSpawn.y, -4f, 4f);
                        GameObject enemigo = ObjectPool.Instance.SpawnFromPool(tagEnemigo, posicionSpawn, Quaternion.identity);
                        ConfigurarSaludEnemigo(enemigo, GameManager.Instance.rondaActual);

                        yield return new WaitForSeconds(Random.Range(tiempoEspera - 0.3f, tiempoEspera + 0.3f));
                    }
                }
            }
            
            yield return null;
        }
    }

    void ConfigurarSaludEnemigo(GameObject enemigo, int nivel)
    {
        if (enemigo != null)
        {
            enemigosVivos++;
            Salud salud = enemigo.GetComponent<Salud>();
            if (salud != null)
            {
                salud.AumentarVidaMaxima(0.1f * nivel);
            }
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
        if (rondaCompletada) return;
        rondaCompletada = true;

        LimpiarEnemigos();

        bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;

        if (esMulti)
        {
            GameManager.Instance.SiguienteRonda();
            IniciarRonda();
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
        if (spawnPoints == null) return null;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == null) continue;
            Transform temp = spawnPoints[i];
            int randomIndex = Random.Range(i, spawnPoints.Length);
            spawnPoints[i] = spawnPoints[randomIndex];
            spawnPoints[randomIndex] = temp;
        }

        foreach (Transform punto in spawnPoints)
        {
            if (punto == null) continue; // Evita acceder a un punto destruido durante la transición de escena
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
