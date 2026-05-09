using UnityEngine;

public class ControlAvion : MonoBehaviour
{
    [Header("Configuración de Vuelo")]
    public float fuerzaEmpuje = 5f;      
    public float velocidadRetorno = 2f;  
    public float limitesVerticales = 4f;
    public float limitesHorizontales = 8f; // Nuevo: Límite para A y D

    [Header("Disparo")]
    public GameObject balaPrefab;
    public Transform puntoDisparo;
    public float danioBala = 25f;

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

        // --- MOVIMIENTO VERTICAL (W/S) ---
        if (Mathf.Abs(entradaVertical) > 0.1f)
            yDeseada += entradaVertical * fuerzaEmpuje * Time.deltaTime;
        else
            yDeseada = Mathf.MoveTowards(yDeseada, 0f, velocidadRetorno * Time.deltaTime);

        // --- MOVIMIENTO HORIZONTAL (A/D) ---
        if (Mathf.Abs(entradaHorizontal) > 0.1f)
            xDeseada += entradaHorizontal * fuerzaEmpuje * Time.deltaTime;

        yDeseada = Mathf.Clamp(yDeseada, -limitesVerticales, limitesVerticales);
        xDeseada = Mathf.Clamp(xDeseada, -limitesHorizontales, limitesHorizontales);

        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, xDeseada, Time.deltaTime * 5f),
            Mathf.Lerp(transform.position.y, yDeseada, Time.deltaTime * 5f),
            transform.position.z
        );

        // Rotación e Inclinación
        float anguloZ = entradaVertical * inclinacionMaxima;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, anguloZ), suavidadGiro * Time.deltaTime);

        // --- DISPARO ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Disparar();
        }

        // Estelas
        if (estelasAire != null)
        {
            bool estaMoviendose = Mathf.Abs(entradaVertical) > 0.1f || Mathf.Abs(entradaHorizontal) > 0.1f;
            foreach (TrailRenderer trail in estelasAire)
                if (trail != null) trail.emitting = estaMoviendose;
        }
    }

    void Disparar()
    {
        GameObject bala = Instantiate(balaPrefab, puntoDisparo.position, Quaternion.identity);
        Bala scriptBala = bala.GetComponent<Bala>();
        scriptBala.danio = danioBala;
    }
}