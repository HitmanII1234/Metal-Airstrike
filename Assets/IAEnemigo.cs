using UnityEngine;

public enum TipoMovimientoEnemigo { Basico, ZigZag, Bezier }

public class IAEnemigo : MonoBehaviour, IPooleable
{
    [Header("Tipo de IA")]
    public TipoMovimientoEnemigo tipoMovimiento;

    [Header("Movimiento")]
    public float velocidad = 3f;
    public float amplitudZigZag = 2f;
    public float frecuenciaZigZag = 2f;

    [Header("Disparo Automático")]
    public string tagBala = "BalaEnemiga";
    public Transform puntoDisparo;
    public float tiempoEntreDisparos = 2f; 
    private float cronometro;

    private float yInicial;
    private float tiempoAparicion;

    // IA Reactiva
    private Transform jugador;
    public float distanciaReaccion = 5f;

    public void OnObjectSpawn()
    {
        yInicial = transform.position.y;
        tiempoAparicion = Time.time;
        cronometro = Random.Range(0f, tiempoEntreDisparos);
        
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) jugador = pObj.transform;
    }

    void Update()
    {
        float esquivaY = 0f;
        if (jugador != null && Vector2.Distance(transform.position, jugador.position) < distanciaReaccion)
        {
            esquivaY = Mathf.Sign(transform.position.y - jugador.position.y) * velocidad * 0.5f * Time.deltaTime;
        }

        float nivelFactor = 1f;
        if (GameManager.Instance != null)
        {
            nivelFactor += 0.05f * (GameManager.Instance.rondaActual - 1);
        }

        if (tipoMovimiento == TipoMovimientoEnemigo.Basico)
        {
            transform.Translate(Vector2.left * velocidad * nivelFactor * Time.deltaTime + new Vector2(0, esquivaY));
        }
        else if (tipoMovimiento == TipoMovimientoEnemigo.ZigZag)
        {
            float newX = transform.position.x - (velocidad * nivelFactor * Time.deltaTime);
            float newY = yInicial + Mathf.Sin((Time.time - tiempoAparicion) * frecuenciaZigZag) * amplitudZigZag;
            transform.position = new Vector3(newX, newY + esquivaY, transform.position.z);
        }
        else if (tipoMovimiento == TipoMovimientoEnemigo.Bezier)
        {
            float t = (Time.time - tiempoAparicion) * velocidad * nivelFactor * 0.5f;
            float x = transform.position.x - (velocidad * nivelFactor * Time.deltaTime);
            float y = yInicial + Mathf.Sin(t) * 2f + Mathf.Cos(t * 2f) * 1f;
            transform.position = new Vector3(x, y + esquivaY, transform.position.z);
        }

        cronometro += Time.deltaTime;

        float delayActual = tiempoEntreDisparos;
        if (GameManager.Instance != null)
        {
            delayActual /= 1f + 0.1f * (GameManager.Instance.rondaActual - 1);
        }

        if (jugador != null && Vector2.Distance(transform.position, jugador.position) < distanciaReaccion)
        {
            delayActual /= 2f; // Ráfagas
        }

        if (cronometro >= delayActual)
        {
            Disparar();
            cronometro = 0; 
        }

        if (transform.position.x < -15f) gameObject.SetActive(false);
    }

    void Disparar()
    {
        if (ObjectPool.Instance != null && puntoDisparo != null)
        {
            GameObject balaObj = ObjectPool.Instance.SpawnFromPool(tagBala, puntoDisparo.position, Quaternion.identity);
            if(balaObj != null)
            {
                Bala scriptBala = balaObj.GetComponent<Bala>();
                if (scriptBala != null)
                {
                    scriptBala.balaEnemiga = true; 
                    
                    // Escalar el daño del enemigo según la ronda actual (ej: +20% por ronda)
                    float dañoBase = 15f;
                    if (GameManager.Instance != null)
                    {
                        scriptBala.danio = dañoBase * (1f + (0.2f * (GameManager.Instance.rondaActual - 1)));
                    }
                    else
                    {
                        scriptBala.danio = dañoBase;
                    }
                }
            }
        }
    }
}