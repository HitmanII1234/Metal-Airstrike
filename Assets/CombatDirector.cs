using System.Collections;
using UnityEngine;

public class CombatDirector : MonoBehaviour
{
    [Header("Configuración de Rondas")]
    public float creditosBase = 100f;
    public float multiplicadorCreditos = 1.2f;

    [Header("Spawning")]
    public Transform[] spawnPoints;
    public float radioComprobacionSpawn = 1.5f;
    public LayerMask capaEnemigos;
    
    private float creditosActuales;
    private int enemigosVivos;

    void Start()
    {
        IniciarRonda();
    }

    void IniciarRonda()
    {
        // Calcular presupuesto para esta ronda
        creditosActuales = creditosBase * Mathf.Pow(multiplicadorCreditos, GameManager.Instance.rondaActual - 1);
        enemigosVivos = 0;
        StartCoroutine(RutinaSpawn());
    }

    IEnumerator RutinaSpawn()
    {
        while (creditosActuales > 0)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            int costoEnemigo = 10; // Ejemplo: el enemigo básico cuesta 10 créditos
            if (creditosActuales >= costoEnemigo)
            {
                Transform spawnPunto = SeleccionarPuntoSpawnValido();
                if (spawnPunto != null)
                {
                    creditosActuales -= costoEnemigo;
                    
                    // Elegir enemigo
                    string tagEnemigo = "EnemigoBasico";
                    
                    if (GameManager.Instance.rondaActual % 5 == 0 && creditosActuales < costoEnemigo)
                    {
                        // Jefes (Estilo Cuphead) cada 5 niveles - FSM (Asumiendo que creamos un BossPrefab)
                        tagEnemigo = "Jefe";
                    }

                    GameObject enemigo = ObjectPool.Instance.SpawnFromPool(tagEnemigo, spawnPunto.position, Quaternion.identity);
                    if (enemigo != null)
                    {
                        enemigosVivos++;
                        // Incrementar vida segun ronda
                        Salud salud = enemigo.GetComponent<Salud>();
                        if (salud != null)
                        {
                            salud.AumentarVidaMaxima(0.1f * GameManager.Instance.rondaActual);
                        }
                    }
                }
            }
        }
    }

    Transform SeleccionarPuntoSpawnValido()
    {
        // Mezclar puntos de spawn para aleatorizar
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform temp = spawnPoints[i];
            int randomIndex = Random.Range(i, spawnPoints.Length);
            spawnPoints[i] = spawnPoints[randomIndex];
            spawnPoints[randomIndex] = temp;
        }

        foreach (Transform punto in spawnPoints)
        {
            // Physics2D.OverlapCircle para verificar que el área está despejada
            Collider2D colision = Physics2D.OverlapCircle(punto.position, radioComprobacionSpawn, capaEnemigos);
            if (colision == null)
            {
                return punto; // Área despejada
            }
        }

        return null; // Todos los puntos están ocupados
    }

    public void EnemigoDerrotado()
    {
        enemigosVivos--;
        if (enemigosVivos <= 0 && creditosActuales <= 0)
        {
            // Fin de ronda
            GameManager.Instance.SiguienteRonda();
            IniciarRonda(); // o ir a pantalla de poderes
        }
    }
}
