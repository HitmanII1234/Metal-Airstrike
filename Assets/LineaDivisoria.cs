using UnityEngine;

public class LineaDivisoria : MonoBehaviour
{
    public Color colorLinea = Color.white;
    public float grosorLinea = 3f;
    public bool soloMultijugador = true;

    private void OnGUI()
    {
        if (soloMultijugador && PlayerPrefs.GetInt("Multijugador", 0) == 0)
            return;

        float anchoPantalla = Screen.width;
        float mitadY = Screen.height / 2f;

        GL.PushMatrix();
        GL.LoadPixelMatrix();

        Color colorOriginal = GUI.color;
        GUI.color = colorLinea;

        // Dibujar línea horizontal en el centro
        for (int i = 0; i < grosorLinea; i++)
        {
            GUI.DrawTexture(new Rect(0, mitadY + i - grosorLinea / 2f, anchoPantalla, 1), Texture2D.whiteTexture);
        }

        GUI.color = colorOriginal;
        GL.PopMatrix();
    }
}
