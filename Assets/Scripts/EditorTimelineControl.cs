using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorTimelineControl : MonoBehaviour
{
    public static EditorTimelineControl Instance;

    #region Public Members
    public Transform origin;

    [Header("Materials for Time Control")]
    public Material[] lineMaterials;

    public enum Signature
    {
        s_1by3,
        s_1by4,
        s_1by6,
        s_1by18
    }

    public Signature divisor;

    [Header("Scroll Value")]
    public float scroll = 100f;

    [Header("Zoom Value")]
    public float zoom = 1;

    public float pixelSpread = 0.01f;

    #endregion

    #region Private Members
    readonly int ResX = Screen.width;
    readonly int ResY = Screen.height;
    Ray ray;
    RaycastHit hit;

    bool timelineControlDrawn = false;

    public List<GameObject> lines = new List<GameObject>();
    #endregion

    private void Awake()
    {
        Instance = this;

        origin = transform;
        DrawTimelineControl();
    }

    void DrawTimelineControl()
    {
        if (!timelineControlDrawn)
        {
            float pixelsPerInch = ResY + (float)pixelSpread;
            float rulerWidth = 1000f;

            Vector3 startPoint = new Vector3(origin.position.x - (ResX / 2), origin.position.y - 0.85f, 1f);
            Vector3 endPoint = new Vector3(ResX, origin.position.y - 0.85f, 1f);

            //Base of Time Control
            DrawLine(startPoint, endPoint, 0);

            Transform screenRulerPosition = origin;
            screenRulerPosition.position = new Vector3(origin.position.x - (ResX / 2), origin.position.y - 0.265f);
            //Drawing the ticks for out Timeline Control
            for (float i = 1; i < (float)pixelsPerInch * rulerWidth; i += 0.0625f)
            {
                if ((i % (pixelsPerInch)) == 0)
                    DrawLine(screenRulerPosition, 0, i / zoom, 0.25f, i / zoom, 0);

                else if (((i * 2) % ((pixelsPerInch))) == 0)
                    DrawLine(screenRulerPosition, 0, i / zoom, 0.20f, i / zoom, 1);

                else if (((i * 4) % ((pixelsPerInch))) == 0)
                    DrawLine(screenRulerPosition, 0, i / zoom, 0.15f, i / zoom, 2);

                else if (((i * 8) % ((pixelsPerInch))) == 0)
                    DrawLine(screenRulerPosition, 0, i / zoom, 0.10f, i / zoom, 3);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                BakeLineRenderAsMesh(lines[i]);
            }

            timelineControlDrawn = true;
        }
    }

    void DrawLine(Vector3 _start, Vector3 _end, int _materialIndex, float _thickness = 0.02f)
    {
        GameObject myLine = new GameObject();

        myLine.transform.position = _start;
        myLine.AddComponent<LineRenderer>();

        LineRenderer lineRenderer = myLine.GetComponent<LineRenderer>();

        lineRenderer.material = lineMaterials[_materialIndex];

        lineRenderer.startWidth = _thickness;
        lineRenderer.endWidth = _thickness;

        lineRenderer.SetPosition(0, _start);
        lineRenderer.SetPosition(1, _end);
    }

    void DrawLine(Transform _transform, float _x1, float _y1, float _x2, float _y2, int _materialIndex, float _thickness = 0.02f)
    {
        GameObject myLine = new GameObject("line");

        myLine.transform.parent = gameObject.transform;

        Vector3 point1 = new Vector3(_transform.position.x + _y1, _transform.position.y + _x1, 1f);
        Vector3 point2 = new Vector3(_transform.position.x + _y2, _transform.position.y + _x2, 1f);

        myLine.AddComponent<LineRenderer>();

        LineRenderer lineRenderer = myLine.GetComponent<LineRenderer>();

        lineRenderer.material = lineMaterials[_materialIndex];

        lineRenderer.startWidth = _thickness;
        lineRenderer.endWidth = _thickness;

        lineRenderer.SetPosition(0, point1);
        lineRenderer.SetPosition(1, point2);

        lines.Add(myLine);
    }

    public void EnableScroll()
    {
        scroll = Input.GetAxis("Mouse ScrollWheel") * 2f;

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Debug.Log("Doing a thing!!!");
            for (int lineIndex = 1; lineIndex < lines.Count; lineIndex++)
            {
                GameObject lineToShift = lines[lineIndex];
                lineToShift.transform.position = new Vector3(lineToShift.transform.position.x + scroll, lineToShift.transform.position.y);
            }
        }
    }

    public void Zoom(string _dir)
    {
        int sign = 0;
        for (int lineIndex = 1; lineIndex < lines.Count; lineIndex++)
        {
            GameObject lineToManipulate = lines[lineIndex];
            

            switch (_dir.ToLower())
            {
                case "in":
                    sign = 1;
                    break;
                case "out":
                    sign = -1;
                    break;
                default:
                    //Do nothing
                    break;
            }

            lineToManipulate.transform.position = new Vector3(lineToManipulate.transform.position.x + (sign * (lineIndex * 0.01f)), lineToManipulate.transform.position.y);
        }
    }

    public static void BakeLineRenderAsMesh(GameObject _obj)
    {
        var lineRenderer = _obj.GetComponent<LineRenderer>();
        var meshFilter = _obj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh);
        meshFilter.sharedMesh = mesh;

        var meshRenderer = _obj.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = lineRenderer.material;

        GameObject.Destroy(lineRenderer);
    }
}