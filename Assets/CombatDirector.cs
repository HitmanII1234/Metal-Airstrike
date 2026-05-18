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
    
    [Header("Costos de Enemigos")]
    public int costoEnemigoBasico = 10;
    public int costoEnemigoIntermedio = 20;
    public int costoEnemigoAvanzado = 40;

    private float creditosActuales;
    private int enemigosVivos;
    private float velocidadSpawn = 2f;

    void Start()
    {
        IniciarRonda();
    }

    void IniciarRonda()
    {
        creditosActuales = creditosBase * Mathf.Pow(multiplicadorCreditos, GameManager.Instance.rondaActual - 1);
        enemigosVivos = 0;
        StartCoroutine(RutinaSpawn());
    }

    IEnumerator RutinaSpawn()
    {
        float tiempoEspera = velocidadSpawn > 0 ? velocidadSpawn : 2f;

        while (creditosActuales > 0 && enemigosVivos < 50)
        {
            yield return new WaitForSeconds(Random.Range(tiempoEspera - 0.5f, tiempoEspera + 0.5f));

            string tagEnemigo = "EnemigoBasico";
            int costoEnemigo = costoEnemigoBasico;

            if (creditosActuales >= costoEnemigoAvanzado)
            {
                if (Random.value > 0.7f)
                {
                    tagEnemigo = "EnemigoAvanzado";
                    costoEnemigo = costoEnemigoAvanzado;
                }
                else
                {
                    tagEnemigo = "EnemigoIntermedio";
                    costoEnemigo = costoEnemigoIntermedio;
                }
            }
            else if (creditosActuales >= costoEnemigoIntermedio)
            {
                if (Random.value > 0.5f)
                {
                    tagEnemigo = "EnemigoIntermedio";
                    costoEnemigo = costoEnemigoIntermedio;
                }
            }

            if (creditosActuales >= costoEnemigo)
            {
                Transform spawnPunto = SeleccionarPuntoSpawnValido();
                if (spawnPunto != null)
                {
                    creditosActuales -= costoEnemigo;

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
        }

        while (enemigosVivos > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        GameManager.Instance.SiguienteRonda();
        IniciarRonda();
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
