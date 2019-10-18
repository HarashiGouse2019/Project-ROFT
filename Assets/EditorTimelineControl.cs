using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorTimelineControl : MonoBehaviour
{

    Material mat;

    // Update is called once per frame
    void Awake()
    {
        CreateLineMaterial();
    }

    private void DrawTimelineControlBase()
    {
        if (!mat)
        {
            Debug.LogError("There is no spoon!!!");
            return;
        }

        Vector3 startPoint = new Vector3(transform.position.x - (Screen.width / 2), transform.position.y, 1f);
        Vector3 endPoint = new Vector3(Screen.width, transform.position.y, 1f);

        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadPixelMatrix();

        GL.Begin(GL.LINES);
        GL.Color(Color.white);
        GL.Vertex(startPoint);
        GL.Vertex(endPoint * 100);
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

    private void OnRenderObject()
    {
        DrawTimelineControlBase();
    }

}
