using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreEntry
{
    public string nombre;
    public string nombre2;
    public int score;
    public int nivel;
    public bool esMultijugador;
    public string fecha;

    public string GetDisplayText()
    {
        if (esMultijugador)
        {
            return nombre + " & " + nombre2 + " | Nivel " + nivel + " | Score: " + score;
        }
        else
        {
            return nombre + " | Nivel " + nivel + " | Score: " + score;
        }
    }
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private const string SCORE_PREFIX = "ScoreEntry_";
    private const string SCORE_COUNT_KEY = "ScoreCount";

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

    public void GuardarScore(string nombre, string nombre2, int score, int nivel, bool esMulti)
    {
        int count = PlayerPrefs.GetInt(SCORE_COUNT_KEY, 0);

        ScoreEntry nuevaEntrada = new ScoreEntry
        {
            nombre = nombre,
            nombre2 = nombre2,
            score = score,
            nivel = nivel,
            esMultijugador = esMulti,
            fecha = DateTime.Now.ToString("dd/MM/yyyy")
        };

        PlayerPrefs.SetString(SCORE_PREFIX + count, JsonUtility.ToJson(nuevaEntrada));
        count++;
        PlayerPrefs.SetInt(SCORE_COUNT_KEY, count);
        PlayerPrefs.Save();

        Debug.Log("[ScoreManager] Score guardado: " + nuevaEntrada.GetDisplayText());
    }

    public List<ScoreEntry> ObtenerTodosLosScores()
    {
        List<ScoreEntry> scores = new List<ScoreEntry>();
        int count = PlayerPrefs.GetInt(SCORE_COUNT_KEY, 0);

        for (int i = 0; i < count; i++)
        {
            string json = PlayerPrefs.GetString(SCORE_PREFIX + i, "");
            if (!string.IsNullOrEmpty(json))
            {
                ScoreEntry entry = JsonUtility.FromJson<ScoreEntry>(json);
                if (entry != null)
                {
                    scores.Add(entry);
                }
            }
        }

        scores.Sort((a, b) => b.score.CompareTo(a.score));
        return scores;
    }

    public void BorrarTodosLosScores()
    {
        int count = PlayerPrefs.GetInt(SCORE_COUNT_KEY, 0);
        for (int i = 0; i < count; i++)
        {
            PlayerPrefs.DeleteKey(SCORE_PREFIX + i);
        }
        PlayerPrefs.DeleteKey(SCORE_COUNT_KEY);
        PlayerPrefs.Save();
    }
}
