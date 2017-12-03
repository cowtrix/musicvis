using System.Collections.Generic;
using System.Configuration.Assemblies;
using UnityEngine;

public class LineRenderer3D : MonoBehaviour
{
    public int CircularResolution = 5;
    public int HistoryLimit = 200;
    public float Distance = .1f;

    public AnimationCurve RadiusOverLifetime = AnimationCurve.Linear(0, 1, 1, 1);
    public float Radius = .2f;

    public Vector3 Velocity;

    private struct PointState
    {
        public Vector3 Position;
        public float Radius;
    }
    
    private List<PointState> _positionHistory;
    private Vector3 _lastPosition;

    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private List<Vector3> _vertexBuffer = new List<Vector3>();
    private List<int> _triangleBuffer = new List<int>();

    private void Awake()
    {
        _positionHistory = new List<PointState>(HistoryLimit);
        _meshFilter = GetComponent<MeshFilter>();
        _mesh = new Mesh();
        _meshFilter.sharedMesh = _mesh;
    }

    public bool temp;
    private void Update()
    {
        if (_lastPosition == transform.position)
        {
            return;
        }
        _lastPosition = transform.position;

        _positionHistory.Add(new PointState(){
            Position = transform.position,
            Radius = Radius,
        });
        while (_positionHistory.Count > HistoryLimit)
        {
            _positionHistory.RemoveAt(0);
        }

        if (_positionHistory.Count < 2)
        {
            return;
        }

        float dt = Time.deltaTime;
        _vertexBuffer.Clear();
        _triangleBuffer.Clear();
        var prevPos = _positionHistory[_positionHistory.Count - 2];
        float distAccum = 0;
        for (var i = _positionHistory.Count-1; i >= 0; i--)
        {
            var position = _positionHistory[i];
            position.Position += Velocity * dt;

            var dist = Vector3.Distance(prevPos.Position, position.Position);
            distAccum += dist;
            if (distAccum < Distance && (i != 0 && i != _positionHistory.Count - 1))
            {
                continue;
            }
            distAccum = 0;

            var normal = (position.Position - prevPos.Position).normalized;
            var tangent = Vector3.Cross(normal, Vector3.up).normalized;
            
            var anglestep = 360 / CircularResolution;
            for (var step = 0; step < CircularResolution; step++)
            {
                var angle = step * anglestep;
                float lifetime = 1 - (i / (float)(_positionHistory.Count - 1));
                var circlePosition = position.Position + Quaternion.AngleAxis(angle, normal) 
                    * tangent * position.Radius * RadiusOverLifetime.Evaluate(lifetime);
                circlePosition = transform.InverseTransformPoint(circlePosition);

                // Add vertex
                _vertexBuffer.Add(circlePosition);
                if (i == _positionHistory.Count - 1)
                {
                    continue;
                }

                // Add tris
                int p1 = _vertexBuffer.Count;
                int p2 = p1 - (CircularResolution);
                int p3 = p1 - 1;
                int p4 = p2 - 1;
                if (step == CircularResolution - 1)
                {
                    p1 = _vertexBuffer.Count - 1;
                    p2 = p1 - CircularResolution;
                    p3 = p1 - step;
                    p4 = p2 - step;

                    _triangleBuffer.Add(p1);
                    _triangleBuffer.Add(p2);
                    _triangleBuffer.Add(p3);

                    _triangleBuffer.Add(p3);
                    _triangleBuffer.Add(p2);
                    _triangleBuffer.Add(p4);
                }
                else
                {
                    _triangleBuffer.Add(p3);
                    _triangleBuffer.Add(p2);
                    _triangleBuffer.Add(p1);

                    _triangleBuffer.Add(p4);
                    _triangleBuffer.Add(p2);
                    _triangleBuffer.Add(p3);
                }
            }
            prevPos = position;
            _positionHistory[i] = position;
        }

        _mesh.SetVertices(_vertexBuffer);
        _mesh.RecalculateNormals();
        _mesh.SetTriangles(_triangleBuffer, 0);

        Debug.Break();
    }

}