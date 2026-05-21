using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance;

    public GameObject player1Prefab;
    public GameObject player2Prefab;

    public Camera cameraP1;
    public Camera cameraP2;

    // Layers para separar los mundos visualmente
    [HideInInspector] public int layerP1;
    [HideInInspector] public int layerP2;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        bool esMulti = PlayerPrefs.GetInt("Multijugador", 0) == 1;

        layerP1 = LayerMask.NameToLayer("MundoP1");
        layerP2 = LayerMask.NameToLayer("MundoP2");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.jugadoresVivos = esMulti ? 2 : 1;
        }

        if (esMulti)
        {
            float orthoSizeOriginal = cameraP1.orthographicSize;
            
            // Configurar cámaras para pantalla dividida
            // Cada cámara mantiene su orthographicSize completo pero muestra solo su mitad
            cameraP1.rect = new Rect(0, 0.5f, 1, 0.5f);
            cameraP2.gameObject.SetActive(true);
            cameraP2.rect = new Rect(0, 0, 1, 0.5f);
            
            // Mover las cámaras para que cada una muestre su propio mundo
            // P1: cámara centrada en Y=5 (ve de Y=0 a Y=10)
            Vector3 camPos1 = cameraP1.transform.position;
            cameraP1.transform.position = new Vector3(camPos1.x, 5f, camPos1.z);
            
            // P2: cámara centrada en Y=-5 (ve de Y=-10 a Y=0)
            Vector3 camPos2 = cameraP2.transform.position;
            cameraP2.transform.position = new Vector3(camPos2.x, -5f, camPos2.z);

            // Excluir el mundo contrario en cada cámara
            if (layerP1 >= 0 && layerP2 >= 0)
            {
                cameraP1.cullingMask &= ~(1 << layerP2);
                cameraP2.cullingMask &= ~(1 << layerP1);
            }

            // Posicionar jugadores en el centro de su propio mundo
            // P1: centrado en Y=5 (su mundo va de Y=0 a Y=10)
            // P2: centrado en Y=-5 (su mundo va de Y=-10 a Y=0)
            GameObject p1 = Instantiate(player1Prefab, new Vector3(-5, 5f, 0), Quaternion.identity);
            GameObject p2 = Instantiate(player2Prefab, new Vector3(-5, -5f, 0), Quaternion.identity);

            // Asignar número de jugador y límites de movimiento
            // Cada jugador se mueve libremente en su propio mundo de 10 unidades de alto
            ControlAvion ctrl1 = p1.GetComponent<ControlAvion>();
            if (ctrl1 != null)
            {
                ctrl1.numeroJugador = 1;
                ctrl1.minYMultiplayer = 0f;
                ctrl1.maxYMultiplayer = 10f;
            }

            ControlAvion ctrl2 = p2.GetComponent<ControlAvion>();
            if (ctrl2 != null)
            {
                ctrl2.numeroJugador = 2;
                ctrl2.minYMultiplayer = -10f;
                ctrl2.maxYMultiplayer = 0f;
            }

            Salud salud1 = p1.GetComponent<Salud>();
            if (salud1 != null) salud1.numeroJugador = 1;

            Salud salud2 = p2.GetComponent<Salud>();
            if (salud2 != null) salud2.numeroJugador = 2;

            // Asignar layers para aislar visualmente cada mundo
            if (layerP1 >= 0) AsignarLayerRecursivo(p1, layerP1);
            if (layerP2 >= 0) AsignarLayerRecursivo(p2, layerP2);

            // Asignar cámaras a las barras de vida si existen
            BarraBillboard bb1 = p1.GetComponentInChildren<BarraBillboard>();
            if (bb1 != null && cameraP1 != null) bb1.targetCamera = cameraP1;

            BarraBillboard bb2 = p2.GetComponentInChildren<BarraBillboard>();
            if (bb2 != null && cameraP2 != null) bb2.targetCamera = cameraP2;
        }
        else
        {
            cameraP1.rect = new Rect(0, 0, 1, 1);
            cameraP2.gameObject.SetActive(false);

            GameObject p1s = Instantiate(player1Prefab, new Vector3(-5, 0, 0), Quaternion.identity);

            ControlAvion ctrl = p1s.GetComponent<ControlAvion>();
            if (ctrl != null) ctrl.numeroJugador = 1;

            Salud salud = p1s.GetComponent<Salud>();
            if (salud != null) salud.numeroJugador = 1;

            BarraBillboard bbsolo = p1s.GetComponentInChildren<BarraBillboard>();
            if (bbsolo != null && cameraP1 != null) bbsolo.targetCamera = cameraP1;
        }
    }

    // Cambia el layer del objeto y todos sus hijos
    public static void AsignarLayerRecursivo(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform hijo in obj.transform)
        {
            AsignarLayerRecursivo(hijo.gameObject, layer);
        }
    }
}

