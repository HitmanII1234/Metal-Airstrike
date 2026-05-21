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

    [Header("Scores y Niveles por Jugador (Multijugador)")]
    public int scoreJugador1 = 0;
    public int scoreJugador2 = 0;
    public int rondaJugador1 = 1;
    public int rondaJugador2 = 1;
    public float scoreObjetivoJ1 = 1000f;
    public float scoreObjetivoJ2 = 1000f;
    public float multiplicadorScore = 1.15f;

    [Header("Progresión por Score (Un Jugador)")]
    public float scoreRequeridoActual = 1000f;
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
        AddScore(amount, 0);
    }

    public void AddScore(int amount, int numeroJugador)
    {
        bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;

        if (esMulti && numeroJugador > 0)
        {
            if (numeroJugador == 1)
            {
                scoreJugador1 += amount;
                    if (scoreJugador1 >= scoreObjetivoJ1)
                    {
                        scoreObjetivoJ1 += Mathf.RoundToInt(scoreObjetivoJ1 * multiplicadorScore);
                        rondaJugador1++;
                        
                        if (NivelManager.Instance != null)
                        {
                            NivelManager.Instance.MostrarAvisoSubidaNivel(1, rondaJugador1);
                        }
                        
                        if (CombatDirector.Instance != null)
                        {
                            CombatDirector.Instance.SubirNivelJugador(1);
                        }
                    }
            }
            else if (numeroJugador == 2)
            {
                scoreJugador2 += amount;
                    if (scoreJugador2 >= scoreObjetivoJ2)
                    {
                        scoreObjetivoJ2 += Mathf.RoundToInt(scoreObjetivoJ2 * multiplicadorScore);
                        rondaJugador2++;
                        
                        if (NivelManager.Instance != null)
                        {
                            NivelManager.Instance.MostrarAvisoSubidaNivel(2, rondaJugador2);
                        }
                        
                        if (CombatDirector.Instance != null)
                        {
                            CombatDirector.Instance.SubirNivelJugador(2);
                        }
                    }
            }
        }
        else
        {
            score += amount;

            if (score >= scoreObjetivoTotal)
            {
                scoreRequeridoActual *= multiplicadorScore;
                scoreObjetivoTotal += Mathf.RoundToInt(scoreRequeridoActual);
                
                if (CombatDirector.Instance != null)
                {
                    CombatDirector.Instance.TerminarRondaPorScore();
                }
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
        bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;
        int scoreFinal = esMulti ? (scoreJugador1 + scoreJugador2) : score;
        int nivelFinal = esMulti ? Mathf.Max(rondaJugador1, rondaJugador2) : rondaActual;
        
        Debug.Log("Game Over! Score: " + scoreFinal + " Ronda: " + rondaActual);
        Time.timeScale = 0f;
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.GuardarScore(playerName, player2Name, scoreFinal, nivelFinal, esMulti);
        }
        
        if (NivelManager.Instance != null)
        {
            NivelManager.Instance.MostrarGameOver(scoreFinal);
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
        
        scoreJugador1 = 0;
        scoreJugador2 = 0;
        rondaJugador1 = 1;
        rondaJugador2 = 1;
        scoreObjetivoJ1 = 1000f;
        scoreObjetivoJ2 = 1000f;
        
        scoreRequeridoActual = 1000f;
        scoreObjetivoTotal = 1000;

        Time.timeScale = 1f;

        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.ResetPoderes();
        }
    }
}
