using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    public Camera cameraP1;
    public Camera cameraP2;

    void Start()
    {
        bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;

        if (esMulti)
        {
            // Configurar pantalla dividida
            cameraP1.rect = new Rect(0, 0.5f, 1, 0.5f);
            cameraP2.gameObject.SetActive(true);
            cameraP2.rect = new Rect(0, 0, 1, 0.5f);

            // Instanciar jugadores (asegurarse de tener referencias a los prefabs)
            Instantiate(player1Prefab, new Vector3(-5, 5, 0), Quaternion.identity);
            Instantiate(player2Prefab, new Vector3(-5, -5, 0), Quaternion.identity);
        }
        else
        {
            cameraP1.rect = new Rect(0, 0, 1, 1);
            cameraP2.gameObject.SetActive(false);
            
            Instantiate(player1Prefab, new Vector3(-5, 0, 0), Quaternion.identity);
        }
    }
}
