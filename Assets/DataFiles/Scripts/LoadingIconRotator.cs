using UnityEngine;

public class LoadingIconRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Speed of rotation in degrees per second.")]
    [SerializeField] private float rotationSpeed = 200f;

    private void Update()
    {
        // Rotate the loading icon around the Z axis (clockwise)
        transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
    }
}
