using UnityEngine;

public class ControlAvion : MonoBehaviour
{
    [Header("Configuración de Vuelo")]
    public float velocidad = 10f; // Qué tan rápido sube y baja
    public float limitesVerticales = 4.5f; // Para que no se salga de la pantalla

    void Update()
    {
        // 1. Detectar la entrada del jugador (Flechas arriba/abajo o W/S)
        float movimientoY = Input.GetAxis("Vertical");

        // 2. Calcular la nueva posición
        // transform.Translate mueve el objeto en base a una dirección
        transform.Translate(Vector3.up * movimientoY * velocidad * Time.deltaTime);

        // 3. (Opcional) Limitar el movimiento para que no desaparezca
        float yLimitada = Mathf.Clamp(transform.position.y, -limitesVerticales, limitesVerticales);
        transform.position = new Vector3(transform.position.x, yLimitada, transform.position.z);
    }
}