using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int vidasPorRonda = 1;
    public int rondaActual = 1;
    public int score = 0;
    public int jugadoresVivos = 1;
    
    // Mejoras (Roguelike - Permanentes)
    public bool hasLifeSteal = false;
    public float bulletImmunityChance = 0f;
    public bool hasMagnetism = false;
    
    // Nombres / Records
    public string playerName = "Player";
    public string player2Name = "Player2";
    
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
        jugadoresVivos--;
        if (jugadoresVivos <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over! Score: " + score + " Ronda: " + rondaActual);
        Time.timeScale = 0f; // Pausa el juego
        
        MenuController menu = FindObjectOfType<MenuController>();
        if (menu != null)
        {
            menu.MostrarGameOver(score);
        }
    }

    public void SiguienteRonda()
    {
        rondaActual++;
        // Reiniciar jugadores si se desea
    }
    
    public void ResetearEstadisticas()
    {
        score = 0;
        rondaActual = 1;
        Time.timeScale = 1f;
    }
}
