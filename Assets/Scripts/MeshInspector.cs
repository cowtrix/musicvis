using UnityEditor;
using UnityEngine;

public class MeshInspector : MonoBehaviour
{
    public bool Enabled;
    private MeshFilter _meshFilter;

    private void OnDrawGizmos()
    {
        if(!Enabled)
        {
            return;
        }

        if (_meshFilter == null)
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        var mesh = _meshFilter.sharedMesh;
        if (mesh == null)
        {
            return;
        }

        for (var i = 0; i < mesh.vertices.Length; i++)
        {
            var meshVertex = mesh.vertices[i];
            var wPos = transform.TransformPoint(meshVertex);
            Handles.Label(wPos, i.ToString());
        }
    }
}