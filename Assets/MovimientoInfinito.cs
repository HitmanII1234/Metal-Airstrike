using UnityEngine;

public class MoverNube : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 0.5f;    // Qué tan rápido se mueve
    public float limiteIzquierdo = -15f; // Punto donde desaparece
    public float limiteDerecho = 15f;   // Punto donde reaparece (el otro lado)

    void Update()
    {
        // 1. Mueve la nube hacia la izquierda constantemente
        transform.Translate(Vector3.left * velocidad * Time.deltaTime);

        // 2. Si la posición X de la nube es menor al límite izquierdo...
        if (transform.position.x < limiteIzquierdo)
        {
            // 3. Teletranspórtala al límite derecho
            Vector3 nuevaPosicion = new Vector3(limiteDerecho, transform.position.y, transform.position.z);
            transform.position = nuevaPosicion;
        }
    }
}