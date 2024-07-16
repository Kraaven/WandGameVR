using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Newtonsoft.Json;
using Spells;
using UnityEngine;
public class LineContainerPen : MonoBehaviour
{
    public List<Vector3[]> ShapeData;
    public List<string> Shapename;
    public bool DEBUG;
    public float CheckThreshold;
    private Transform TIP;
    
    //public List<GameObject> Shapes;

    //public List<Spell> Spells;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        ShapeData = new List<Vector3[]>();
        TIP = GetComponent<LineInputPen>().Hand;
        
        Directory.CreateDirectory(Application.dataPath + "/Shapes");
        foreach (var name in Shapename)
        {
            Debug.Log($"Loading {name}");
            ShapeData.Add(JsonConvert.DeserializeObject<Vector3[]>(File.ReadAllText(Application.dataPath + $"/Shapes/{name}.txt")));

            if (DEBUG)
            {
                var l = new GameObject($"Letter : {name}", new []{typeof(LineRenderer)}).GetComponent<LineRenderer>();
                l.positionCount = 60;
                l.material = new Material(Shader.Find("Unlit/Color"));
                l.material.color = Color.white;
                l.SetPositions(ShapeData.Last());
                l.startColor = l.endColor = Color.white;
                l.startWidth = l.endWidth = 0.1f;
            }
        }
        
    }
    
        public void CheckData(Vector3[] drawnShape)
        {
            var minDistance = CheckThreshold;
        if (ShapeData == null || ShapeData.Count == 0)
        {
            Debug.LogError("No stored shapes to compare.");
            return;
        }

        int bestShapeIndex = -1;

        for (int i = 0; i < ShapeData.Count; i++)
        {
            float distance = ComputeProcrustesDistance(drawnShape, ShapeData[i]);
            Debug.Log($"{Shapename[i]} has the deviation {distance}");
            if (distance < minDistance)
            {
                minDistance = distance;
                bestShapeIndex = i;
            }
        }

        if (bestShapeIndex >= 0)
        {
            Debug.Log($"Best matching shape is at shape: {Shapename[bestShapeIndex]} with a distance of: {minDistance}");
            // DisplayShape(Shapename[bestShapeIndex]);
        }
        else
        {
            Debug.Log("No matching shape found.");
        }
    }

    private float ComputeProcrustesDistance(Vector3[] shape1, Vector3[] shape2)
    {
        // Normalize and align the shapes
        shape1 = NormalizeShape(shape1);
        shape2 = NormalizeShape(shape2);

        // Compute the Procrustes distance
        var alignedShape2 = AlignShapes(shape1, shape2, out float disparity);

        return disparity;
    }

    private Vector3[] NormalizeShape(Vector3[] shape)
    {
        // Translate the shape to the origin
        shape = TranslateToOrigin(shape);

        // Scale the shape to ensure the distance between the first two points is 1
        shape = ScaleToSetSize(shape, 1.0f);

        return shape;
    }

    private Vector3[] AlignShapes(Vector3[] shape1, Vector3[] shape2, out float disparity)
    {
        // Center shapes
        Vector3[] centeredShape1 = CenterShape(shape1);
        Vector3[] centeredShape2 = CenterShape(shape2);

        // Calculate the optimal rotation matrix using Singular Value Decomposition (SVD)
        Matrix4x4 rotationMatrix = CalculateOptimalRotation(centeredShape1, centeredShape2);

        // Apply the rotation to shape2
        Vector3[] alignedShape2 = ApplyRotation(centeredShape2, rotationMatrix);

        // Calculate the disparity (sum of squared distances between corresponding points)
        disparity = CalculateDisparity(centeredShape1, alignedShape2);

        return alignedShape2;
    }

    private Vector3[] CenterShape(Vector3[] shape)
    {
        Vector3 centroid = shape.Aggregate(Vector3.zero, (acc, p) => acc + p) / shape.Length;
        return shape.Select(p => p - centroid).ToArray();
    }

    private Matrix4x4 CalculateOptimalRotation(Vector3[] shape1, Vector3[] shape2)
    {
        // Create MathNet matrices from the shapes
        var matrix1 = DenseMatrix.OfArray(To2DArray(shape1));
        var matrix2 = DenseMatrix.OfArray(To2DArray(shape2));

        // Calculate the cross-covariance matrix
        var covarianceMatrix = matrix1.TransposeThisAndMultiply(matrix2);

        // Perform Singular Value Decomposition (SVD)
        var svd = covarianceMatrix.Svd();

        // Calculate the rotation matrix
        var rotationMatrix = svd.U * svd.VT;

        // Convert the MathNet matrix to Unity's Matrix4x4
        return ToMatrix4x4(rotationMatrix);
    }

    private double[,] To2DArray(Vector3[] vectors)
    {
        var array = new double[vectors.Length, 3];
        for (int i = 0; i < vectors.Length; i++)
        {
            array[i, 0] = vectors[i].x;
            array[i, 1] = vectors[i].y;
            array[i, 2] = vectors[i].z;
        }
        return array;
    }

    private Matrix4x4 ToMatrix4x4(Matrix<double> matrix)
    {
        var result = Matrix4x4.identity;
        result.m00 = (float)matrix[0, 0];
        result.m01 = (float)matrix[0, 1];
        result.m02 = (float)matrix[0, 2];
        result.m10 = (float)matrix[1, 0];
        result.m11 = (float)matrix[1, 1];
        result.m12 = (float)matrix[1, 2];
        result.m20 = (float)matrix[2, 0];
        result.m21 = (float)matrix[2, 1];
        result.m22 = (float)matrix[2, 2];
        return result;
    }

    private Vector3[] ApplyRotation(Vector3[] shape, Matrix4x4 rotationMatrix)
    {
        return shape.Select(p => rotationMatrix.MultiplyPoint3x4(p)).ToArray();
    }

    private float CalculateDisparity(Vector3[] shape1, Vector3[] shape2)
    {
        float disparity = 0.0f;
        for (int i = 0; i < shape1.Length; i++)
        {
            disparity += (shape1[i] - shape2[i]).sqrMagnitude;
        }
        return disparity;
    }

    private Vector3[] TranslateToOrigin(Vector3[] points)
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


    // void DisplayShape(string Shape)
    // {
    //     switch (Shape)
    //     {
    //         case "Square":
    //             Instantiate(Spells[0], TIP.position,TIP.rotation);
    //             break;
    //         case "Triangle":
    //             Instantiate(Spells[1], TIP.position,TIP.rotation);
    //             break;
    //         
    //         case "Teleport":
    //             Instantiate(Spells[2], TIP.position,TIP.rotation);
    //             break;
    //         default:
    //             Debug.Log("Spell does not exist");
    //             break;
    //     }
    // }
    
}
