/**

    This class demonstrates the code discussed in these two articles:

    http://devmag.org.za/2011/04/05/bzier-curves-a-tutorial/
    http://devmag.org.za/2011/06/23/bzier-path-algorithms/

    Use this code as you wish, at your own risk. If it blows up 
    your computer, makes a plane crash, or otherwise cause damage,
    injury, or death, it is not my fault.

    @author Herman Tulleken, dev.mag.org.za

*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierManager : MonoBehaviour
{

    public enum Mode
    {
        Line, //Draws Line Segments at points
        Bezier, //Interprets points as control points of Bezier curve
        BezierInterpolated, //Interpolates 
        BezierReduced
    }

    public Mode mode;
    public List<Vector3> points=new List<Vector3>();
    private List<Vector3> gizmos;
    public LineRenderer lineRenderer;

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
       
        mode = Mode.BezierInterpolated;
    }

    // Update is called once per frame
    void Update()
    {
        //ProcessInput();
        //Render();
    }

    private void ProcessInput()
    {
        if (mode == Mode.BezierReduced)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //points.Clear();
            }
            if (Input.GetMouseButton(0))
            {
                Vector2 screenPosition = Input.mousePosition;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 4));

                points.Add(worldPosition);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 screenPosition = Input.mousePosition;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 4));

                points.Add(worldPosition);
            }
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            mode = Mode.Line;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            mode = Mode.Bezier;
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            mode = Mode.BezierInterpolated;
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            mode = Mode.BezierReduced;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            points.Clear();
        }
    }


    ///Note: this file merely illustrate the algorithms.
    ///Generally, they should NOT be called each frame!
    public void Render(Vector3 porta1, Vector3 porta2)
    {
       // Debug.Log("Render");
        float aux = Mathf.Abs(porta1.x - porta2.x);
        aux = aux * (10.0f/100.0f);
        points.Clear();
        points.Add(porta1);
        points.Add(porta1 + Vector3.right * aux + Vector3.up*3);
        points.Add(porta1 / 2 + porta2 / 2);
        points.Add(porta2 + Vector3.left * aux + Vector3.down * 3);
        points.Add(porta2);
        /*
        for (int i = 0; i < points.Count; i++)
        {
            points[i] -= Vector3.forward*100;
        }
        */
        switch (mode)
        {
            case Mode.Line:
                RenderLineSegments();
                break;
            case Mode.Bezier:
                RenderBezier();
                break;
            case Mode.BezierInterpolated:
                BezierInterpolate();
                break;
            case Mode.BezierReduced:
                BezierReduce();
                break;


        }
    }

    private void RenderLineSegments()
    {
        gizmos = points;
        SetLinePoints(points);
    }

    private void RenderBezier()
    {
        BezierPath bezierPath = new BezierPath();

        bezierPath.SetControlPoints(points);
        List<Vector3> drawingPoints = bezierPath.GetDrawingPoints2();

        gizmos = drawingPoints;

        SetLinePoints(drawingPoints);
    }

    private void BezierInterpolate()
    {
        BezierPath bezierPath = new BezierPath();
        bezierPath.Interpolate(points, .25f);

        List<Vector3> drawingPoints = bezierPath.GetDrawingPoints2();

        gizmos = bezierPath.GetControlPoints();

        SetLinePoints(drawingPoints);
    }

    private void BezierReduce()
    {
        BezierPath bezierPath = new BezierPath();
        bezierPath.SamplePoints(points, 10, 1000, 0.33f);

        List<Vector3> drawingPoints = bezierPath.GetDrawingPoints2();
        Debug.Log(gizmos.Count);

        gizmos = bezierPath.GetControlPoints();
        SetLinePoints(drawingPoints);
    }

    private void SetLinePoints(List<Vector3> drawingPoints)
    {
        lineRenderer.SetVertexCount(drawingPoints.Count);

       
        for (int i = 0; i < drawingPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, drawingPoints[i]);
        }
    }

    public void OnDrawGizmos()
    {
        if (gizmos == null)
        {
            return;
        }

        for (int i = 0; i < gizmos.Count; i++)
        {
            Gizmos.DrawWireSphere(gizmos[i], 1f);
        }
    }
    /*
    public void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        GUILayout.Label("F1 Line Segments (Click to add points)");
        GUILayout.Label("F2 Bezier curve (Click to add points)");
        GUILayout.Label("F3 Bezier interpolation (Click to add points)");
        GUILayout.Label("F4 Bezier sampling / reduction (Drag to add points)");
        GUILayout.Label("X  Clear");
        GUILayout.Label("");
        GUILayout.Label("Switch on Gizmos in Unity to view control points");

        GUILayout.EndArea();
    }
     */
}

