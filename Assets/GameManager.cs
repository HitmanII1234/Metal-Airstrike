using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int vidasPorRonda = 1;
    public int rondaActual = 1;
    public int score = 0;
    
    // Mejoras (Roguelike - Permanentes)
    public bool hasLifeSteal = false;
    public float bulletImmunityChance = 0f;
    public bool hasMagnetism = false;
    
    // Nombres / Records
    public string playerName = "Player";
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    public void PlayerDied()
    {
        vidasPorRonda--;
        if (vidasPorRonda <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over! Score: " + score + " Ronda: " + rondaActual);
        // Regresar al menu principal o mostrar pantalla de resultados
    }

    public void SiguienteRonda()
    {
        rondaActual++;
        vidasPorRonda = 1; // Reinicia vidas (ajustable con mejoras)
        // Podría llamar a la selección de poderes
    }
}
