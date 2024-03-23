using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!GUILayout.Button("Create Mesh")) return;
        var fov = (FieldOfView)target;
        fov.CreateMesh(fov.GetComponent<MeshFilter>());
    }
}

#endif

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float fov = 90f;
    [SerializeField] private float viewDistance = 50f;
    [SerializeField] private LayerMask wallMask;
    

    private MeshFilter _meshFilter;
    private float _startingAngle;
    private Vector3 _origin;
    
    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _origin = Vector3.zero;
    }

    private void Update()
    {
        CreateMesh(_meshFilter);
    }

    public void CreateMesh(MeshFilter meshFilter)
    {
        const uint rayCount = 50;
        var angle = _startingAngle;
        var angleIncrease = fov / rayCount;
        

        var vertices = new Vector3[rayCount + 1 + 1];
        var uv = new Vector2[vertices.Length];
        var triangles = new int[rayCount * 3];

        vertices[0] = _origin;

        var vertexIndex = 1;
        var triangleIndex = 0;
        for (var i = 0; i <= rayCount; i++)
        {
            // Credit: CODE MONKEY
            // angle = 0 -> 360
            var angleRad = angle * (Mathf.PI / 180f);
            var angleVector = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            
            var hit = Physics2D.Raycast(_origin, angleVector, viewDistance, wallMask);
            var vertex = hit.collider == null ? _origin + angleVector * viewDistance : (Vector3)hit.point;
            
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }
        
        var mesh = new Mesh
        {
            vertices = vertices,
            uv = uv,
            triangles = triangles
        };
        meshFilter.mesh = mesh;
    }

    // Credit: CODE MONKEY
    private static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        var n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 350;

        return n;
    }
    
    public void SetAimDirection(Vector3 dir)
    {
        _startingAngle = GetAngleFromVectorFloat(dir) + fov / 1.7f;
    }

    public void SetOrigin(Vector3 origin)
    {
        _origin = origin;
    }
}
