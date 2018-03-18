using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LineRenderer3D : MonoBehaviour
{
    public enum ENormalMode
    {
        Forward, Right, Up, Left, Back, Down, Delta,
    }

    public Material Material;
    public Transform TrackedTransform;
    public int CircularResolution = 5;
    public AnimationCurve RadiusOverLifetime = AnimationCurve.Linear(0, 1, 1, 1);
    public float RadiusMultiplier = .2f;
    public ENormalMode NormalMode = ENormalMode.Forward;
    public List<PointState> Points = new List<PointState>();

    public UnityEvent OnPositionChanged;

    [Serializable]
    public struct PointState
    {
        public Vector3 Position;
        public Vector3 Normal;   
        public Vector3 Tangent;
        public float Radius;
    }

    protected Vector3 LastPosition { get; private set; }

    protected Transform CurrentTransform
    {
        get { return TrackedTransform ? TrackedTransform : transform; }
    }
    
    private Mesh _mesh;
    private List<Vector3> _vertexBuffer = new List<Vector3>();
    private List<Vector4> _uvBuffer = new List<Vector4>();
    private List<int> _triangleBuffer = new List<int>();
    private MaterialPropertyBlock _materialPropertyBlock;

    protected virtual void Awake()
    {
        gameObject.GetOrAddComponent<MeshRenderer>();
        _mesh = new Mesh();
        LastPosition = CurrentTransform.position;
    }

    protected virtual void Update()
    {
        if (LastPosition != CurrentTransform.position && OnPositionChanged != null)
        {
            OnPositionChanged.Invoke();
        }
        Graphics.DrawMesh(_mesh, transform.localToWorldMatrix, Material, gameObject.layer, null, 0, _materialPropertyBlock);
    }

    protected PointState GetPointState()
    {
        Vector3 normal, tangent;
        switch (NormalMode)
        {
            case ENormalMode.Back:
                normal = -transform.forward;
                tangent = -transform.right;
                break;
            case ENormalMode.Right:
                normal = transform.right;
                tangent = transform.up;
                break;
            case ENormalMode.Up:
                normal = transform.up;
                tangent = transform.forward;
                break;
            case ENormalMode.Delta:
                normal = (LastPosition - CurrentTransform.position).normalized;
                tangent = Vector3.Cross(normal, CurrentTransform.up).normalized;
                break;
            
            default:
                throw new Exception();
        }

        Vector3 position = CurrentTransform.position;

        return new PointState()
        {
            Position = position,
            Normal = normal,
            Radius = RadiusMultiplier,
            Tangent = tangent,
        };
    }
    
    public void RebakeMesh()
    {
        _vertexBuffer.Clear();
        _triangleBuffer.Clear();
        _uvBuffer.Clear();
        _mesh.Clear();

        if (Points.Count < 2)
        {
            return;
        }
        
        for (var i = Points.Count - 1; i >= 0; i--)
        {
            var position = Points[i];
            var maxPoints = (float)GetMaxPoints();
            var lifetime = ((Points.Count - i - 1) / maxPoints);
            var normal = position.Normal;
            var tangent = position.Tangent;
            
            var anglestep = 360 / CircularResolution;
            for (var step = 0; step < CircularResolution; step++)
            {
                var angle = step * anglestep;
                var circlePosition = position.Position + Quaternion.AngleAxis(angle, normal)
                                     * tangent * position.Radius * RadiusOverLifetime.Evaluate(lifetime);
                circlePosition = transform.InverseTransformPoint(circlePosition);

                // Add vertex
                _vertexBuffer.Add(circlePosition);
                _uvBuffer.Add(new Vector4((step / (float)(CircularResolution-1)), lifetime));
                if (i == Points.Count - 1)
                {
                    continue;
                }

                // Add tris
                int p1 = _vertexBuffer.Count - 1;
                int p2 = p1 - CircularResolution;
                int p3 = p1 + 1;
                int p4 = p2 + 1;
                if (step == CircularResolution - 1)
                {
                    p3 -= CircularResolution;
                    p4 -= CircularResolution;
                }
                _triangleBuffer.Add(p1);
                _triangleBuffer.Add(p2);
                _triangleBuffer.Add(p3);

                _triangleBuffer.Add(p3);
                _triangleBuffer.Add(p2);
                _triangleBuffer.Add(p4);
            }
        }
        _mesh.SetVertices(_vertexBuffer);
        _mesh.SetTriangles(_triangleBuffer, 0);
        _mesh.SetUVs(0, _uvBuffer);
        _mesh.RecalculateNormals();
    }

    protected virtual int GetMaxPoints()
    {
        return Points.Count;
    }
    
    public void SetPropertyBlock(MaterialPropertyBlock block)
    {
        _materialPropertyBlock = block;
    }

    private void OnDrawGizmosSelected()
    {
        var ps = GetPointState();
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(ps.Position, ps.Position + ps.Normal);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(ps.Position, ps.Position + ps.Tangent);
        Gizmos.color = Color.white;
    }
}