using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    public int vidasPorRonda = 1;
    public int rondaActual = 1;
    public int score = 0;
    public int jugadoresVivos = 1;

    [Header("Progresión por Score")]
    public float scoreRequeridoActual = 1000f;
    public float multiplicadorScore = 1.15f;
    public int scoreObjetivoTotal = 1000;
    // Nombres / Records
    public string playerName = "Player";
    public string player2Name = "Player2";
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (PlayerPrefs.HasKey("Player1Name"))
                playerName = PlayerPrefs.GetString("Player1Name");
            if (PlayerPrefs.HasKey("Player2Name"))
                player2Name = PlayerPrefs.GetString("Player2Name");
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;

        // Comprobar si alcanzamos el score para subir de nivel
        if (score >= scoreObjetivoTotal)
        {
            // Calcular el siguiente objetivo de score
            scoreRequeridoActual *= multiplicadorScore;
            scoreObjetivoTotal += Mathf.RoundToInt(scoreRequeridoActual);
            
            if (CombatDirector.Instance != null)
            {
                CombatDirector.Instance.TerminarRondaPorScore();
            }
        }
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
        
        if (NivelManager.Instance != null)
        {
            NivelManager.Instance.MostrarGameOver(score);
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
        jugadoresVivos = 1;
        
        scoreRequeridoActual = 1000f;
        scoreObjetivoTotal = 1000;

        Time.timeScale = 1f;

        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.ResetPoderes();
        }
    }
}
