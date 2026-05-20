using UnityEngine;

public class Bala : MonoBehaviour, IPooleable
{
    public float velocidad = 10f;
    public float danio = 25f;
    public bool balaEnemiga = false; 

    public void OnObjectSpawn()
    {
        // Se llama al reaparecer desde el pool
    }

    void Update()
    {
        float direccion = balaEnemiga ? -1f : 1f;
        transform.Translate(Vector2.right * direccion * velocidad * Time.deltaTime);

        if (Mathf.Abs(transform.position.x) > 20f) 
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        Salud saludObj = otro.GetComponent<Salud>();
        if (saludObj == null) return; // Si no tiene vida, ignorar

        string tagObjetivo = otro.gameObject.tag.Trim();
        bool esJugador = tagObjetivo == "Player" || otro.name.Contains("Player");

        if (balaEnemiga && esJugador) 
        {
            Dañar(otro);
        }
        else if (!balaEnemiga && !esJugador)
        {
            // Si la bala es nuestra y el objetivo NO es el jugador, asume que es enemigo
            Dañar(otro);
        }
    }

    void Dañar(Collider2D objetivo)
    {
        Salud salud = objetivo.GetComponent<Salud>();
        if (salud != null) salud.RecibirDanio(danio);
        gameObject.SetActive(false);
    }
}