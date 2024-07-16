using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class LineInputPen : MonoBehaviour
{
   
    // Start is called before the first frame update
    private Spline Line;
    public float Threshold;
    public int iterations = 1; // Number of smoothing iterations
    public float smoothingFactor = 0.5f;
    public string ShapeName;
    public List<Vector3[]> ShapeSamples;
    public List<GameObject> DrawnShapes;
    public Transform Hand;

    
    
    public bool Pressed;
    void Start()
    {
        Line = GetComponent<SplineContainer>().Splines[0];
        ShapeSamples = new List<Vector3[]>();
        DrawnShapes = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (XrInput.Right_Grip.IsPressed())
        {
            Pressed = true;
            Line.Add(new BezierKnot(GetInputPosition()),TangentMode.AutoSmooth);
            Debug.Log("Tracking");
            
        }
        else
        {
            if (XrInput.Right_Grip.WasReleasedThisFrame()&& Pressed)
            {
                Debug.Log("ButtonUp");
                Pressed = false;
                if (Line.Count > 6)
                {
                    SmoothSpline(Line, iterations, smoothingFactor);

                    
                    // //Show the drawn Line
                    // var Seepath = new GameObject("Drawn", new[] { typeof(LineRenderer) });
                    // var L = Seepath.GetComponent<LineRenderer>();
                    // //Debug.Log(Line.Count);
                    // L.positionCount = 60;
                    // L.SetPositions(ConvertSplineToArray(Line, 60));
                    // L.startWidth = 0.01f;
                    // L.endWidth = 0.01f;
                    // L.material = new Material(Shader.Find("Unlit/Color"));
                    // L.material.color = Color.red;
                    
                    
                    var Points = ConvertSplineToArray(Line, 60);
                    Points = ScaleToSetSize(Points,0.45f);
                    Points = MakeLineSegment(Points);
                    Points = TranslateToOrigin(Points);
                    Points = RotatePoints(Points);
                    
                    
                    ShapeSamples.Add(Points);
                    // DrawnShapes.Add(ShownLine.gameObject);
                    
                    
                    // ShownLine.SetPositions(Points);
                    // ShownLine.material = new Material(Shader.Find("Unlit/Color"));
                    // ShownLine.material.color = Color.black;
                }

                Line.Clear();
                Line = new Spline();
                
                //Compare the current Line with the stored ones
                GetComponent<LineContainerPen>().CheckData(ShapeSamples.Last());
            }
        }

        var pos = GetInputPosition();
        if (Pressed && Vector3.Distance(pos, Line.Last().Position) > Threshold)
        {
            Line.Add(new BezierKnot(pos), TangentMode.AutoSmooth);
        }


        // if (XrInput.Left_Primary.WasPressedThisFrame())
        // {
        //     Debug.Log($"saving with samples: {ShapeSamples.Count}");
        //     var Avgs = JsonConvert.SerializeObject(AveragePoints(ShapeSamples));
        //     File.WriteAllText(Application.dataPath+$"/Shapes/{ShapeName}.txt",Avgs);
        // }
        
    }

    Vector3 GetInputPosition()
    {
        // return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return Hand.position;
    }

    void SmoothSpline(Spline spline, int iterations, float factor)
    {
        for (int iter = 0; iter < iterations; iter++)
        {
            for (int i = 1; i < spline.Count - 1; i++)
            {
                Vector3 prev = spline[i - 1].Position;
                Vector3 next = spline[i + 1].Position;
                Vector3 current = spline[i].Position;

                Vector3 newPosition = current + factor * ((prev + next) / 2 - current);
                spline[i] = new BezierKnot(newPosition, spline[i].TangentIn, spline[i].TangentOut, spline[i].Rotation);
            }
        }
    }
    
    public static Vector3[] MakeLineSegment(Vector3[] points)
    {
        if (points.Length < 3)
            return new []{Vector3.zero,Vector3.zero,Vector3.zero}; // Not enough points to form a line segment

        Vector3 p0 = points[0];
        Vector3 p2 = points[2];

        // Calculate the direction vector from p0 to p2
        Vector3 direction = p2 - p0;

        // Modify the second point to be halfway between p0 and p2
        points[1] = p0 + direction * 0.5f;

        return points;
    }
    Vector3[] ConvertSplineToArray(Spline spline, int sampleCount)
    {
        Vector3[] points = new Vector3[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)(sampleCount - 1); // Normalized parameter [0, 1]
            points[i] = spline.EvaluatePosition(t);
        }
        return points;
    }
    
    public Vector3[] ScaleToSetSize(Vector3[] points, float setSize)
    {
        if (points.Length < 2)
        {
            Debug.LogError("Not enough points to scale.");
            return points;
        }

        Vector3 direction = points[1] - points[0];
        float currentDistance = direction.magnitude;
        float scaleFactor = setSize / currentDistance;

        Vector3[] scaledPoints = new Vector3[points.Length];
        scaledPoints[0] = points[0];
        for (int i = 1; i < points.Length; i++)
        {
            scaledPoints[i] = points[0] + (points[i] - points[0]) * scaleFactor;
        }

        return scaledPoints;
    }
    
    public static Vector3[] RotatePoints(Vector3[] points)
    {
        if (points.Length < 3)
        {
            Debug.LogError("Need at least three points to perform the rotation.");
            return points;
        }

        // Calculate the normal vector from the first three points
        Vector3 normal = Vector3.Cross(points[1] - points[0], points[2] - points[0]).normalized;

        // Determine the rotation to make the normal vector point upwards
        Quaternion rotation = Quaternion.FromToRotation(normal, Vector3.up);

        // Apply the rotation to all points
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = rotation * points[i];
        }

        // Recalculate the direction vector from point 0 to point 1 after the rotation
        Vector3 direction01 = (points[1] - points[0]).normalized;

        // Determine the rotation to make the direction vector point towards the positive Z-axis
        Vector3 targetDirection = new Vector3(0, 0, 1);
        Quaternion alignment = Quaternion.FromToRotation(direction01, targetDirection);

        // Apply the alignment rotation to all points
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = alignment * points[i];
        }

        return points;
    }


    
    public Vector3[] TranslateToOrigin(Vector3[] points)
    {
        if (points.Length == 0)
        {
            Debug.LogError("No points to translate.");
            return points;
        }

        Vector3[] translatedPoints = new Vector3[points.Length];
        Vector3 offset = points[0]; // Offset to move the first point to (0,0,0)
        
        for (int i = 0; i < points.Length; i++)
        {
            translatedPoints[i] = points[i] - offset;
        }

        return translatedPoints;
    }
    
    public static Vector3[] AveragePoints(List<Vector3[]> listOfPoints)
    {
        if (listOfPoints == null || listOfPoints.Count == 0)
        {
            Debug.LogError("No lists provided to average.");
            return null;
        }

        int pointCount = listOfPoints[0].Length;
        Vector3[] averagePoints = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            Vector3 sum = Vector3.zero;
            foreach (Vector3[] points in listOfPoints)
            {
                if (points.Length != pointCount)
                {
                    Debug.LogError("All Vector3 arrays must have the same length.");
                    return null;
                }
                sum += points[i];
            }
            averagePoints[i] = sum / listOfPoints.Count;
        }

        return averagePoints;
    }
    
    void CorrectFirstSegmentDirection(Vector3[] points)
    {
        int pointCount = points.Length;

        // Check if there are at least three points
        if (pointCount < 3)
        {
            Debug.LogWarning("Not enough points to correct the first segment direction.");
            return;
        }

        // Calculate the average direction of the first few segments
        int maxSegments = 5; // Consider up to 5 segments
        int segmentCount = Mathf.Min(pointCount - 1, maxSegments);
        Vector3 averageDirection = Vector3.zero;

        for (int i = 1; i <= segmentCount; i++)
        {
            averageDirection += (points[i] - points[i - 1]).normalized;
        }
        averageDirection /= segmentCount;

        // Calculate the direction of the first segment
        Vector3 firstSegmentDirection = (points[1] - points[0]).normalized;

        // Calculate the rotation needed to align the first segment with the average direction
        Quaternion rotation = Quaternion.FromToRotation(firstSegmentDirection, averageDirection.normalized);

        // Apply the rotation to the second point
        points[1] = rotation * (points[1] - points[0]) + points[0];
    }

    
}
