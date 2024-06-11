using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class LineContainer : MonoBehaviour
{
    public List<Vector3[]> ShapeData;
    public List<string> Shapename;
    public bool DEBUG;
    
    
    // Start is called before the first frame update
    void Start()
    {
        ShapeData = new List<Vector3[]>();
        
        Directory.CreateDirectory(Application.dataPath + "/Shapes");
        foreach (var name in Shapename)
        {
            Debug.Log($"Loading {name}");
            ShapeData.Add(JsonConvert.DeserializeObject<Vector3[]>(File.ReadAllText(Application.dataPath + $"/Shapes/{name}.txt")));
        }
        
        // var ShownLine = new GameObject("Line").AddComponent<LineRenderer>();
        // ShownLine.positionCount = 60;
        // ShownLine.startWidth = 0.1f;
        // ShownLine.endWidth = 0.1f;
        //
        // ShownLine.SetPositions(ShapeData);
        // ShownLine.material = new Material(Shader.Find("Unlit/Color"));
        // ShownLine.material.color = Color.white;
    }
    
    public void CheckData(Vector3[] Sample)
    {
        float[] Deviations = new float[] { };

        // (float, string) Best = (500,"null");
        Debug.Log($"Shapes: {ShapeData.Count}, Names: {Shapename.Count}");
        List<(float, string)> CrossChecks = new List<(float, string)>();
        for (int i = 0; i < ShapeData.Count; i++)
        {
            Deviations = new float[60];
            float DeviAverage = 0;
            
            
            var N = Shapename[i];
            var Dat = ShapeData[i];
            Debug.Log($"Checking with shape: {N}");
            for (int j = 0; j < 60; j++)
            {
                Deviations[j] = Vector3.Distance(Sample[j], Dat[j]);
                DeviAverage += Deviations[j];
            }
            DeviAverage /= 60;
            
            CrossChecks.Add((DeviAverage,N));
        }
        
        
        
        
        
        // Debug.Log($"{Best} is the Mean Deviation");
        // Debug.Log(String.Join(", ",Deviations));
        foreach (var valueTuple in CrossChecks)
        {
            Debug.Log(valueTuple);
        }

        if (DEBUG)
        {
            var ShownLine = new GameObject("Line").AddComponent<LineRenderer>();
            ShownLine.positionCount = 60;
            ShownLine.startWidth = 0.1f;
            ShownLine.endWidth = 0.1f;

            var Chart = new Vector3[60];

            for (int i = 0; i < 60; i++)
            {
                Chart[i] = new Vector3((float)i/5, Mathf.Pow(Deviations[i],4), 0);
            }
            ShownLine.SetPositions(Chart);
            ShownLine.material = new Material(Shader.Find("Unlit/Color"));
            ShownLine.material.color = Color.white;
            
            var ShownLine2 = new GameObject("Line").AddComponent<LineRenderer>();
            ShownLine2.positionCount = 60;
            ShownLine2.startWidth = 0.1f;
            ShownLine2.endWidth = 0.1f;
            
            // ShownLine2.SetPositions(ShapeData);
            // ShownLine2.material = new Material(Shader.Find("Unlit/Color"));
            // ShownLine2.material.color = Color.red;
        }
        

    }
}
