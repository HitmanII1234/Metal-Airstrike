using UnityEngine;

public class Bala : MonoBehaviour
{
    public float velocidad = 10f;
    public float danio = 25f;
    public bool balaEnemiga = false; // Si es true, viajará a la izquierda

    void Update()
    {
        // Si es bala enemiga va a la izquierda (-), si es del jugador a la derecha (+)
        float direccion = balaEnemiga ? -1f : 1f;
        transform.Translate(Vector2.right * direccion * velocidad * Time.deltaTime);

        // Destruir si sale de pantalla
        if (Mathf.Abs(transform.position.x) > 20f) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        // Si la bala es enemiga, solo daña al jugador. Si es del jugador, solo daña al enemigo.
        if (balaEnemiga && otro.CompareTag("Player")) 
        {
            Dañar(otro);
        }
        else if (!balaEnemiga && otro.CompareTag("Enemigo"))
        {
            Dañar(otro);
        }
    }

    void Dañar(Collider2D objetivo)
    {
        Salud salud = objetivo.GetComponent<Salud>();
        if (salud != null) salud.RecibirDanio(danio);
        Destroy(gameObject);
    }
}