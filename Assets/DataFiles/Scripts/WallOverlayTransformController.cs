using UnityEngine;

public class WallOverlayTransformController : MonoBehaviour
{
    [Header("Scale")]
    [SerializeField] private float scaleStep = 0.1f;
    [SerializeField] private float minScale = 0.4f;
    [SerializeField] private float maxScale = 2.5f;

    [Header("Rotation")]
    [SerializeField] private float rotationStep = 5f;

    private float currentScaleMultiplier = 1f;

    public void ScaleUp()
    {
        currentScaleMultiplier = Mathf.Clamp(
            currentScaleMultiplier + scaleStep,
            minScale,
            maxScale
        );

        transform.localScale *= 1f + scaleStep;
    }

    public void ScaleDown()
    {
        currentScaleMultiplier = Mathf.Clamp(
            currentScaleMultiplier - scaleStep,
            minScale,
            maxScale
        );

        transform.localScale *= 1f - scaleStep;
    }

    public void RotateLeft()
    {
        transform.Rotate(Vector3.forward, rotationStep, Space.Self);
    }

    public void RotateRight()
    {
        transform.Rotate(Vector3.forward, -rotationStep, Space.Self);
    }
}