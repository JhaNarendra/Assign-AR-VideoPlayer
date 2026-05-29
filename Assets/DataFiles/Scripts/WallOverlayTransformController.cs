using UnityEngine;

public class WallOverlayTransformController : MonoBehaviour
{
    [Header("Scale Settings")]
    [SerializeField] private float scaleStep = 0.1f;
    [SerializeField] private float minScaleMultiplier = 0.5f;
    [SerializeField] private float maxScaleMultiplier = 2.5f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationStep = 5f;

    private Vector3 initialScale;
    private float currentScaleMultiplier = 1f;

    private void Awake()
    {
        initialScale = transform.localScale;
    }

    public void ScaleUp()
    {
        currentScaleMultiplier = Mathf.Clamp(
            currentScaleMultiplier + scaleStep,
            minScaleMultiplier,
            maxScaleMultiplier
        );

        ApplyScale();
    }

    public void ScaleDown()
    {
        currentScaleMultiplier = Mathf.Clamp(
            currentScaleMultiplier - scaleStep,
            minScaleMultiplier,
            maxScaleMultiplier
        );

        ApplyScale();
    }

    public void RotateLeft()
    {
        transform.Rotate(0f, 0f, rotationStep, Space.Self);
    }

    public void RotateRight()
    {
        transform.Rotate(0f, 0f, -rotationStep, Space.Self);
    }

    private void ApplyScale()
    {
        transform.localScale = initialScale * currentScaleMultiplier;
    }
}