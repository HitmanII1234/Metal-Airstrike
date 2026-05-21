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

    public int nivelJugadorAsignado = 0;
    private int yMinZonaInterno = -10;
    private int yMaxZonaInterno = 10;

    public float yMinZona
    {
        get { return yMinZonaInterno; }
        set { yMinZonaInterno = (int)value; }
    }

    public float yMaxZona
    {
        get { return yMaxZonaInterno; }
        set { yMaxZonaInterno = (int)value; }
    }

    public void OnObjectSpawn()
    {
        yInicial = transform.position.y;
        tiempoAparicion = Time.time;
        cronometro = Random.Range(0f, tiempoEntreDisparos);
        
        bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;
        
        if (esMulti)
        {
            if (transform.position.y > 0)
            {
                GameObject pObj = GameObject.Find("Player1(Clone)") ?? GameObject.Find("Player1");
                if (pObj != null) jugador = pObj.transform;
            }
            else
            {
                GameObject pObj = GameObject.Find("Player2(Clone)") ?? GameObject.Find("Player2");
                if (pObj != null) jugador = pObj.transform;
            }
        }
        else
        {
            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) jugador = pObj.transform;
        }
    }

    void Update()
    {
        float esquivaY = 0f;
        if (jugador != null && Vector2.Distance(transform.position, jugador.position) < distanciaReaccion)
        {
            esquivaY = Mathf.Sign(transform.position.y - jugador.position.y) * velocidad * 0.5f * Time.deltaTime;
        }

        float nivelFactor = 1f;
        bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;

        if (esMulti && nivelJugadorAsignado > 0 && GameManager.Instance != null)
        {
            int nivelUsar = (nivelJugadorAsignado == 1) ? GameManager.Instance.rondaJugador1 : GameManager.Instance.rondaJugador2;
            nivelFactor += 0.05f * (nivelUsar - 1);
        }
        else if (GameManager.Instance != null)
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
        
        if (esMulti && nivelJugadorAsignado > 0 && GameManager.Instance != null)
        {
            int nivelUsar = (nivelJugadorAsignado == 1) ? GameManager.Instance.rondaJugador1 : GameManager.Instance.rondaJugador2;
            delayActual /= 1f + 0.1f * (nivelUsar - 1);
        }
        else if (GameManager.Instance != null)
        {
            delayActual /= 1f + 0.1f * (GameManager.Instance.rondaActual - 1);
        }

        if (jugador != null && Vector2.Distance(transform.position, jugador.position) < distanciaReaccion)
        {
            delayActual /= 2f;
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
                    
                    float dañoBase = 15f;
                    bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;
                    
                    if (esMulti && nivelJugadorAsignado > 0 && GameManager.Instance != null)
                    {
                        int nivelUsar = (nivelJugadorAsignado == 1) ? GameManager.Instance.rondaJugador1 : GameManager.Instance.rondaJugador2;
                        scriptBala.danio = dañoBase * (1f + (0.2f * (nivelUsar - 1)));
                    }
                    else if (GameManager.Instance != null)
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