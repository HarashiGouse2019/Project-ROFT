using UnityEngine;
using UnityEngine.UI;

public class EditorTimelineControl : MonoBehaviour
{
    public RectTransform origin;

    Material mat;

    public enum Signature
    {
        s_1by3,
        s_1by4,
        s_1by6,
        s_1by18
    }

    public Signature divisor;
    public float zoom = 1;

    private void Awake()
    {
        CreateLineMaterial();
    }

    private void OnDrawGizmos()
    {
        Vector3 startPoint = new Vector3(origin.position.x - (Screen.width / 2), origin.position.y, 1f);
        Vector3 endPoint = new Vector3(Screen.width, origin.position.y, 1f);

        Gizmos.DrawLine(startPoint, endPoint);
        Gizmos.color = Color.white;
    }

    void CreateTimelineControl()
    {
        Vector3 startPoint = new Vector3(origin.position.x - (Screen.width / 2), (Screen.height - origin.position.y) - 58, 1f);
        Vector3 endPoint = new Vector3(Screen.width, (Screen.height - origin.position.y) - 58, 1f);

        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadPixelMatrix();

        GL.Begin(GL.LINE_STRIP);
        GL.Color(Color.blue);
        GL.Vertex(startPoint);
        GL.Vertex(endPoint);
        GL.End();
        GL.PopMatrix();
    }

    void CreateLineMaterial()
    {
        var shader = Shader.Find("Hidden/Internal-Colored");

        mat = new Material(shader)
        {
            hideFlags = HideFlags.HideAndDontSave
        };

        //Turn on Alpha blending
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        //Turn backface culling off
        mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

        //Turn off deph writes
        mat.SetInt("_ZWrite", 0);
    }

    void OnRenderObject()
    {
        CreateTimelineControl();
    }

}
