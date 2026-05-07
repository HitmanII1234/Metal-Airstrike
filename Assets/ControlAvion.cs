using UnityEngine;

public class ControlAvion : MonoBehaviour
{
    [Header("Configuración de Vuelo")]
    public float fuerzaEmpuje = 5f;      
    public float velocidadRetorno = 2f;  
    public float limitesVerticales = 4f;

    [Header("Inclinación Realista")]
    public float inclinacionMaxima = 20f;
    public float suavidadGiro = 5f;

    [Header("Efectos del Avión")]
    public Transform helice;           
    public float velocidadHelice = 1500f;
    // Ahora es un arreglo para soportar 2 o más estelas
    public TrailRenderer[] estelasAire;   

    private float yDeseada = 0f;

    void Update()
    {
        // --- EFECTOS VISUALES ---
        if (helice != null)
        {
            helice.Rotate(Vector3.forward * velocidadHelice * Time.deltaTime);
        }

        // Control de múltiples estelas
        if (estelasAire != null && estelasAire.Length > 0)
        {
            bool estaMoviendose = Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;
            
            foreach (TrailRenderer trail in estelasAire)
            {
                if (trail != null) trail.emitting = estaMoviendose;
            }
        }

        // --- TU LÓGICA DE VUELO ---
        float entradaVertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(entradaVertical) > 0.1f)
        {
            yDeseada += entradaVertical * fuerzaEmpuje * Time.deltaTime;
        }
        else
        {
            yDeseada = Mathf.MoveTowards(yDeseada, 0f, velocidadRetorno * Time.deltaTime);
        }

        yDeseada = Mathf.Clamp(yDeseada, -limitesVerticales, limitesVerticales);

        float nuevaY = Mathf.Lerp(transform.position.y, yDeseada, Time.deltaTime * 5f);
        transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);

        float anguloZ = entradaVertical * inclinacionMaxima;
        
        Quaternion rotacionMeta = Quaternion.Euler(0, 0, anguloZ);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionMeta, suavidadGiro * Time.deltaTime);
    }
}