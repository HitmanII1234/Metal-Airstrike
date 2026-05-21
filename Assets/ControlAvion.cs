using UnityEngine;

public class ControlAvion : MonoBehaviour
{
    [Header("Configuración de Vuelo")]
    public float fuerzaEmpuje = 5f;      
    public float velocidadRetorno = 2f;  
    public float limitesVerticales = 4f;
    public float limitesHorizontales = 8f; 

    [Header("Restricción Multijugador")]
    public float minYMultiplayer = 0f;
    public float maxYMultiplayer = 10f;

    [Header("Disparo")]
    public string tagBala = "BalaJugador";
    public Transform puntoDisparo;
    public float danioBala = 25f;
    public float cadenciaDisparo = 0.2f;
    private float tiempoProximoDisparo = 0f;
    public bool tieneDobleDisparo = false;

    [Header("Controles Personalizados")]
    public string ejeVertical = "Vertical";
    public string ejeHorizontal = "Horizontal";
    public KeyCode teclaDisparo = KeyCode.Space;
    public int numeroJugador = 1;

    [Header("Inclinación Realista")]
    public float inclinacionMaxima = 20f;
    public float suavidadGiro = 5f;

    [Header("Efectos del Avión")]
    public Transform helice;           
    public float velocidadHelice = 1500f;
    public TrailRenderer[] estelasAire;   

    private float yDeseada = 0f;
    private float xDeseada = 0f;

    void Start()
    {
        xDeseada = transform.position.x;
    }

    void Update()
    {
        if (helice != null)
            helice.Rotate(Vector3.forward * velocidadHelice * Time.deltaTime);

        float entradaVertical = 0f;
        float entradaHorizontal = 0f;

        bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;

        if (!esMulti)
        {
            entradaVertical = Input.GetAxis(ejeVertical);
            entradaHorizontal = Input.GetAxis(ejeHorizontal);
        }
        else
        {
            if (numeroJugador == 1)
            {
                if (Input.GetKey(KeyCode.W)) entradaVertical = 1f;
                else if (Input.GetKey(KeyCode.S)) entradaVertical = -1f;

                if (Input.GetKey(KeyCode.D)) entradaHorizontal = 1f;
                else if (Input.GetKey(KeyCode.A)) entradaHorizontal = -1f;

                teclaDisparo = KeyCode.X;
            }
            else if (numeroJugador == 2)
            {
                if (Input.GetKey(KeyCode.UpArrow)) entradaVertical = 1f;
                else if (Input.GetKey(KeyCode.DownArrow)) entradaVertical = -1f;

                if (Input.GetKey(KeyCode.RightArrow)) entradaHorizontal = 1f;
                else if (Input.GetKey(KeyCode.LeftArrow)) entradaHorizontal = -1f;

                teclaDisparo = KeyCode.Space;
            }
        }

        if (Mathf.Abs(entradaVertical) > 0.1f)
            yDeseada += entradaVertical * fuerzaEmpuje * Time.deltaTime;
        else
            yDeseada = Mathf.MoveTowards(yDeseada, 0f, velocidadRetorno * Time.deltaTime);

        if (Mathf.Abs(entradaHorizontal) > 0.1f)
            xDeseada += entradaHorizontal * fuerzaEmpuje * Time.deltaTime;

        float minY = -limitesVerticales;
        float maxY = limitesVerticales;

        if (esMulti)
        {
            if (numeroJugador == 1)
            {
                // Jugador 1: su mundo va de Y=0 a Y=10
                minY = 0f;
                maxY = 10f;
            }
            else if (numeroJugador == 2)
            {
                // Jugador 2: su mundo va de Y=-10 a Y=0
                minY = -10f;
                maxY = 0f;
            }
        }

        yDeseada = Mathf.Clamp(yDeseada, minY, maxY);
        xDeseada = Mathf.Clamp(xDeseada, -limitesHorizontales, limitesHorizontales);

        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, xDeseada, Time.deltaTime * 5f),
            Mathf.Lerp(transform.position.y, yDeseada, Time.deltaTime * 5f),
            transform.position.z
        );

        float anguloZ = entradaVertical * inclinacionMaxima;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, anguloZ), suavidadGiro * Time.deltaTime);

        if (Input.GetKey(teclaDisparo) && Time.time >= tiempoProximoDisparo)
        {
            Disparar();
            tiempoProximoDisparo = Time.time + cadenciaDisparo;
        }

        if (estelasAire != null)
        {
            bool estaMoviendose = Mathf.Abs(entradaVertical) > 0.1f || Mathf.Abs(entradaHorizontal) > 0.1f;
            foreach (TrailRenderer trail in estelasAire)
                if (trail != null) trail.emitting = estaMoviendose;
        }
    }

    void Disparar()
    {
        if (ObjectPool.Instance == null) return;
        
        if (tieneDobleDisparo)
        {
            SpawnBala(puntoDisparo.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
            SpawnBala(puntoDisparo.position + new Vector3(0, -0.3f, 0), Quaternion.identity);
        }
        else
        {
            SpawnBala(puntoDisparo.position, Quaternion.identity);
        }
    }

    void SpawnBala(Vector3 pos, Quaternion rot)
    {
        GameObject bala = ObjectPool.Instance.SpawnFromPool(tagBala, pos, rot);
        if (bala != null)
        {
            Bala scriptBala = bala.GetComponent<Bala>();
            if (scriptBala != null)
            {
                scriptBala.danio = danioBala;
                scriptBala.balaEnemiga = false;
            }
        }
    }
}