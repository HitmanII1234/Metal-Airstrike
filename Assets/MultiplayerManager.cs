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
            cameraP1.rect = new Rect(0, 0.5f, 1, 0.5f);
            cameraP2.gameObject.SetActive(true);
            cameraP2.rect = new Rect(0, 0, 1, 0.5f);

            // Excluir el mundo contrario en cada cámara
            if (layerP1 >= 0 && layerP2 >= 0)
            {
                cameraP1.cullingMask &= ~(1 << layerP2);
                cameraP2.cullingMask &= ~(1 << layerP1);
            }

            GameObject p1 = Instantiate(player1Prefab, new Vector3(-5, 5, 0), Quaternion.identity);
            GameObject p2 = Instantiate(player2Prefab, new Vector3(-5, -5, 0), Quaternion.identity);

            // Asignar número de jugador
            ControlAvion ctrl1 = p1.GetComponent<ControlAvion>();
            if (ctrl1 != null) ctrl1.numeroJugador = 1;

            ControlAvion ctrl2 = p2.GetComponent<ControlAvion>();
            if (ctrl2 != null) ctrl2.numeroJugador = 2;

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

