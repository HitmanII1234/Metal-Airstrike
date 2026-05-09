using UnityEngine;

public class IAEnemigo : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 3f;

    [Header("Disparo Automático")]
    public GameObject balaPrefab;
    public Transform puntoDisparo;
    public float tiempoEntreDisparos = 2f; // El "delay"
    private float cronometro;

    void Start()
    {
        // Inicializamos el cronómetro para que no disparen todos al mismo milisegundo al aparecer
        cronometro = Random.Range(0f, tiempoEntreDisparos);
    }

    void Update()
    {
        // Movimiento horizontal hacia la izquierda
        transform.Translate(Vector2.left * velocidad * Time.deltaTime);

        // Lógica del Delay
        cronometro += Time.deltaTime;

        if (cronometro >= tiempoEntreDisparos)
        {
            Disparar();
            cronometro = 0; // Reiniciar el delay
        }

        if (transform.position.x < -15f) Destroy(gameObject);
    }

    void Disparar()
    {
        if (balaPrefab != null && puntoDisparo != null)
        {
            GameObject balaObj = Instantiate(balaPrefab, puntoDisparo.position, Quaternion.identity);
            Bala scriptBala = balaObj.GetComponent<Bala>();
            
            if (scriptBala != null)
            {
                scriptBala.balaEnemiga = true; // Marcamos que esta bala es de un bot
            }
        }
    }
}