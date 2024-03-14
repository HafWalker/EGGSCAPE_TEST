using UnityEngine;

/// <summary>
/// Basic Constant Rotation
/// </summary>
public class ConstantRotation : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 10f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);    
    }
}