using UnityEngine;

public class BarraBillboard : MonoBehaviour
{
    public bool lookAtCamera = true;
    public Camera targetCamera;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (!lookAtCamera || targetCamera == null || this == null) return;

        try
        {
            Vector3 dir = transform.position - targetCamera.transform.position;
            dir.y = 0; // opcional: evita rotar en el eje X
            if (dir.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }
        catch (System.Exception)
        {
            // Evita errores en consola al descargar la escena
        }
    }
}
