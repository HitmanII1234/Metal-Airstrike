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
        if (!lookAtCamera || targetCamera == null) return;

        Vector3 dir = transform.position - targetCamera.transform.position;
        dir.y = 0; // opcional: evita rotar en el eje X
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}
