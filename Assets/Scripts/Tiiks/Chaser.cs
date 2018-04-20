using System.Runtime.InteropServices;
using UnityEngine;

public class Chaser : MonoBehaviour
{
    public float RotationChaseSpeed = 10;
    public Transform Target;
    public Vector3 Rotation = Vector3.zero;
    
    private void Update()
    {
        if (transform.position == Target.position)
        {
            return;
        }
        var dt = Time.deltaTime;
        var targetPosition = Target.position;
        var targetRotation = Quaternion.Euler(Rotation) * Quaternion.LookRotation(targetPosition, transform.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationChaseSpeed * dt);
        
        transform.LookAt(Target, transform.up);
        transform.position = Target.position;
    }
}