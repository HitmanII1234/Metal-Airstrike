using UnityEngine;

public class ControlAvion : MonoBehaviour
{
    [Header("Configuración de Vuelo")]
    public float fuerzaEmpuje = 5f;      
    public float velocidadRetorno = 2f;  
    public float limitesVerticales = 4f;
    public float limitesHorizontales = 8f; 

    [Header("Disparo")]
    public string tagBala = "BalaJugador";
    public Transform puntoDisparo;
    public float danioBala = 25f;
    public float cadenciaDisparo = 0.2f;
    private float tiempoProximoDisparo = 0f;
    public bool tieneDobleDisparo = false;

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

        float entradaVertical = Input.GetAxis("Vertical");
        float entradaHorizontal = Input.GetAxis("Horizontal");

        if (Mathf.Abs(entradaVertical) > 0.1f)
            yDeseada += entradaVertical * fuerzaEmpuje * Time.deltaTime;
        else
            yDeseada = Mathf.MoveTowards(yDeseada, 0f, velocidadRetorno * Time.deltaTime);

        if (Mathf.Abs(entradaHorizontal) > 0.1f)
            xDeseada += entradaHorizontal * fuerzaEmpuje * Time.deltaTime;

        yDeseada = Mathf.Clamp(yDeseada, -limitesVerticales, limitesVerticales);
        xDeseada = Mathf.Clamp(xDeseada, -limitesHorizontales, limitesHorizontales);

        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, xDeseada, Time.deltaTime * 5f),
            Mathf.Lerp(transform.position.y, yDeseada, Time.deltaTime * 5f),
            transform.position.z
        );

        float anguloZ = entradaVertical * inclinacionMaxima;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, anguloZ), suavidadGiro * Time.deltaTime);

        if (Input.GetKey(KeyCode.Space) && Time.time >= tiempoProximoDisparo)
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